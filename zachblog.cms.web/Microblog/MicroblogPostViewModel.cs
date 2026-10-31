using System;
using System.Collections.Generic;

namespace zachblog.cms.web.Microblog
{
    public class MicroblogPostViewModel
    {
        public string ContentItemId { get; set; }
        public string Body { get; set; }
        public DateTime CreatedUtc { get; set; }
        public List<string> ImageUrls { get; set; } = new();

        // MVP: only the first image is displayed.
        public string FirstImageUrl => ImageUrls.Count > 0 ? ImageUrls[0] : null;
    }

    public class MicroblogIndexViewModel
    {
        public List<MicroblogPostViewModel> Posts { get; set; } = new();
        public bool CanPublish { get; set; }
        public int SoftCharacterLimit { get; set; } = MicroblogConstants.SoftCharacterLimit;
    }
}
