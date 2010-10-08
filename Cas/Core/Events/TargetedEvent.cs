using System;
using Cas.Core.Interfaces;

namespace Cas.Core.Events
{
    public class TargetedEvent : EventBase
    {
        public IIsUnique Target { get; private set; }

        public int Result { get; private set; }

        public TargetedEvent(ILocation location, int generation, IIsUnique target, int result) 
            : base(location, generation)
        {
            if (target == null) throw new ArgumentNullException("target");

            this.Target = target;
            this.Result = result;
        }

        public override string ToString()
        {
            return string.Format("{0}: Won encounter ({3}) against ({2}) {1}", this.Generation, Target.ToShortString(), Target.GetType().Name, Result);
        }
    }
}
