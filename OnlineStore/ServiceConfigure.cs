using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineStore.Data;
using OnlineStore.Data.Interfaces;
using OnlineStore.Models.Database;

namespace OnlineStore;

public static class ServiceConfigure
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDb(configuration);
        services.AddRepositories();
        services.AddAuth();

        return services;
    }

    private static void AddDb(this IServiceCollection services, IConfiguration configuration)
    {
        const string mssqlConnectionSection = "MSSqlServer";
        var mssqlConnectionString = configuration.GetConnectionString(mssqlConnectionSection);
        services.AddSqlServer<ApplicationContext>(mssqlConnectionString);
    }

    private static void AddAuth(this IServiceCollection services)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options => { options.LoginPath = new PathString("/Account/Login"); });
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        AddRepository<Category>();
        AddRepository<Comment>();
        AddRepository<Image>();
        AddRepository<Product>();
        AddRepository<PurchaseProduct>();
        AddRepository<Purchase>();
        AddRepository<Rate>();
        AddRepository<ShoppingCart>();
        AddRepository<User>();
        AddRepository<UserRole>();

        void AddRepository<T>() where T : class
        {
            services.AddScoped<IRepository<T>, Repository<T>>()
                .AddScoped(serviceProvider =>
                    new Lazy<IRepository<T>>(serviceProvider.GetRequiredService<IRepository<T>>));
        }
    }
}