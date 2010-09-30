using System;
using System.Collections.Generic;
using Cas.Core.Events;

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
        /// A catalog of all unique species in the simulation.
        /// </summary>
        List<ISpecies> Species { get; }

        /// <summary>
        /// Performs work required to add a new agent to the simulation.
        /// </summary>
        /// <remarks>
        /// The key piece of work is to ensure that a species exists for
        /// this agent.
        /// </remarks>
        void RegisterBirth(IAgent agent, IEvent birthEvent);

        /// <summary>
        /// Records the death of an agent, which updates the underlying species.
        /// </summary>
        void RegisterDeath(IAgent agent);

        /// <summary>
        /// Attaches a history event to the specified agent
        /// </summary>
        void AddEventToAgent(IAgent agent, IEvent newEvent);

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
        /// <param name="maximumTagSize">
        /// The largest size that a tag can reach in the simulation.
        /// </param>
        /// <param name="mutationPercent">
        /// The percentage chance that any given point within a cell mutates during a reproduction.
        /// </param>
        void Initialize(int distinctResources, int normalToWildcardResourceRatio, int maximumTagSize, double mutationPercent);

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
        /// Should events be logged on agents?
        /// </summary>
        bool LogHistory { get; set; }

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

        /// <summary>
        /// The base percent chance (0-1) that an agent decides to migrate to a new location, per generation.
        /// </summary>
        double MigrationBaseChance { get; }

        /// <summary>
        /// The maximum bonus to MigrationBaseChance (0-1) that an agent can be awarded for being unfit.
        /// </summary>
        double MaximumMigrationBonus { get; }

        /// <summary>
        /// The percent chance (0-1) that an agent will randomly die at the end of a generation.
        /// </summary>
        double RandomDeathChance { get; }

        #endregion

    }
}
