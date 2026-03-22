using FitnessTracking.Api;
using Carter;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

services.AddProgramDependencies(configuration);

var app = builder.Build();

app.MapCarter();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    
    app.UseSwaggerUI(o =>
    {
        o.SwaggerEndpoint("/swagger/v1/swagger.json", "FitnessTracking.Api v1");
        o.RoutePrefix = "docs";
    });
}

app.UseHttpsRedirection();


app.Run();