using System;
using Cas.Core.Interfaces;

namespace Cas.Core.Events
{
    public interface IEvent
    {
        /// <summary>
        /// The location at which the event took place
        /// </summary>
        ILocation Location { get; }

        /// <summary>
        /// The generation in which the event occurred
        /// </summary>
        int Generation { get; }

        /// <summary>
        /// A textual description of the event.
        /// </summary>
        string ToString();
    }
}
