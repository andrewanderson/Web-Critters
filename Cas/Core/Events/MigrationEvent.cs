using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cas.Core.Interfaces;

namespace Cas.Core.Events
{
    public class MigrationEvent : PayUpkeepEvent
    {
        public ILocation OriginLocation { get; private set; }

        public MigrationEvent(ILocation originLocation, ILocation destinationLocation, int cost, int generation)
            : base(destinationLocation, cost, generation)
        {
            if (originLocation == null) throw new ArgumentNullException("originLocation");

            this.OriginLocation = originLocation;
        }

        public override string ToString()
        {
            return string.Format("{0}: Migrated at a cost of {1} from {2} to {3}", this.Generation, this.Cost, this.OriginLocation.ToShortString(), this.Location.ToShortString());
        }
    }
}
