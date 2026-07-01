using Booking.Config;
using Booking.Interfaces.Repositories;
using Booking.Interfaces.Services;
using Booking.Repositories;
using Booking.Services;
using Microsoft.Extensions.Caching.StackExchangeRedis;

namespace Booking
{
    public static class Container
    {
        public static IServiceCollection RegisterServices(
            this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            services.AddSingleton<Database>();

            // Redis
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("RedisConnection");
                options.InstanceName = "Booking:";
            });

            //Email
            services.AddScoped<IEmailService, EmailService>();

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();

            // Services
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}