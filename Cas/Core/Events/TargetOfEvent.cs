using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cas.Core.Interfaces;

namespace Cas.Core.Events
{
    public class TargetOfEvent : EventBase
    {
        public IIsUnique Actor { get; private set; }

        public int Result { get; private set; }

        public TargetOfEvent(ILocation location, int generation, IIsUnique actor, int result)
            : base(location, generation)
        {
            if (actor == null) throw new ArgumentNullException("actor");

            this.Actor = actor;
            this.Result = result;
        }

        public override string ToString()
        {
            return string.Format("{0}: Lost encounter ({3}) with ({2}) {1}", this.Generation, Actor.ToShortString(), Actor.GetType().Name, Result);
        }
    }
}
