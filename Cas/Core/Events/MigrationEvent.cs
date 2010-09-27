using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cas.Core.Events
{
    public class MigrationEvent : PayUpkeepEvent
    {
        public Guid OriginLocationId { get; set; }

        public MigrationEvent(Guid originLocationId, Guid destinationLocationId, int cost, int generation)
            : base(destinationLocationId, cost, generation)
        {
            this.OriginLocationId = originLocationId;
        }

        public override string ToString()
        {
            return string.Format("{0}: Migrated at a cost of {1} from location {2} to {3}", this.Generation, this.Cost, this.OriginLocationId, this.LocationId);
        }
    }
}
