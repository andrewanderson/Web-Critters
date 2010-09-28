using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cas.Core.Interfaces
{
    /// <summary>
    /// A resource node is a logical grouping of Resource objects that can be
    /// treated as a single unit within a Simulation.  
    /// </summary>
    /// <remarks>
    /// ResourceNodes may be as simple as a single character (Grid CAS), or as complex as
    /// a of multi-character bundle (Twitter CAS), or anything in between.  ResourceNodes
    /// are usually located in ILocation objects.
    /// </remarks>
    public interface IResourceNode : IInteractable, IIsUnique
    {
        /// <summary>
        /// The Resources that can be derived from this Resource Node.
        /// </summary>
        List<Resource> RenewableResources { get; }

        /// <summary>
        /// Instruct the resource node to populate the reservoir with new 
        /// copies of all renewable resources.
        /// </summary>
        void RefreshReservoir();

        IResourceNode DeepCopy();
    }
}
