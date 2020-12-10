using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApplication1.Models;

namespace WebApplication1
{
    public class Startup
    {
        private IConfiguration _config;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddTransient<ITransient, Sample>();
            services.AddScoped<IScoped, Sample>();
            services.AddSingleton<ISingleton, Sample>();
            services.Configure<Config>(Configuration.GetSection("DemoUser"));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
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



        public interface ISample
        {
            int Id { get; }
        }

        public interface ITransient : ISample
        {
        }

        public interface IScoped : ISample
        {
        }

        public interface ISingleton : ISample
        {
        }

        public class Sample : ITransient, IScoped, ISingleton
        {
            private static int _counter;
            private int _id;

            public Sample()
            {
                _id = ++_counter;
            }

            public int Id => _id;
        }
    }
}
