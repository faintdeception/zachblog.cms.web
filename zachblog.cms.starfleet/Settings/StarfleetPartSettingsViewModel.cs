using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace zachblog.cms.starfleet.Settings
{
    public class StarfleetPartSettingsViewModel
    {
        public string MySetting { get; set; }

        [BindNever]
        public StarfleetPartSettings StarfleetPartSettings { get; set; }
    }
}
