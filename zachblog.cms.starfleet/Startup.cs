using Fluid;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.ContentTypes.Editors;
using OrchardCore.Data.Migration;
using OrchardCore.Modules;
using System;
using zachblog.cms.starfleet.Drivers;
using zachblog.cms.starfleet.Handlers;
using zachblog.cms.starfleet.Models;
using zachblog.cms.starfleet.Settings;
using zachblog.cms.starfleet.ViewModels;

namespace zachblog.cms.starfleet
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.Configure<TemplateOptions>(o =>
            {
                o.MemberAccessStrategy.Register<StarfleetPartViewModel>();
            });

            services.AddContentPart<StarfleetPart>()
                .UseDisplayDriver<StarfleetPartDisplayDriver>()
                .AddHandler<StarfleetPartHandler>();

            services.AddScoped<IContentTypePartDefinitionDisplayDriver, StarfleetPartSettingsDisplayDriver>();
            services.AddScoped<IDataMigration, Migrations>();
        }

        public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            routes.MapAreaControllerRoute(
                name: "Home",
                areaName: "zachblog.cms.starfleet",
                pattern: "Home/Index",
                defaults: new { controller = "Home", action = "Index" }
            );
        }
    }
}
