using System;
using System.Collections.Generic;

namespace Cas.Core.Interfaces
{
    /// <summary>
    /// Classes that implement this interface represent the top-level CAS simulations that will be run.
    /// A simulation determines the parameters and settings that will be used to manage the CAS.
    /// </summary>
    public interface ISimulation : IDisposable
    {

        event EventHandler GenerationStarted;
        event EventHandler GenerationFinished;

        /// <summary>
        /// The environment that the CAS will run with.
        /// </summary>
        IEnvironment Environment { get; }

        /// <summary>
        /// The generation number that is either about to execute, or is executing.
        /// </summary>
        int CurrentGeneration { get; }

        /// <summary>
        /// Performs all start-up tasks associated with the simulation.  After executing this method
        /// the Simulation should be in a state whereby it can execute any number of generations.
        /// </summary>
        /// <param name="distinctResources">
        /// The number of resources available in the simulation.  These will be represented using letters starting with 'a'.
        /// </param>
        /// <param name="normalToWildcardResourceRatio">
        /// The prominence of wildcard resources
        /// </param>
        void Initialize(int distinctResources, int normalToWildcardResourceRatio);

        /// <summary>
        /// Discard the current simulation configuration.
        /// </summary>
        void Reset();

        /// <summary>
        /// Execute a single generation of the CAS.
        /// </summary>
        void RunGeneration();

        #region Settings

        /// <summary>
        /// The number of interactions that will take place in a given location for
        /// each generation in the simulation per agent that is present.  These interactions
        /// will be randomly distributed.
        /// </summary>
        double InteractionsPerGenerationFactor { get; }

        /// <summary>
        /// The maximum number of resources that a location may charge its residents each generation.
        /// </summary>
        int MaximumUpkeepCostPerLocation { get; }

        /// <summary>
        /// The percent chance of upkeep being charged per location (0-1)
        /// </summary>
        double UpkeepChance { get; }

        /// <summary>
        /// The number of resources that an agent requires to initiate reproduction above and beyond
        /// the amount needed to replicate its genome.
        /// </summary>
        double ReproductionThreshold { get; }

        /// <summary>
        /// The percentage of resources that a parent agent will contribute to a newly formed child.
        /// </summary>
        double ReproductionInheritance { get; }

        #endregion

    }
}
