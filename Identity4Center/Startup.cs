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

              //身份信息授权资源
              //.AddInMemoryIdentityResources(Config.GetIdentityResources())
              ////API访问授权资源
              //.AddInMemoryApiResources(Config.GetApiResources())
              ////添加客户端
              //.AddInMemoryClients(Config.GetClients())
              // //添加用户
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
            //访问wwwroot目录静态文件
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
    /// 实现动态路由
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
            
//读数据
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
