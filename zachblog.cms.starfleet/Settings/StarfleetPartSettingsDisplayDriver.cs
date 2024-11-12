using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.ContentTypes.Editors;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;
using System;
using System.Threading.Tasks;
using zachblog.cms.starfleet.Models;

namespace zachblog.cms.starfleet.Settings
{
    public class StarfleetPartSettingsDisplayDriver : ContentTypePartDefinitionDisplayDriver
    {
        public override Task<IDisplayResult> EditAsync(ContentTypePartDefinition contentTypePartDefinition, BuildEditorContext context)
        {
            if (!String.Equals(nameof(StarfleetPart), contentTypePartDefinition.PartDefinition.Name))
            {
                return Task.FromResult<IDisplayResult>(null);
            }

            return Task.FromResult<IDisplayResult>(
                Initialize<StarfleetPartSettingsViewModel>("StarfleetPartSettings_Edit", model =>
                {
                    var settings = contentTypePartDefinition.GetSettings<StarfleetPartSettings>();

                    model.MySetting = settings.MySetting;
                    model.StarfleetPartSettings = settings;
                }).Location("Content")
            );
        }

        public override async Task<IDisplayResult> UpdateAsync(ContentTypePartDefinition contentTypePartDefinition, UpdateTypePartEditorContext context)
        {
            if (!String.Equals(nameof(StarfleetPart), contentTypePartDefinition.PartDefinition.Name))
            {
                return null;
            }

            var model = new StarfleetPartSettingsViewModel();

            if (await context.Updater.TryUpdateModelAsync(model, Prefix, m => m.MySetting))
            {
                context.Builder.WithSettings(new StarfleetPartSettings { MySetting = model.MySetting });
            }

            return Edit(contentTypePartDefinition, context);
        }
    }
}
