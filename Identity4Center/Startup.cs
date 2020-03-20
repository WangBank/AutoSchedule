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
              //�����Ϣ��Ȩ��Դ
              .AddInMemoryIdentityResources(Config.GetIdentityResources())
              //API������Ȩ��Դ
              .AddInMemoryApiResources(Config.GetApiResources())
              //��ӿͻ���
              .AddInMemoryClients(Config.GetClients())
               //����û�
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
            //����wwwrootĿ¼��̬�ļ�
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
