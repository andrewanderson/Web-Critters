using System;

namespace Cas.Core.Events
{
    public class PayUpkeepEvent : EventBase
    {
        public int Cost { get; private set; }

        public PayUpkeepEvent(Guid agentId, Guid locationId, int cost, int generation)
            : this(agentId, locationId, cost, generation, "Paid Upkeep") { }

        protected PayUpkeepEvent(Guid agentId, Guid locationId, int cost, int generation, string description)
            : base(agentId, locationId, description, generation)
        {
            this.Cost = cost;
        }

        public override string ToString()
        {
            return string.Format("{0}: Paid upkeep of {1} at location {2}", this.Generation, this.Cost, this.LocationId);
        }
    }
}
