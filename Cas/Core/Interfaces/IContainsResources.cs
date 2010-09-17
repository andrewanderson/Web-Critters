using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cas.Core.Interfaces
{
    /// <summary>
    /// An interface that encapsulates the notion of storing and retrieving resources.
    /// </summary>
    public interface IContainsResources
    {
        /// <summary>
        /// The total number of resources stored in the object.
        /// </summary>
        int CurrentResourceCount { get; }

        /// <summary>
        /// Removes up to <paramref name="count" /> random resources from the Reservoir of the object.
        /// </summary>
        List<Resource> RemoveResources(int count);

        /// <summary>
        /// Adds the contents of <paramref name="resources" /> to the Reservoir of the object.
        /// </summary>
        void AddResources(List<Resource> resources);

        /// <summary>
        /// A visualization method to allow display of all of the resources currently possessed
        /// by the object.
        /// </summary>
        string ShowResourcePool(string delimiter);
    }
}
