using System;
using System.Collections.Generic;

namespace Cas.Core.Interfaces
{
    /// <summary>
    /// Classes that implement this interface support tagging operations as proposed 
    /// in John Holland's <c>Hidden Order</c> (pg 49, for example).  Agents use their
    /// detectors to differentiate between tagged entities in the environment.  
    /// 
    /// By separating tagable operations from the agents, it makes it possible for agents
    /// to use the same detectors to interact with each other as they do to harvest resources.
    /// </summary>
    public interface IInteractable : IContainsResources
    {
        /// <summary>
        /// The offense tag for the Interactable unit.
        /// </summary>
        Tag Offense { get; set; } // pg. 101

        /// <summary>
        /// The defense tag for the Interactable unit.
        /// </summary>
        Tag Defense { get; set; } // pg. 101

        /// <summary>
        /// Governs whether or not an Interactable will interact with an IInteractable
        /// object.  (i.e. ICell/IAgent/IResource)
        /// </summary>
        Tag Exchange { get; set; } // pg. 111
    }
}
