using AutoSchedule.Common;
using AutoSchedule.Dtos.Data;
using AutoSchedule.Models;
using FreeSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NewLife.Caching;
using Newtonsoft.Json.Serialization;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace AutoSchedule
{
    public class Startup
    {
        public IFreeSql Fsql { get; }
        public Startup(IConfiguration configuration)
        {

            Configuration = configuration;
            string MessageDb = Configuration.GetConnectionString("MessageDb");
            Fsql = new FreeSqlBuilder()
                     .UseConnectionString(DataType.Oracle, MessageDb)
                     .UseAutoSyncStructure(false)
                     .Build();
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string SqlLiteConn = string.Empty;
          
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                SqlLiteConn =  Configuration.GetConnectionString("SqlLiteLinux");
            }
            else
            {
                SqlLiteConn = Configuration.GetConnectionString("SqlLiteWin");
            }
            services.AddDbContext<SqlLiteContext>(options => options.UseSqlite(SqlLiteConn), ServiceLifetime.Scoped);
            services.AddSingleton<IFreeSql>(Fsql);
            services.AddHttpClient();
            //自定义注册服务
            services.AddExtendService(configure =>
            {
                configure.UseAutoTaskJob();
                configure.UseIOCJobFactory();
                configure.UseISchedulerFactory();
                configure.UseQuartzStartup();
            });

            services.AddScoped<IScopeTest, Operation>();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            });

            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddMvc().AddNewtonsoftJson(s => s.SerializerSettings.ContractResolver = new DefaultContractResolver());
        }

        private void GetServiceByOther(IServiceCollection services)
        {
            GetContext.ServiceProvider = services.BuildServiceProvider();
        }

        private void getService()
        {
            //启动的时候清空Redis
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var quartzStartup = (QuartzStartup)GetContext.ServiceProvider.GetService(typeof(QuartzStartup));
                quartzStartup.rds.Clear();
            }
             
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            
            app.UseStaticFiles();
            lifetime.ApplicationStarted.Register(UnRegService);
            //app.Run(r => r.Response.WriteAsync("没想到吧",Encoding.GetEncoding("utf-8")));
            GetContext.ServiceProvider = app.ApplicationServices;
            getService();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            
        }
        public HttpContext requestServices ;
        private void  UnRegService()
        {
            Console.WriteLine("网站启动成功,请勿关闭此窗口!");
            Console.WriteLine($"请访问:localhost:{Configuration.GetSection("urls").Value.Split(':')[2]} 进行下一步配置!");
            Console.WriteLine($"祝您生活愉快");
        }

    }
}