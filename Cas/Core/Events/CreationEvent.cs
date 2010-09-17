using System;

namespace Cas.Core.Events
{
    public class CreationEvent : EventBase
    {
        public CreationEvent(Guid agentId, Guid locationId, int generation) 
            : base(agentId, locationId, "Created", generation)  {  }
    }
}
