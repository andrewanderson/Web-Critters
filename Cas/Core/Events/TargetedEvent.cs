using System;

namespace Cas.Core.Events
{
    public class TargetedEvent : EventBase
    {
        public Guid TargetId { get; private set; }

        public Type TargetType { get; private set; }

        public string Result { get; private set; }

        public TargetedEvent(Guid agentId, Guid locationId, string eventType, int generation, Guid targetId, Type targetType, string result) 
            : base(agentId, locationId, eventType, generation)
        {
            if (targetType == null) throw new ArgumentNullException("targetType");

            this.TargetId = targetId;
            this.TargetType = targetType;
            this.Result = result;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1} ({4}) on ({3}) {2}", this.Generation, EventType, TargetId, TargetType.Name, Result);
        }
    }
}
