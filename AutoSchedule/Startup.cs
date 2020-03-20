using AutoSchedule.Common;
using AutoSchedule.Dtos.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NewLife.Caching;
using System;
using System.Diagnostics;
using System.Text;

namespace AutoSchedule
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
            services.AddDbContext<SqlLiteContext>(options => options.UseSqlite(Configuration.GetConnectionString("SqlLite")), ServiceLifetime.Transient);
            //services.AddDbContext<SqlLiteContext>(options => options.UseSqlite(Configuration.GetConnectionString("SqlLite")));
            services.AddHttpClient();
            //�Զ���ע�����
            services.AddExtendService(configure =>
            {
                configure.UseAutoTaskJob();
                configure.UseIOCJobFactory();
                configure.UseISchedulerFactory();
                configure.UseQuartzStartup();
            });
            //�Զ�������ȡ��
            GetServiceByOther(services);
            //Microsoft.VisualStudio.Web.CodeGeneration.Design
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            //getService(services);
            //.AddRazorRuntimeCompilation();
        }

        private void GetServiceByOther(IServiceCollection services)
        {
            GetContext.ServiceProvider = services.BuildServiceProvider();
        }

        private void getService()
        {
            //������ʱ�����Redis
            var ss = GetContext.ServiceProvider;
            var quartzStartup = (QuartzStartup)ss.GetService(typeof(QuartzStartup));
            quartzStartup.rds.Clear();
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
            //lifetime.ApplicationStarted.Register(UnRegService1);
            //lifetime.ApplicationStopping.Register(UnRegService);//Ӧ��ֹͣ��ӷ�������ע��
            //app.Run(r => r.Response.WriteAsync("û�뵽��",Encoding.GetEncoding("utf-8")));

            //�������ƻ�Redis����
            getService();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            
        }

        private void UnRegService()
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.FileName = "notepad.exe";
            Process.Start(processInfo);
        }

        private void UnRegService1()
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.FileName = "notepad.exe";
            Process.Start(processInfo);
        }
    }
}