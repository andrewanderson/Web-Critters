using System;
using Cas.Core.Interfaces;

namespace Cas.Core.Events
{
    public abstract class EventBase : IEvent
    {
        public Guid LocationId { get; private set; }

        public int Generation { get; private set; }

        protected EventBase(Guid locationId,int generation) 
        {
            if (generation < 0) throw new ArgumentOutOfRangeException("generation");

            this.LocationId = locationId;
            this.Generation = generation;
        }

        public static IIsUnique ToUnique(IInteractable agentOrResourceNode) 
        {
            if (agentOrResourceNode is IResourceNode)
            {
                return agentOrResourceNode as IIsUnique;
            }
            else if (agentOrResourceNode is IAgent)
            {
                return (agentOrResourceNode as IAgent).Species;
            }
            else
            {
                throw new InvalidCastException("Could not cast to IIsUnique");
            }
        }

        public override string ToString()
        {
            return string.Format("{0}: {1} at location {2}", this.Generation, this.GetType().Name, this.LocationId);
        }
    }
}
