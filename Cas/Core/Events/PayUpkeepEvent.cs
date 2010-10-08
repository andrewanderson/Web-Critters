using System;
using Cas.Core.Interfaces;

namespace Cas.Core.Events
{
    public class PayUpkeepEvent : EventBase
    {
        public int Cost { get; private set; }

        public PayUpkeepEvent(ILocation location, int cost, int generation)
            : base(location, generation)
        {
            this.Cost = cost;
        }

        public override string ToString()
        {
            return string.Format("{0}: Paid upkeep of {1} at {2}", this.Generation, this.Cost, this.Location.ToShortString());
        }
    }
}
