using System;

namespace Cas.Core.Events
{
    public interface IEvent
    {
        /// <summary>
        /// The identifier of the location in which the event took place
        /// </summary>
        Guid LocationId { get; }

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
