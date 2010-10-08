using System;
using System.Collections.Generic;
using System.Linq;

namespace Cas.Core.Interfaces
{
    /// <summary>
    /// A location represents one discrete container within the CAS environment.  Locations are home
    /// to resources and agents.
    /// </summary>
    public interface ILocation : IBoundary
    {
        /// <summary>
        /// The cost, in resources, that the location levies on its agents.  Must be
        /// a positive whole number.
        /// </summary>
        int UpkeepCost { get; set; }

        /// <summary>
        /// A list of locations that are considered connected to this location.
        /// </summary>
        List<ILocation> Connections { get; }

        /// <summary>
        /// The amount of resources of each type that are available at the start of every generation for this location
        /// </summary>
        List<IResourceNode> ResourceAllocation { get; }

        /// <summary>
        /// Create a connection to another location.
        /// </summary>
        void ConnectTo(ILocation location);

        /// <summary>
        /// Replenish all of the resources at this location
        /// </summary>
        void RefreshResourcePool();

        /// <summary>
        /// Remove resources equal to the UpkeepCost from all agents in this location.
        /// </summary>
        void ChargeUpkeep(int generation);

        /// <summary>
        /// A terse representation of the Location.
        /// </summary>
        string ToShortString();
    }
}
