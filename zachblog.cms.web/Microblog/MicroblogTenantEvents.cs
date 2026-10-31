using System.Threading.Tasks;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Media.Fields;
using OrchardCore.Modules;

namespace zachblog.cms.web.Microblog
{
    /// <summary>
    /// Idempotently ensures the "Microblog" content type exists for the tenant.
    /// </summary>
    public class MicroblogTenantEvents : ModularTenantEvents
    {
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public MicroblogTenantEvents(IContentDefinitionManager contentDefinitionManager)
        {
            _contentDefinitionManager = contentDefinitionManager;
        }

        public override async Task ActivatedAsync()
        {
            if (await _contentDefinitionManager.GetTypeDefinitionAsync(MicroblogConstants.ContentType) != null)
            {
                return;
            }

            await _contentDefinitionManager.AlterPartDefinitionAsync(MicroblogConstants.ContentType, part => part
                .WithField(MicroblogConstants.BodyField, field => field
                    .OfType(nameof(TextField))
                    .WithDisplayName("Body"))
                .WithField(MicroblogConstants.ImagesField, field => field
                    .OfType(nameof(MediaField))
                    .WithDisplayName("Images")));

            await _contentDefinitionManager.AlterTypeDefinitionAsync(MicroblogConstants.ContentType, type => type
                .DisplayedAs("Microblog Post")
                .Creatable()
                .Listable()
                .WithPart(MicroblogConstants.ContentType));
        }
    }
}
