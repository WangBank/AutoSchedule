using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
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
            
           // string SqlLiteConn = RuntimeInformation.IsOSPlatform(OSPlatform.Linux)? Configuration.GetConnectionString("SqlLiteLinux") : Configuration.GetConnectionString("SqlLiteWin");
            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;database=IdentityServer4.Quickstart.EntityFramework-3.0.0;trusted_connection=yes;";

            var builder = services.AddIdentityServer()
              .AddDeveloperSigningCredential()

              //�����Ϣ��Ȩ��Դ
              //.AddInMemoryIdentityResources(Config.GetIdentityResources())
              ////API������Ȩ��Դ
              //.AddInMemoryApiResources(Config.GetApiResources())
              ////��ӿͻ���
              //.AddInMemoryClients(Config.GetClients())
              // //����û�
              //.AddTestUsers(Config.GetUsers())
              ;
             
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
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //    //foreach (var item in SystemData.GetApiInfos())
            //    //{
            //    //    endpoints.MapDynamicControllerRoute<OpenApiTransformer>(item.Url);
            //    //}
            //    endpoints.MapDynamicControllerRoute<OpenApiTransformer>("{**slug}");
            //    //{**slug}
            //});
        }
    }
    /// <summary>
    /// ʵ�ֶ�̬·��
    /// </summary>
    public class OpenApiTransformer : DynamicRouteValueTransformer
    {
        public OpenApiTransformer()
        {

        }

        public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext
        , RouteValueDictionary values)
        {

            RouteValueDictionary keyValuePairs = new RouteValueDictionary();
            
//������
            if ("" == null)
            {
                return await Task.FromResult(values);
            }
            keyValuePairs.Add("controller", "Controller name");
            keyValuePairs.Add("action","ActionName");
            keyValuePairs.Add("namespace", "ApiTest");
            return await Task.FromResult(keyValuePairs);
        }
    }

}
