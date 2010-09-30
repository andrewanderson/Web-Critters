using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cas.Core.Interfaces
{
    /// <summary>
    /// A unique type of agent within a simulation.
    /// </summary>
    public interface ISpecies : IIsUnique
    {
        /// <summary>
        /// An example of the agent
        /// </summary>
        IAgent Exemplar { get; }

        /// <summary>
        /// The generation that this species first arose.
        /// </summary>
        long FirstSeen { get; }

        /// <summary>
        /// The number of resources consumed from ResourceNodes
        /// across all agents in this species.
        /// </summary>
        long ResourcesFromResourceNodes { get; }

        /// <summary>
        /// The number of resources consumed from Agents
        /// across all agents in this species.
        /// </summary>
        long ResourcesFromAgents { get; }

        /// <summary>
        /// A classifcation of this species based of what it consumes.
        /// </summary>
        DietType DietType { get; }

        /// <summary>
        /// Report that an agent of this species consumed resources.
        /// </summary>
        void RecordConsumptionOf(IIsUnique prey, int amount);

        /// <summary>
        /// Report that an agent of this species was successfully attacked by a predator.
        /// </summary>
        void RecordPredation(ISpecies predator);

        /// <summary>
        /// The total number of agents in the simulation with this species.
        /// </summary>
        long Population { get; set; }

        /// <summary>
        /// The locations in the simulation where at least one member of this species can be found.
        /// </summary>
        IEnumerable<ILocation> Habitat { get; }

        /// <summary>
        /// The foods that this species has consumed across all time, ordered by number of
        /// occurences.
        /// </summary>
        IEnumerable<IIsUnique> Prey { get; }

        /// <summary>
        /// The agents that this species has been predated by across all time, ordered by number of
        /// occurences.
        /// </summary>
        IEnumerable<IIsUnique> Predators { get; }
    }
}
