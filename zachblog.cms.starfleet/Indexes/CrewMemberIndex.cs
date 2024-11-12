using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YesSql.Indexes;
using zachblog.cms.starfleet.Models;

namespace zachblog.cms.starfleet.Indexes
{
    // The BookIndex objects will be stored in the database. Since this is actually a relational database these will be
    // records in a table specifically created for this index.
    public class CrewMemberIndex : MapIndex
    {
        public string Name { get; set; }
        public string Rank { get; set; }
        public string Species { get; set; }
        public string Position { get; set; }
        public string Bio { get; set; }
    }

    // These IndexProvider services will provide the mappings between the objects stored in the document database and the
    // index objects stored in records. When a Book object is being saved by the ISession service an index record will also
    // be stored in the related index table. Don't forget to register this class with the service provider (see:
    // Startup.cs).

    // Note that this IndexProvider is registered as a singleton which is the good choice usually. However, if you want to
    // inject other services into it (like any of the basic Orchard services you've seen until now) then you need to
    // register it with AddScoped<IScopedIndexProvider, BookIndexProvider>() and make the class also implement
    // IScopedIndexProvider.
    public class CrewMemberIndexProvider : IndexProvider<StarfleetCrewMember>
    {
        public override void Describe(DescribeContext<StarfleetCrewMember> context) =>
            context.For<CrewMemberIndex>()
                .Map(crewMember =>
                    new CrewMemberIndex
                    {
                        Name = crewMember.Name,
                        Rank = crewMember.Rank,
                        Species = crewMember.Species,
                        Position = crewMember.Position,
                        Bio = crewMember.Bio
                    });
    }

    // NEXT STATION: Migrations/BookMigrations.
}
