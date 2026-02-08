using System;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using UrGuide.WebApp.MessageQueue.Consumers;
using UrGuide.WebApp.MessageQueue.Services;
using UrGuide.Shared.Contracts;
using UrGuide.Services.Contracts;

namespace UrGuide.WebApp.MessageQueue;

/// <summary>
/// Extension methods for configuring message queue services
/// </summary>
public static class MessageQueueExtensions
{
    public static IServiceCollection AddMessageQueue(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var useQueuedServices = configuration.GetValue<bool>("MessageQueue:UseQueuedServices", false);
        
        // Only configure MassTransit/RabbitMQ if queued services are enabled
        if (!useQueuedServices)
        {
            return services;
        }

        var rabbitMqHost = configuration.GetValue<string>("RabbitMQ:Host") ?? "localhost";
        var rabbitMqVirtualHost = configuration.GetValue<string>("RabbitMQ:VirtualHost") ?? "/";
        var rabbitMqUsername = configuration.GetValue<string>("RabbitMQ:Username") ?? "guest";
        var rabbitMqPassword = configuration.GetValue<string>("RabbitMQ:Password");
        
        if (string.IsNullOrEmpty(rabbitMqPassword))
        {
            throw new InvalidOperationException(
                "RabbitMQ:Password must be configured via environment variables or user secrets when MessageQueue:UseQueuedServices is enabled.");
        }

        // Add MassTransit with RabbitMQ
        services.AddMassTransit(x =>
        {
            // Register consumers
            x.AddConsumer<SendEmailConsumer>();
            x.AddConsumer<ProcessImageConsumer>();
            x.AddConsumer<SendNotificationConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitMqHost, rabbitMqVirtualHost, h =>
                {
                    h.Username(rabbitMqUsername);
                    h.Password(rabbitMqPassword);
                });

                // Configure receive endpoints with retry policies (no global retry to avoid stacking)
                cfg.ReceiveEndpoint("email-queue", e =>
                {
                    e.ConfigureConsumer<SendEmailConsumer>(context);
                    e.UseMessageRetry(r => r.Intervals(
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(15),
                        TimeSpan.FromSeconds(30)
                    ));
                });

                cfg.ReceiveEndpoint("image-processing-queue", e =>
                {
                    e.ConfigureConsumer<ProcessImageConsumer>(context);
                    e.UseMessageRetry(r => r.Intervals(
                        TimeSpan.FromSeconds(10),
                        TimeSpan.FromSeconds(30),
                        TimeSpan.FromSeconds(60)
                    ));
                });

                cfg.ReceiveEndpoint("notification-queue", e =>
                {
                    e.ConfigureConsumer<SendNotificationConsumer>(context);
                    e.UseMessageRetry(r => r.Intervals(
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(15),
                        TimeSpan.FromSeconds(30)
                    ));
                });

                // Do not call ConfigureEndpoints - we've explicitly configured all endpoints above
            });
        });

        // Replace synchronous services with queued versions
        // Note: Consumers will resolve the concrete synchronous service, not the queued one
        services.AddTransient<IEmailService, QueuedEmailService>();
        services.AddTransient<QueuedNotificationService>();

        return services;
    }

    public static IHealthChecksBuilder AddMessageQueueHealthChecks(
        this IHealthChecksBuilder builder,
        IConfiguration configuration)
    {
        var enableMonitoring = configuration.GetValue<bool>("MessageQueue:EnableMonitoring", false);
        var useQueuedServices = configuration.GetValue<bool>("MessageQueue:UseQueuedServices", false);
        
        // Only add RabbitMQ health checks if queued services and monitoring are enabled
        if (!useQueuedServices || !enableMonitoring)
        {
            return builder;
        }

        var rabbitMqHost = configuration.GetValue<string>("RabbitMQ:Host") ?? "localhost";
        var rabbitMqVirtualHost = configuration.GetValue<string>("RabbitMQ:VirtualHost") ?? "/";
        var rabbitMqUsername = configuration.GetValue<string>("RabbitMQ:Username") ?? "guest";
        var rabbitMqPassword = configuration.GetValue<string>("RabbitMQ:Password");

        if (string.IsNullOrEmpty(rabbitMqPassword))
        {
            return builder; // Skip health check if no password configured
        }

        // RabbitMQ health check using connection factory
        builder.AddRabbitMQ(
            sp =>
            {
                var factory = new RabbitMQ.Client.ConnectionFactory
                {
                    HostName = rabbitMqHost,
                    VirtualHost = rabbitMqVirtualHost,
                    UserName = rabbitMqUsername,
                    Password = rabbitMqPassword
                };
                // Use synchronous connection creation to avoid blocking issues
                return factory.CreateConnectionAsync().GetAwaiter().GetResult();
            },
            name: "rabbitmq",
            tags: new[] { "messagequeue", "rabbitmq" });

        return builder;
    }
}
