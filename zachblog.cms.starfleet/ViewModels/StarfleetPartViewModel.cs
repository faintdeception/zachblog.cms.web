using Microsoft.AspNetCore.Mvc.ModelBinding;
using OrchardCore.ContentManagement;
using zachblog.cms.starfleet.Models;
using zachblog.cms.starfleet.Settings;

namespace zachblog.cms.starfleet.ViewModels
{
    public class StarfleetPartViewModel
    {
        public string MySetting { get; set; }

        public bool Show { get; set; }

        [BindNever]
        public ContentItem ContentItem { get; set; }

        [BindNever]
        public StarfleetPart StarfleetPart { get; set; }

        [BindNever]
        public StarfleetPartSettings Settings { get; set; }
    }
}
