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
        /// A catalogue of all unique species in the simulation.
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
        /// Retrieves a species from the list of currently active species, or retrieves the 
        /// fossil record of the species.
        /// </summary>
        IIsUnique GetSpeciesOrFossil(UniqueIdentifier id);

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
        void Initialize();

        /// <summary>
        /// Discard the current simulation configuration.
        /// </summary>
        void Reset();

        /// <summary>
        /// Execute a single generation of the CAS.
        /// </summary>
        void RunGeneration();

        /// <summary>
        /// Should events be logged on agents?
        /// </summary>
        bool LogHistory { get; set; }

        /// <summary>
        /// The settings for the simulation.
        /// </summary>
        Configuration Configuration { get; }

    }
}
