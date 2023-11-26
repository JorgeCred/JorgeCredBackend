using JorgeCred.Data.Context;
using JorgeCred.Domain;
using JorgeCred.Identity.Data;
using JorgeCred.Identity.Services;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JorgeCred.API.IoC
{
    public static class NativeInjectorConfig
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<JorgeContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("JorgeCredDatabase"))
            );

            services.AddDbContext<IdentityDataContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("JorgeCredDatabase"))
            );

            services.AddDefaultIdentity<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<IdentityDataContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<IdentityService>();

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("rabbitmq://chimpanzee.rmq.cloudamqp.com/fzrrkjya", h =>
                    {
                        h.Username("fzrrkjya");
                        h.Password("4hKfmc3shjyOXU4Jr_ptSem0hh5LfP-0");
                    });
                });
            });

        }
    }
}
