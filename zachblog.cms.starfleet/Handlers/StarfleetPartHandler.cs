using OrchardCore.ContentManagement.Handlers;
using System.Threading.Tasks;
using zachblog.cms.starfleet.Models;

namespace zachblog.cms.starfleet.Handlers
{
    public class StarfleetPartHandler : ContentPartHandler<StarfleetPart>
    {
        public override Task InitializingAsync(InitializingContentContext context, StarfleetPart part)
        {
            part.Show = true;

            return Task.CompletedTask;
        }
    }
}