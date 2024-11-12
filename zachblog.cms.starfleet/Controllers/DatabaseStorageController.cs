
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Notify;
using System.Collections.Generic;
using System.Threading.Tasks;
using YesSql;
using zachblog.cms.starfleet.Indexes;
using zachblog.cms.starfleet.Models;

namespace zachblog.cms.starfleet.Controllers
{
    public class DatabaseStorageController : Controller
    {
        private readonly ISession _session;
        private readonly IDisplayManager<StarfleetCrewMember> _crewMemberDisplayManager;
        private readonly INotifier _notifier;
        private readonly IHtmlLocalizer H;
        private readonly IUpdateModelAccessor _updateModelAccessor;

        public DatabaseStorageController(
            ISession session,
            IDisplayManager<StarfleetCrewMember> bookDisplayManager,
            INotifier notifier,
            IHtmlLocalizer<DatabaseStorageController> htmlLocalizer,
            IUpdateModelAccessor updateModelAccessor)
        {
            _session = session;
            _crewMemberDisplayManager = bookDisplayManager;
            _notifier = notifier;
            _updateModelAccessor = updateModelAccessor;
            H = htmlLocalizer;
        }

        // A page with a button that will call the CreateBooks POST action. See it under
        // /Lombiq.TrainingDemo/DatabaseStorage/CreateBooks.
        [HttpGet]
        public IActionResult CreateBooks() => View();

        // Note the ValidateAntiForgeryToken attribute too: This validates the XSRF-prevention token automatically added in
        // the form (check for the input field named __RequestVerificationToken in the HTML output) of the CreateBooks view.
        [HttpPost, ActionName(nameof(CreateBooks)), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBooksPost()
        {
            // For demonstration purposes this will create 3 books and store them in the database one-by-one using the
            // ISession service. Note that you can even go to the database directly, circumventing YesSql too, by injecting
            // the IDbConnectionAccessor service and access the underlying connection.

            // Since storing them in the documents is not enough we need to index them to be able to filter them in a query.
            // NEXT STATION: Indexes/BookIndex.cs
            foreach (var book in CreateDemoStarfleetCrewMember())
            {
                // So now you understand what will happen in the background when this service is being called.

                await _session.SaveAsync(book);
            }

            await _notifier.InformationAsync(H["Books have been created in the database."]);

            return RedirectToAction(nameof(CreateBooks));
        }

        // This page will display the books written by J.K. Rosenzweig. See it under
        // /Lombiq.TrainingDemo/DatabaseStorage/JKRosenzweigBooks.
        public async Task<IActionResult> JKRosenzweigBooks()
        {
            // ISession service is used for querying items.
            var jkRosenzweigBooks = await _session
                // First, we define what object (document) we want to query and what index should be used for filtering.
                .Query<StarfleetCrewMember, CrewMemberIndex>()
                // In the .Where() method you can describe a lambda where the object will be the index object.
                .Where(index => index.Rank == "Captain")
                // When the query is built up you can call ListAsync() to execute it. This will return a list of books.
                .ListAsync();

            // Now this is what we possibly understand now, we will create a list of display shapes from the previously
            // fetched books. Note how we use the AwaitEachAsync() extension to run async operations sequentially. This is
            // important: You can't know if BuildDisplayAsync() is thread-safe so you shouldn't use e.g. Task.WhenAll().
            var bookShapes = await jkRosenzweigBooks.AwaitEachAsync(async book =>
                // We'll need to pass an IUpdateModel (used for model validation) to the method, which we can access via its
                // accessor service. Later you'll also see how we'll use this to run validations in drivers.
                await _crewMemberDisplayManager.BuildDisplayAsync(book, _updateModelAccessor.ModelUpdater));

            // You can check out Views/DatabaseStorage/JKRosenzweigBooks.cshtml and come back here.
            return View(bookShapes);
        }

        // END OF TRAINING SECTION: Storing data in document database and index records

        // NEXT STATION: Models/PersonPart.cs

        private static StarfleetCrewMember[] CreateDemoStarfleetCrewMember() =>
            [
            new StarfleetCrewMember
                {
                    Name = "Jean-Luc Picard",
                    Rank = "Captain",
                    Species = "Human",
                    Position = "Commanding Officer",
                    Bio = "Captain Jean-Luc Picard is a fictional character in the Star Trek franchise, most often seen as the Captain of the starship USS Enterprise (NCC-1701-D)."
                },
                new StarfleetCrewMember
                {
                    Name = "William T. Riker",
                    Rank = "Commander",
                    Species = "Human",
                    Position = "First Officer",
                    Bio = "William Thomas Riker is a fictional character in the Star Trek universe appearing primarily as a main character in Star Trek: The Next Generation."
                },
                new StarfleetCrewMember
                {
                    Name = "Data",
                    Rank = "Lieutenant Commander",
                    Species = "Android",
                    Position = "Operations Officer",
                    Bio = "Data is a character in the fictional Star Trek franchise. He appears in the television series Star Trek: The Next Generation and the feature films Star Trek Generations, Star Trek: First Contact, Star Trek: Insurrection, and Star Trek: Nemesis."
                }
        ];
    }
}
