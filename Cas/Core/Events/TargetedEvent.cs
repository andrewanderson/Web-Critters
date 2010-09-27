using System;

namespace Cas.Core.Events
{
    public class TargetedEvent : EventBase
    {
        public Guid TargetId { get; private set; }

        public Type TargetType { get; private set; }

        public string Result { get; private set; }

        public TargetedEvent(Guid locationId, int generation, Guid targetId, Type targetType, string result) 
            : base(locationId, generation)
        {
            if (targetType == null) throw new ArgumentNullException("targetType");

            this.TargetId = targetId;
            this.TargetType = targetType;
            this.Result = result;
        }

        public override string ToString()
        {
            return string.Format("{0}: Won encounter ({3}) against ({2}) {1}", this.Generation, TargetId, TargetType.Name, Result);
        }
    }
}
