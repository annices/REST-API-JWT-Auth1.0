using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MVCApp1.Models;
using Polly;

namespace MVCApp1
{
    /// <summary>
    /// The purpose with the startup class is to set up the configuration we want to apply for our application.
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
            services.AddControllersWithViews().AddSessionStateTempDataProvider();

            services.AddSession(options =>
            {
                // Set session cookie timeout:
                options.IdleTimeout = TimeSpan.FromSeconds(1800);
                options.Cookie.HttpOnly = true;
                // Make the session cookie essential:
                options.Cookie.IsEssential = true;
            });

            // Get our defined API client name from appsetting.json file and register it for our API calls:
            services.AddHttpClient(Configuration[new APIClient().Name], client =>
            {
                client.BaseAddress = new Uri(Configuration[new APIClient().BaseURL]);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            }).SetHandlerLifetime(TimeSpan.FromSeconds(5))

            // Register a re-try API call policy. Re-try three times in 1, 3 and 5 seconds intervals:
            .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new[] {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10)
            }));
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        public void Configure(IApplicationBuilder app)
        {
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "Default",
                    pattern: "{controller=Home}/{action=Index}"); // Set the start page for our application.
                endpoints.MapControllerRoute(
                    name: "Update",
                    pattern: "{controller=Home}/{action=Update}/{email}");
            });
        }
    }
}
