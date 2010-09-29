using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cas.Core.Interfaces
{
    /// <summary>
    /// An interface that serves to flag implementors as singletons within
    /// the simulation.
    /// 
    /// Examples:  Species, ResourceNodes
    /// </summary>
    public interface IIsUnique
    {
        /// <summary>
        /// Unique identifier for the object.
        /// </summary>
        long Id { get; }
    }
}
