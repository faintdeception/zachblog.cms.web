using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;
using System.Threading.Tasks;
using zachblog.cms.starfleet.Models;
using zachblog.cms.starfleet.Settings;
using zachblog.cms.starfleet.ViewModels;

namespace zachblog.cms.starfleet.Drivers
{
    public class StarfleetPartDisplayDriver : ContentPartDisplayDriver<StarfleetPart>
    {
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public StarfleetPartDisplayDriver(IContentDefinitionManager contentDefinitionManager)
        {
            _contentDefinitionManager = contentDefinitionManager;
        }

        public override IDisplayResult Display(StarfleetPart part, BuildPartDisplayContext context)
        {
            return Initialize<StarfleetPartViewModel>(GetDisplayShapeType(context), m => BuildViewModel(m, part, context))
                .Location("Detail", "Content:10")
                .Location("Summary", "Content:10")
                ;
        }

        public override IDisplayResult Edit(StarfleetPart part, BuildPartEditorContext context)
        {
            return Initialize<StarfleetPartViewModel>(GetEditorShapeType(context), model =>
            {
                model.Show = part.Show;
                model.ContentItem = part.ContentItem;
                model.StarfleetPart = part;
            });
        }

        public override async Task<IDisplayResult> UpdateAsync(StarfleetPart model, IUpdateModel updater)
        {
            await updater.TryUpdateModelAsync(model, Prefix, t => t.Show);

            return Edit(model);
        }

        private Task BuildViewModel(StarfleetPartViewModel model, StarfleetPart part, BuildPartDisplayContext context)
        {
            var settings = context.TypePartDefinition.GetSettings<StarfleetPartSettings>();

            model.ContentItem = part.ContentItem;
            model.MySetting = settings.MySetting;
            model.Show = part.Show;
            model.StarfleetPart = part;
            model.Settings = settings;

            return Task.CompletedTask;
        }
    }
}
