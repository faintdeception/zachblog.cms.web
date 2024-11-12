using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OrchardCore.ContentManagement;
using OrchardCore.DisplayManagement.Notify;
using System.Linq;
using System.Threading.Tasks;

namespace StarfleetModule.Controllers
{
    public class CrewController : Controller
    {
        private readonly INotifier _notifier;
        private readonly ILogger _logger;
        private readonly IStringLocalizer T;
        private readonly IHtmlLocalizer H;


        public CrewController(
            INotifier notifier,
            IStringLocalizer<CrewController> localizer,
            IHtmlLocalizer<CrewController> htmlLocalizer,
            ILogger<CrewController> logger)
        {
            _logger = logger;
            _notifier = notifier;
            T = localizer;
            H = htmlLocalizer;

        }

        public ActionResult Index()
        {

            ViewData["Message"] = T["Hello World!"];

            return View();
        }

        [Route("StarfleetModule/NotifyMe")]
        public async Task<IActionResult> NotifyMe()
        {
            _logger.LogError("NotifyMe was called");
            await _notifier.InformationAsync(H["Congratulations! You have been notified! Check the error log too!"]);

            return View();
        }
    }
}
