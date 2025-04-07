using Battleships.Management;
using Microsoft.Extensions.DependencyInjection;

namespace Battleships.Startup
{
    public static class DependencyResolver
    {
        public static IServiceProvider RegisterServices()
        {
            var services = new ServiceCollection();
            services.AddScoped<IBoardManagement, BoardManagement>();
            services.AddScoped<Program>();

            return services.BuildServiceProvider();
        }

        public static void DisposeServices(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                return;
            }
            if (serviceProvider is IDisposable sp)
            {
                sp.Dispose();
            }
        }
    }
}
