using Microsoft.AspNetCore.Mvc;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zachblog.cms.starfleet.Models;

namespace zachblog.cms.starfleet.Controllers
{
    internal class DisplayManagementController : Controller, IUpdateModel
    {
        // The core display management features can be used via the IDisplayManager service. The generic parameter will be
        // the object that needs to be displayed on the UI somehow. Don't forget to register this generic class with the
        // service provider (see: Startup.cs).
        private readonly IDisplayManager<StarfleetCrewMember> _bookDisplayManager;

        public DisplayManagementController(IDisplayManager<StarfleetCrewMember> bookDisplayManager) => _bookDisplayManager = bookDisplayManager;

        // Before we learn how shapes are generated using the display manager let's see what are these shapes actually.
        // Ad-hoc shapes can be created anywhere without the display manager. In this example we'll see how to create an
        // ad-hoc shape inside a view (or could be another shape). Later we'll see how to do it from a filter too. Open from
        // under /Lombiq.TrainingDemo/DisplayManagement/AdHocShape.
        public ActionResult AdHocShape() => View();

        // NEXT STATION: Views/DisplayManagement/AdHocShape.cshtml

        // First, create a page that will display a summary and some additional data of the book. See it under
        // /Lombiq.TrainingDemo/DisplayManagement/DisplayBook.
        public async Task<IActionResult> DisplayBook()
        {
            // For demonstration purposes create a dummy book object.
            var book = CreateDemoCrew();

            // This method will generate a shape primarily for displaying information about the given object.
            var shape = await _bookDisplayManager.BuildDisplayAsync(book, this);

            // We will see how this display shape is generated and what will contain but first let's see how is this
            // rendered in the MVC view. NEXT STATION: Go to Views/DisplayManagement/DisplayBook.cshtml.

            return View(shape);
        }

        // Let's generate another Book display shape, but now with a display type. See it under
        // /Lombiq.TrainingDemo/DisplayManagement/DisplayBookDescription.
        public async Task<IActionResult> DisplayBookDescription()
        {
            // Generate another book object to be used for demonstration purposes.
            var book = CreateDemoCrew();

            // This time give an additional parameter which is the display type. If display type is given then Orchard Core
            // will search a cshtml file with a name [ClassName].[DisplayType].cshtml.
            var shape = await _bookDisplayManager.BuildDisplayAsync(book, this, "Description");

            // NEXT STATION: Go to Views/Book.Description.cshtml

            return View(shape);
        }

        private static StarfleetCrewMember CreateDemoCrew() =>
            new()
            {
                Name = "Jean-Luc Picard",
                Rank = "Captain",
                Species = "Human",
                Position = "Commanding Officer",
                Bio = "Captain Jean-Luc Picard is a fictional character in the Star Trek franchise, most often seen as the Captain of the starship USS Enterprise (NCC-1701-D)."
            };
    }

    // If you've finished with both actions (and their .cshtml files as well), then you can move forward to the next
    // training.

    // END OF TRAINING SECTION: Display management

    // NEXT STATION: Controllers/DatabaseStorageController
}