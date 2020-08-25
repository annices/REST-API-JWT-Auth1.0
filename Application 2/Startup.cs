using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using App2.Models;
using Polly;

namespace App2
{
    /// <summary>
    /// The purpose with the startup class is to configure the settings we want to apply for our application.
    /// </summary>
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Get our defined API client name from appsetting.json file and register it for our API calls:
            services.AddHttpClient(Configuration[new APIClient().Name], client =>
            {
                client.BaseAddress = new Uri(Configuration[new APIClient().BaseURL]); // Base URL to application 1.
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            }).SetHandlerLifetime(TimeSpan.FromSeconds(5))

            // Register a re-try API call policy. Re-try three times in 1, 3 and 5 seconds intervals:
            .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new[] {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10)
            }));

            // Register support for JSON Web Tokens:
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration[new Token().Issuer],
                    ValidAudience = Configuration[new Token().Audience],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration[new Token().Key]))
                };
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        public void Configure(IApplicationBuilder app)
        {
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "Default",
                    pattern: "{controller=API}/{action=Index}"); // Set the start action for this application.
            });
        }

    } // End class.
} // End namespace.
