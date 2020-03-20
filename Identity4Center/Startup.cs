using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity4Center.BaseConfig;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Identity4Center
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var builder = services.AddIdentityServer()
              .AddDeveloperSigningCredential()
              //身份信息授权资源
              .AddInMemoryIdentityResources(Config.GetIdentityResources())
              //API访问授权资源
              .AddInMemoryApiResources(Config.GetApiResources())
              //添加客户端
              .AddInMemoryClients(Config.GetClients())
               //添加用户
              .AddTestUsers(Config.GetUsers());
           
            services.AddControllers();
            services.AddMvc(s=> {
                s.EnableEndpointRouting = false;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();
            app.UseIdentityServer();
            //访问wwwroot目录静态文件
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
