using System.Collections.Generic;
using System.Threading.Tasks;
using OrchardCore.Security.Permissions;

namespace zachblog.cms.web.Microblog
{
    public class MicroblogPermissions : IPermissionProvider
    {
        public static readonly Permission PublishMicroblog = new(
            nameof(PublishMicroblog), "Publish microblog posts from /tweets");

        public Task<IEnumerable<Permission>> GetPermissionsAsync() =>
            Task.FromResult<IEnumerable<Permission>>(new[] { PublishMicroblog });

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes() => new[]
        {
            new PermissionStereotype
            {
                Name = "Administrator",
                Permissions = new[] { PublishMicroblog }
            }
        };
    }
}
