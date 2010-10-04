using System;
using Cas.Core.Interfaces;

namespace Cas.Core.Events
{
    public abstract class EventBase : IEvent
    {
        public ILocation Location { get; private set; }

        public int Generation { get; private set; }

        protected EventBase(ILocation location,int generation) 
        {
            if (location == null) throw new ArgumentNullException("location");
            if (generation < 0) throw new ArgumentOutOfRangeException("generation");

            this.Location = location;
            this.Generation = generation;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1} at {2}", this.Generation, this.GetType().Name, this.Location.ToShortString());
        }
    }
}
