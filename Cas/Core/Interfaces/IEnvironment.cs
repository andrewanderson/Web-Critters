using System;
using System.Collections.Generic;

namespace Cas.Core.Interfaces
{
    /// <summary>
    /// An environment is a collection of locations.
    /// </summary>
    public interface IEnvironment: IDisposable
    {
        /// <summary>
        /// The simulation to which this environment belongs.
        /// </summary>
        ISimulation Simulation { get; }

        /// <summary>
        /// The locations that the environment currently encompasses.  
        /// 
        /// In some simulations the location list may be static (i.e. a simple grid) while in
        /// others it may be dynamic (i.e. the Internet).
        /// </summary>
        List<ILocation> Locations { get; }

        /// <summary>
        /// Perform any necessary start-up tasks
        /// </summary>
        void Initialize();

        /// <summary>
        /// Retrieve a resource node fromt he environment using its 
        /// (negative) unique identifier as a look-up.
        /// </summary>
        IResourceNode FindResourceNodeById(long id);
    }
}
