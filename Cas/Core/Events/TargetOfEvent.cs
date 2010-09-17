using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cas.Core.Events
{
    public class TargetOfEvent : EventBase
    {
        public Guid ActorId { get; private set; }

        public Type ActorType { get; private set; }

        public string Result { get; private set; }

        public TargetOfEvent(Guid agentId, Guid locationId, string eventType, int generation, Guid actorId, Type actorType, string result)
            : base(agentId, locationId, eventType, generation)
        {
            if (actorType == null) throw new ArgumentNullException("actorType");

            this.ActorId = actorId;
            this.ActorType = actorType;
            this.Result = result;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1} {4} by ({3}) {2}", this.Generation, EventType, ActorId, ActorType.Name, Result);
        }
    }
}
