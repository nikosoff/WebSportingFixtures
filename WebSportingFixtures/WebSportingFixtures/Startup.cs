using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using WebSportingFixtures.Core.Interfaces;
using WebSportingFixtures.DependencyInjection;
using WebSportingFixtures.Services;

namespace WebSportingFixtures
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IStore>(_ => DependencyInjectionContainer.GetStore());
            services.AddScoped<IRawEventProvider>(_ => DependencyInjectionContainer.GetRawEventProvider());
            services.AddScoped<ITextSimilarityAlgorithm>(_ => DependencyInjectionContainer.GetTextSimilarityAlgorithm());
            services.AddScoped<SportingFixturesService>();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMvc(routes => routes.MapRoute("Default", "{controller=Home}/{action=Index}/{id?}"));

        }
    }
}
