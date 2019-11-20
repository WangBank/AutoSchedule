using AutoSchedule.Common;
using AutoSchedule.Dtos.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

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

            //自定义注册服务
            services.ConfigServies();

            //自定义服务获取类
            GetServiceByOther(services);

            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            //getService(services);
            //.AddRazorRuntimeCompilation();
        }

        private void GetServiceByOther(IServiceCollection services)
        {
            GetContext.ServiceProvider = services.BuildServiceProvider();
        }

        private void getService(IServiceCollection services)
        {
            var ss = services.BuildServiceProvider();
            var context = (SqlLiteContext)ss.GetService(typeof(SqlLiteContext));
            var ssss = context.OpenSql.ToListAsync().Result;
            Console.WriteLine("hhh");
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