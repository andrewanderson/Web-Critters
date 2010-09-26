using System;

namespace Cas.Core.Events
{
    public class CreationEvent : EventBase
    {
        public CreationEvent(Guid agentId, Guid locationId, int generation) 
            : base(agentId, locationId, generation)  {  }

        public override string ToString()
        {
            return string.Format("{0}: Created at location {1}", this.Generation, this.LocationId);
        }
    }
}
