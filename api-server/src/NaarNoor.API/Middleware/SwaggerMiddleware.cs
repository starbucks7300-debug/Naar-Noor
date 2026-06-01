namespace NaarNoor.API.Middleware;

/// <summary>
/// Swagger UI middleware
/// </summary>
public static class SwaggerMiddleware
{
    public static void UseSwaggerMiddleware(this WebApplication app)
    {
        app.UseSwagger(c =>
        {
            c.RouteTemplate = "api/swagger/{documentName}/swagger.json";
        });
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/api/swagger/v1/swagger.json", "Naar & Noor API v1");
            c.RoutePrefix = "api/docs";
        });
    }
}
