using FitnessTracking.Api.Constants;
using FitnessTracking.Infrastructure.Persistence;
using FitnessTracking.Api.Middleware;
using Carter;
using FitnessTracking.Shared.Constants;
using Microsoft.Extensions.FileProviders;

namespace FitnessTracking.Api;

public static class ApiExtensions
{
    public static WebApplication UseApiPipeline(this WebApplication app, IConfiguration configuration)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseUploadsStaticFiles(configuration);
        app.MapCarter();
        app.UseDevelopmentSwagger();
        app.UseHttpsRedirection();

        return app;
    }

    public static async Task<WebApplication> UseMigratingAsync(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<FitnessTrackingDbContext>();
            await dbContext.Database.EnsureCreatedAsync();
        }
        return app;
    }

    private static WebApplication UseUploadsStaticFiles(this WebApplication app, IConfiguration configuration)
    {
        var uploadsRoot = Path.Combine(
            app.Environment.ContentRootPath,
            configuration[FileStorageConstants.RootPathConfigKey] ?? FileStorageConstants.DefaultRootFolderName);

        Directory.CreateDirectory(uploadsRoot);

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(uploadsRoot),
            RequestPath = FileStorageConstants.UploadsRequestPath
        });

        return app;
    }

    private static WebApplication UseDevelopmentSwagger(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            return app;
        }

        app.UseSwagger();
        app.UseSwaggerUI(o =>
        {
            o.SwaggerEndpoint(ApiConstants.Swagger.JsonEndpoint, ApiConstants.Swagger.ApiVersionName);
            o.RoutePrefix = ApiConstants.Swagger.DocsRoutePrefix;
        });

        return app;
    }
}