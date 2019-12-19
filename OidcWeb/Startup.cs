using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace OidcWeb
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {

            // Add the configuration file appsettings.local.json, which is not included by default. That
            // configuration file is not included in the source code repository, but its contents is
            // documented in the appsettings.local.json.md file, which is located in the root folder
            // for this web application.
            this.Configuration = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.local.json", true)
                .Build();
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                    options.RequireAuthenticatedSignIn = true;
                })
                .AddOpenIdConnect(options =>
                {
                    this.Configuration.Bind("oidc", options);

                    options.TokenValidationParameters.NameClaimType = "name";
                    options.TokenValidationParameters.RoleClaimType = "role";

                    // Here you can set other options if you want to hard-code something instead of having it configurable.

                    options.Events.OnTicketReceived = async (context) =>
                    {
                        var user = context.Principal;
                        var identity = user.Identity as ClaimsIdentity;

                        // Here you can connect to other systems and augment the claims of the logged on user,
                        // if you want to add something that is not available in the AD tenant handling the authentication.

                        await Task.Yield();
                    };
                })
                .AddCookie();

            services.AddControllers();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}
