using FitnessTracking.Api;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

services.AddProgramDependencies(configuration);

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(o => 
    {
        o.RoutePrefix = "/docs";
    });
}

app.UseHttpsRedirection();

app.Run();
