using System;

namespace Cas.Core.Events
{
    public abstract class EventBase : IEvent
    {
        public Guid LocationId { get; private set; }

        public Guid AgentId { get; private set; }

        public int Generation { get; private set; }

        protected EventBase(Guid agentId, Guid locationId,int generation) 
        {
            if (generation < 0) throw new ArgumentOutOfRangeException("generation");

            this.AgentId = agentId;
            this.LocationId = locationId;
            this.Generation = generation;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1} at location {2}", this.Generation, this.GetType().Name, this.LocationId);
        }
    }
}
