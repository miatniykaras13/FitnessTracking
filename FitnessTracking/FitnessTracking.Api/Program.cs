using FitnessTracking.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProgramDependencies(builder.Configuration);

var app = builder.Build();

app.UseApiPipeline(builder.Configuration);

await app.UseMigratingAsync();

app.Run();