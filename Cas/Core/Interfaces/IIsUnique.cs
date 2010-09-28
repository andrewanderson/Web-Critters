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
        // No methods; this is just a grouping interface.
    }
}
