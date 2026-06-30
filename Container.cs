using Booking.Config;
using Booking.Interfaces.Repositories;
using Booking.Interfaces.Services;
using Booking.Repositories;
using Booking.Services;

namespace Booking
{
    public static class Container
    {
        public static IServiceCollection RegisterServices(
            this IServiceCollection services)
        {
            // Database
            services.AddSingleton<Database>();

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();

            // Services
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}