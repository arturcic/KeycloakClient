using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace KeycloakClient.Extensions
{
    public static class KeycloakServiceCollectionExtensions
    {
        public static void AddKeycloakAdminClient(this IServiceCollection services, IConfiguration configuration, string sectionKey = nameof(KeycloakAdminClientOptions))
        {
            var section = configuration.GetSection(sectionKey);
            services.Configure<KeycloakAdminClientOptions>(section);

            AddKeycloakAdminServices(services);
        }

        public static void AddKeycloakAdminClient(this IServiceCollection services, Action<KeycloakAdminClientOptions> setupAction)
        {
            services.Configure(setupAction);
            AddKeycloakAdminServices(services);
        }

        private static void AddKeycloakAdminServices(IServiceCollection services)
        {
            services.AddTransient<KeycloakRefreshTokenDelegatingHandler>();

            services.AddHttpClient<IKeycloakClient, KeycloakClient>()
                .AddHttpMessageHandler<KeycloakRefreshTokenDelegatingHandler>();
        }
        
        public static void AddKeycloakAuth(this IServiceCollection services, IConfiguration configuration, string sectionKey = nameof(KeycloakClientOptions))
        {
            var section = configuration.GetSection(sectionKey);
            services.Configure<KeycloakClientOptions>(section);

            AddKeycloakAuthServices(services);
        }

        public static void AddKeycloakAuth(this IServiceCollection services, Action<KeycloakClientOptions> setupAction)
        {
            services.Configure(setupAction);
            AddKeycloakAuthServices(services);
        }

        private static void AddKeycloakAuthServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var env = serviceProvider.GetService<IHostingEnvironment>();
                var clientOptions = serviceProvider.GetService<IOptions<KeycloakClientOptions>>().Value;

                options.Authority = new Uri(clientOptions.Url).Combine("realms").Combine(clientOptions.Realm).ToString();
                options.Audience = clientOptions.ClientId;

                if (env.IsDevelopment())
                {
                    options.RequireHttpsMetadata = false;
                }

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = c =>
                    {
                        c.NoResult();

                        c.Response.StatusCode = 500;
                        c.Response.ContentType = "text/plain";
                        return c.Response.WriteAsync(env.IsDevelopment()
                            ? c.Exception.ToString()
                            : "An error occured processing your authentication.");
                    }
                };
            });
        }
    }
}
