using Microsoft.Extensions.DependencyInjection;
using WebStore.Interfaces.Service;
using WebStore.Services.Products.InCookies;
using WebStore.Services.Products.InSQL;

namespace WebStore.Services.Products
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddWebStoreServices(this IServiceCollection services)
        {
            services.AddScoped<IEmployeesData, SqlEmployeesData>();
            services.AddScoped<IProductData, SqlProductData>();
            services.AddScoped<IOrderService, SqlOrderService>();
            services.AddScoped<ICartService, CookiesCartService>();

            return services;
        }
    }
}
