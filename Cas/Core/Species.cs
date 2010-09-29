using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cas.Core.Interfaces;

namespace Cas.Core
{
    public class Species : ISpecies
    {
        /// <summary>
        /// Unique identifier for the species.
        /// </summary>
        public long Id {
            get 
            {
                return this.id;
            }
        }
        private readonly long id;

        private static long NextAvailableId = 1;

        /// <summary>
        /// An example of the agent that this species represents
        /// </summary>
        public IAgent Exemplar { 
            get
            {
                return this.exemplar;
            } 
        }
        private readonly IAgent exemplar;

        /// <summary>
        /// The generation that this species first arose.
        /// </summary>
        public long FirstSeen
        {
            get 
            { 
                return this.firstSeen; 
            }
        }
        private readonly long firstSeen;

        /// <summary>
        /// The number of resources consumed from ResourceNodes
        /// across all agents in this species.
        /// </summary>
        public long ResourcesFromResourceNodes { get; private set; }

        /// <summary>
        /// The number of resources consumed from Agents
        /// across all agents in this species.
        /// </summary>
        public long ResourcesFromAgents { get; private set; }

        /// <summary>
        /// A classifcation of this species based of what it consumes.
        /// </summary>
        /// <remarks>
        ///  0-15%  meat = herbivore
        /// 16-85%  meat = omnivore
        /// 86-100% meat = carnivore
        /// </remarks>
        public DietType DietType
        {
            get 
            {
                long totalConsumed = ResourcesFromAgents + ResourcesFromResourceNodes;

                if (totalConsumed == 0) return DietType.None;

                double percentMeat = ResourcesFromAgents / totalConsumed;
                if (percentMeat < 0.15)
                {
                    return DietType.Herbivore;
                }
                else if (percentMeat > 0.85)
                {
                    return DietType.Carnivore;
                }

                return DietType.Omnivore;
            }
        }

        private readonly ISimulation Simulation;
        private readonly string toString;

        protected readonly Dictionary<IIsUnique, long> preyCounts = new Dictionary<IIsUnique,long>(IIsUnqiueEqualityComparer.Instance);
        protected readonly Dictionary<ISpecies, long> predatorCounts = new Dictionary<ISpecies, long>(IIsUnqiueEqualityComparer.Instance);

        public Species(ISimulation simulation, IAgent exemplar)
        {
            if (simulation == null) throw new ArgumentNullException("simulation");
            if (exemplar == null) throw new ArgumentNullException("exemplar");

            this.Simulation = simulation;
            this.firstSeen = simulation.CurrentGeneration;
            this.id = NextAvailableId++;
            this.ResourcesFromResourceNodes = 0;
            this.ResourcesFromAgents = 0;
            this.Population = 0;

            this.exemplar = exemplar.DeepCopy();


            this.toString = string.Format("#{0}, {1}", this.id, this.exemplar);
        }

        /// <summary>
        /// Report that an agent of this species consumed resources.
        /// </summary>
        public void RecordConsumptionOf(IIsUnique prey, int amount)
        {
            if (prey == null) throw new ArgumentNullException("prey");
            if (amount < 1) throw new ArgumentOutOfRangeException("amount");

            if (prey is ISpecies)
            {
                ResourcesFromAgents += amount;

                (prey as ISpecies).RecordPredation(this);
            }
            else // IResourceNode
            {
                ResourcesFromResourceNodes += amount;
            }

            // Increment the counter for this consumption
            if (!preyCounts.ContainsKey(prey))
            {
                preyCounts.Add(prey, 0);
            }
            preyCounts[prey] += 1;
        }

        /// <summary>
        /// Report that an agent of this species was successfully attacked by a predator.
        /// </summary>
        public void RecordPredation(ISpecies predator)
        {
            // Increment the counter for this consumption
            if (!predatorCounts.ContainsKey(predator))
            {
                predatorCounts.Add(predator, 0);
            }
            predatorCounts[predator] += 1;
        }

        /// <summary>
        /// The total number of agents in the simulation with this species.
        /// </summary>
        public long Population { get; set; }

        /// <summary>
        /// The locations in the simulation where at least one member of this species can be found.
        /// </summary>
        public List<ILocation> Habitat
        {
            get 
            {
                return this.Simulation
                    .Environment
                    .Locations
                    .Where(loc => loc.Agents.Any(agent => agent.Species == this))
                    .ToList();
            }
        }

        /// <summary>
        /// The foods that this species has consumed across all time, ordered by number of
        /// occurences.
        /// </summary>
        public List<IIsUnique> Prey
        {
            get 
            {
                return this.preyCounts
                    .OrderBy(kvp => kvp.Value)
                    .Select(kvp => kvp.Key)
                    .ToList();
            }
        }

        /// <summary>
        /// The agents that this species has been predated by across all time, ordered by number of
        /// occurences.
        /// </summary>
        public List<ISpecies> Predators
        {
            get 
            {
                return this.predatorCounts
                    .OrderBy(kvp => kvp.Value)
                    .Select(kvp => kvp.Key)
                    .ToList();
            }
        }

        public override string ToString()
        {
            return this.toString;
        }
    }
}
