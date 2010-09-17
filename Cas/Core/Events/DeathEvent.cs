using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cas.Core.Events
{
    public class DeathEvent : EventBase
    {
        public DeathEvent(Guid agentId, Guid locationId, int generation)
            : base(agentId, locationId, "Died", generation) { }
    }
}
