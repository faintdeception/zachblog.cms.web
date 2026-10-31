using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrchardCore.Modules;
using OrchardCore.Security.Permissions;
using zachblog.cms.web.Microblog;

namespace zachblog.cms.web
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOrchardCms(builder => builder.ConfigureServices(tenant =>
            {
                tenant.AddScoped<IPermissionProvider, MicroblogPermissions>();
                tenant.AddScoped<IModularTenantEvents, MicroblogTenantEvents>();
            }));
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            
            app.UseOrchardCore();
        }
    }
}
