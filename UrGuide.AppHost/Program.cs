var builder = DistributedApplication.CreateBuilder(args);

var sqlPassword = builder.AddParameter("sql-password", secret: true);

var rabbitMqUser = builder.AddParameter(
    "rabbitmq-user",
    "guest",
    publishValueAsDefault: true,
    secret: false);

var rabbitMqPassword = builder.AddParameter("rabbitmq-password", secret: true);

var sqlServer = builder.AddSqlServer("sqlserver", sqlPassword, port: 14330)
    .WithDataVolume("sqlserver-data");

var authDatabase = sqlServer.AddDatabase("auth-db", "urguide_id4");
var appDatabase = sqlServer.AddDatabase("data-db", "urguide_data");

var rabbitMq = builder.AddRabbitMQ("rabbitmq", rabbitMqUser, rabbitMqPassword, port: 5672)
    .WithDataVolume("rabbitmq-data")
    .WithManagementPlugin(port: 15672);

var elasticsearch = builder.AddElasticsearch("elasticsearch", password: null, port: 9200)
    .WithDataVolume("elasticsearch-data")
    .WithImageRegistry("docker.elastic.co")
    .WithImage("elasticsearch/elasticsearch", "8.11.0")
    .WithEnvironment("discovery.type", "single-node")
    .WithEnvironment("xpack.security.enabled", "false")
    .WithEnvironment("ES_JAVA_OPTS", "-Xms512m -Xmx512m")
    .WithHttpHealthCheck("/_cluster/health");

var seq = builder.AddContainer("seq", "datalust/seq", "latest")
    .WithEnvironment("ACCEPT_EULA", "Y")
    .WithVolume("seq-data", "/data")
    .WithHttpEndpoint(targetPort: 5341, port: 5341, name: "ingestion")
    .WithHttpEndpoint(targetPort: 80, port: 8080, name: "http")
    .WithHttpHealthCheck("/health", endpointName: "http");

var api = builder.AddProject(
        "api",
        "../UrGuide.WebApp/UrGuide.WebApp.csproj",
        options =>
        {
            options.ExcludeLaunchProfile = true;
            options.ExcludeKestrelEndpoints = true;
        })
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:80")
    .WithEnvironment("ApplicationUri", "http://localhost:5000")
    .WithReference(authDatabase, "AuthConnection")
    .WithReference(appDatabase, "DefaultConnection")
    .WithEnvironment("RabbitMQ__Host", "rabbitmq")
    .WithEnvironment("RabbitMQ__Username", rabbitMqUser)
    .WithEnvironment("RabbitMQ__Password", rabbitMqPassword)
    .WithEnvironment("Elasticsearch__Url", elasticsearch)
    .WithEnvironment("Seq__ServerUrl", seq.GetEndpoint("ingestion"))
    .WithHttpEndpoint(targetPort: 80, port: 5000, name: "http")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WaitFor(authDatabase)
    .WaitFor(appDatabase)
    .WaitFor(rabbitMq)
    .WaitFor(elasticsearch)
    .WaitForStart(seq);

builder.AddDockerfile("admin-dashboard", "../admin-dashboard")
    .WithHttpEndpoint(targetPort: 80, port: 3001, name: "http")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WaitFor(api);

builder.AddDockerfile("guide-portal", "../guide-portal")
    .WithHttpEndpoint(targetPort: 80, port: 3002, name: "http")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WaitFor(api);

builder.AddDockerfile("tourist-website", "../tourist-website")
    .WithHttpEndpoint(targetPort: 80, port: 3003, name: "http")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WaitFor(api);

builder.Build().Run();
