var builder = DistributedApplication.CreateBuilder(args);

var dbUsername = builder.AddParameter("scrb-db-username", "scribbly");
var dbPassword = builder.AddParameter("scrb-db-password", "B@nanaPh0n3", true);

var postgres = builder
    .AddPostgres(
        name: "scrb-db", 
        userName: dbUsername, 
        password: dbPassword,
        port: 14559)
    .WithPgAdmin()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithBindMount(source: Path.Combine(Directory.GetCurrentDirectory(), @"..\..\_data"), target: "/var/lib/postgresql/data");

var readDatabase = postgres.AddDatabase("scrb-db-read");
var eventsDatabase = postgres.AddDatabase("scrb-db-events");

var apiService = builder
    .AddProject<Projects.Scribbly_Eventually_Cookbook_Api>("scrb-api")
    .WithReference(readDatabase)
    .WithReference(eventsDatabase)
    .WaitFor(readDatabase)
    .WaitFor(eventsDatabase);

builder.AddProject<Projects.Scribbly_Eventually_Cookbook_Web>("scrb-web")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
