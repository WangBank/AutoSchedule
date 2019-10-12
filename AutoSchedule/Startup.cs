using AutoSchedule.Common;
using AutoSchedule.Dtos.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

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
            services.AddSingleton<QuartzStartup>();
            services.AddTransient<HelloJob>();
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();//×¢²áISchedulerFactoryµÄÊµÀý¡£

            services.AddSingleton<IJobFactory, IOCJobFactory>();
            //.AddRazorRuntimeCompilation();
            services.AddDbContext<SqlLiteContext>(options => options.UseSqlite(Configuration.GetConnectionString("SqlLite")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
