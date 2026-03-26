using FitnessTracking.Api;
using Carter;
using FitnessTracking.Api.Middleware;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

services.AddProgramDependencies(configuration);

var app = builder.Build();

var uploadsRoot = Path.Combine(builder.Environment.ContentRootPath, configuration["FileStorage:RootPath"] ?? "uploads");
Directory.CreateDirectory(uploadsRoot);

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsRoot),
    RequestPath = "/uploads"
});

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

await app.UseMigrating();


app.Run();