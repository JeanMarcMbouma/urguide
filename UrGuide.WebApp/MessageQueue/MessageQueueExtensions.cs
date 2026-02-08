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
        var rabbitMqHost = configuration.GetValue<string>("RabbitMQ:Host") ?? "localhost";
        var rabbitMqVirtualHost = configuration.GetValue<string>("RabbitMQ:VirtualHost") ?? "/";
        var rabbitMqUsername = configuration.GetValue<string>("RabbitMQ:Username") ?? "guest";
        var rabbitMqPassword = configuration.GetValue<string>("RabbitMQ:Password") ?? "guest";
        var useQueuedServices = configuration.GetValue<bool>("MessageQueue:UseQueuedServices", false);

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

                // Configure retry policy
                cfg.UseMessageRetry(r => r.Intervals(
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(15),
                    TimeSpan.FromSeconds(30)
                ));

                // Configure dead letter queue
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

                cfg.ConfigureEndpoints(context);
            });
        });

        // Optionally replace synchronous services with queued versions
        if (useQueuedServices)
        {
            // Replace IEmailService with QueuedEmailService
            services.AddTransient<IEmailService, QueuedEmailService>();
            
            // Note: We keep the original services available for direct/synchronous use cases
            // Register the queued notification service
            services.AddTransient<QueuedNotificationService>();
        }

        return services;
    }

    public static IHealthChecksBuilder AddMessageQueueHealthChecks(
        this IHealthChecksBuilder builder,
        IConfiguration configuration)
    {
        var rabbitMqHost = configuration.GetValue<string>("RabbitMQ:Host") ?? "localhost";
        var rabbitMqVirtualHost = configuration.GetValue<string>("RabbitMQ:VirtualHost") ?? "/";
        var rabbitMqUsername = configuration.GetValue<string>("RabbitMQ:Username") ?? "guest";
        var rabbitMqPassword = configuration.GetValue<string>("RabbitMQ:Password") ?? "guest";

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
                // RabbitMQ.Client 7.0 uses async methods
                return factory.CreateConnectionAsync().GetAwaiter().GetResult();
            },
            name: "rabbitmq",
            tags: new[] { "messagequeue", "rabbitmq" });

        return builder;
    }
}
