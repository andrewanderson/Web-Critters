using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cas.Core.Interfaces
{
    /// <summary>
    /// A physical location within the simulation.  May be a location in the environment,
    /// or one level of a (potentially) multi-cellular agent.
    /// </summary>
    public interface IBoundary
    {
        /// <summary>
        /// The individual agents that are currently living in this boundary.
        /// </summary>
        List<IAgent> Agents { get; }

        /// <summary>
        /// The resources that are currently available at this boundary.
        /// </summary>
        List<IResourceNode> CurrentResources { get; }
    }
}
