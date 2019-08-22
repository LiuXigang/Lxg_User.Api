using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Reslience;
using User.Identity.Authentication;
using User.Identity.Infrastructure;
using User.Identity.Services;

namespace User.Identity
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentityServer()
                .AddExtensionGrantValidator<SmsAuthCodeValidator>()
                .AddDeveloperSigningCredential()
                .AddInMemoryClients(IdentityConfig.GetClient())
                .AddInMemoryIdentityResources(IdentityConfig.GetIdentityResources())
                .AddInMemoryApiResources(IdentityConfig.GetResources());

            services.AddSingleton(new HttpClient());
            services.AddScoped<IAuthCodeService, AuthCodeService>()
                .AddScoped<IUserService, UserService>();

            services.AddSingleton(typeof(ResilienceClientFactory), sp =>
            {
                var logger = sp.GetRequiredService<ILogger<ResilienceClientFactory>>();
                var httpContextAccessor= sp.GetRequiredService<IHttpContextAccessor>();
                return new ResilienceClientFactory(logger, httpContextAccessor, 5, 5);
            });
            services.AddSingleton<IHttpClient>(sp =>
            {
                return sp.GetRequiredService<ResilienceClientFactory>().GetResilienceHttpClient();

            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();
            app.UseMvc();
        }
    }
}
