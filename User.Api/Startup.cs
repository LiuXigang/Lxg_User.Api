using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using User.Api.Data;
using User.Api.Filters;

namespace User.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<UserContext>(options =>
            {
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddMvc(options =>
            {
                options.Filters.Add<HttpGlobalExceptionFilter>();//自定义全局异常过滤器
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            #region CAP
            services.AddCap(options =>
            {
                options
                    .UseEntityFramework<UserContext>()
                    .UseRabbitMQ("localhost")
                    .UseDashboard();
                //options.UseDiscovery(d =>
                //{
                //    d.DiscoveryServerHostName = "localhost";
                //    d.DiscoveryServerPort = 8500;
                //    d.CurrentNodeHostName = "localhost";
                //    d.CurrentNodePort = 56688;
                //    d.NodeId = "123";
                //    d.NodeName = "CAP UserAPI Node";
                //});
            });
            #endregion
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //app.UseCap();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
