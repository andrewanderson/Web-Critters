using System;
using Cas.Core.Interfaces;

namespace Cas.Core.Events
{
    public class CreationEvent : EventBase
    {
        public CreationEvent(ILocation location, int generation) 
            : base(location, generation)  {  }

        public override string ToString()
        {
            return string.Format("{0}: Created at {1}", this.Generation, this.Location.ToShortString());
        }
    }
}
