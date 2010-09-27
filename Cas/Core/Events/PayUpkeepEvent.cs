using System;

namespace Cas.Core.Events
{
    public class PayUpkeepEvent : EventBase
    {
        public int Cost { get; private set; }

        public PayUpkeepEvent(Guid locationId, int cost, int generation)
            : base(locationId, generation)
        {
            this.Cost = cost;
        }

        public override string ToString()
        {
            return string.Format("{0}: Paid upkeep of {1} at location {2}", this.Generation, this.Cost, this.LocationId);
        }
    }
}
