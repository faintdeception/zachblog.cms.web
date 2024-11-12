using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;

namespace zachblog.cms.starfleet
{
    public class Migrations : DataMigration
    {
        IContentDefinitionManager _contentDefinitionManager;

        public Migrations(IContentDefinitionManager contentDefinitionManager)
        {
            _contentDefinitionManager = contentDefinitionManager;
        }

        public int Create()
        {
            // Define StarfleetMember content type
            _contentDefinitionManager.AlterTypeDefinitionAsync("StarfleetMember", type => type
                .Creatable()
                .Listable()
                .WithPart("TitlePart")
                .WithPart("RankPart")
                .WithPart("DivisionPart")
            );

            // Define RankPart
            _contentDefinitionManager.AlterPartDefinitionAsync("RankPart", part => part
                .WithField("Rank", field => field
                    .OfType("TextField")
                    .WithDisplayName("Rank")
                )
            );

            // Define DivisionPart
            _contentDefinitionManager.AlterPartDefinitionAsync("DivisionPart", part => part
                .WithField("Division", field => field
                    .OfType("TextField")
                    .WithDisplayName("Division")
                )
            );

            return 1;
        }
    }
}
