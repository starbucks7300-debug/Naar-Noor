using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Infrastructure.Data;

namespace NaarNoor.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var raw = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(raw))
            raw = configuration["DATABASE_URL"];
        if (string.IsNullOrWhiteSpace(raw))
            throw new InvalidOperationException("Database connection string not found. Set DATABASE_URL environment variable.");

        var connectionString = NormaliseConnectionString(raw);

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        return services;
    }

    private static string NormaliseConnectionString(string value)
    {
        if (!value.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase) &&
            !value.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase))
            return value;

        var uri = new Uri(value);
        var userInfo = uri.UserInfo.Split(':', 2);
        var user = Uri.UnescapeDataString(userInfo[0]);
        var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty;
        var host = uri.Host;
        var port = uri.IsDefaultPort ? 5432 : uri.Port;
        var database = uri.AbsolutePath.TrimStart('/');

        return $"Host={host};Port={port};Database={database};Username={user};Password={password};Trust Server Certificate=true;SSL Mode=Prefer;";
    }
}
