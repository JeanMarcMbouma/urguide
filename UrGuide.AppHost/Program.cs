using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Add SQL Server
var sqlServer = builder.AddSqlServer("sqlserver")
    .WithDataVolume();

// Add databases
var urguideDb = sqlServer.AddDatabase("urguide_data");
var urguideAuthDb = sqlServer.AddDatabase("urguide_id4");

// Add the main web application
var webApp = builder.AddProject<Projects.UrGuide_WebApp>("webapp")
    .WithReference(urguideDb)
    .WithReference(urguideAuthDb)
    .WithHttpsEndpoint(port: 7080, name: "https")
    .WithHttpEndpoint(port: 5080, name: "http");

// Add the frontend React app (served by the ASP.NET Core SPA middleware)
webApp.WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName);

var app = builder.Build();

app.Run();