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
        /// The species id(s) that this species arose from, if any.
        /// </summary>
        /// <remarks>
        /// Species that were created at the start of the simulation
        /// will have an empty list, species that arose from AsexualReproduction
        /// will have one Id, and species that arose from SexualReproduction
        /// will have two Ids.
        /// </remarks>
        public List<long> DerivedFromSpeciesIds 
        {
            get
            {
                return this.derivedFromSpeciesIds;
            }
        }
        private readonly List<long> derivedFromSpeciesIds = new List<long>();
        
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

                double percentMeat = (double)ResourcesFromAgents / (double)totalConsumed;
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

        protected readonly Dictionary<long, long> preyCounts = new Dictionary<long, long>();
        protected readonly Dictionary<long, long> predatorCounts = new Dictionary<long, long>();

        public Species(ISimulation simulation, IAgent exemplar, params long[] derivedFromSpeciesIds)
        {
            if (simulation == null) throw new ArgumentNullException("simulation");
            if (exemplar == null) throw new ArgumentNullException("exemplar");

            this.Simulation = simulation;
            this.firstSeen = simulation.CurrentGeneration;
            this.id = NextAvailableId++;
            this.ResourcesFromResourceNodes = 0;
            this.ResourcesFromAgents = 0;
            this.Population = 0;

            this.derivedFromSpeciesIds.AddRange(derivedFromSpeciesIds);
            this.exemplar = exemplar.DeepCopy();
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
            if (!preyCounts.ContainsKey(prey.Id))
            {
                preyCounts.Add(prey.Id, 0);
            }
            preyCounts[prey.Id] += 1;
        }

        /// <summary>
        /// Report that an agent of this species was successfully attacked by a predator.
        /// </summary>
        public void RecordPredation(ISpecies predator)
        {
            // Increment the counter for this consumption
            if (!predatorCounts.ContainsKey(predator.Id))
            {
                predatorCounts.Add(predator.Id, 0);
            }
            predatorCounts[predator.Id] += 1;
        }

        /// <summary>
        /// The total number of agents in the simulation with this species.
        /// </summary>
        public long Population { get; set; }

        /// <summary>
        /// The locations in the simulation where at least one member of this species can be found.
        /// </summary>
        public IEnumerable<ILocation> Habitat
        {
            get 
            {
                return this.Simulation
                    .Environment
                    .Locations
                    .Where(loc => loc.Agents.Any(agent => agent.Species == this));
            }
        }

        /// <summary>
        /// The foods that this species has consumed across all time, ordered by number of
        /// occurences.
        /// </summary>
        public IEnumerable<IIsUnique> Prey
        {
            get 
            {
                return this.preyCounts
                    .OrderByDescending(kvp => kvp.Value)
                    .Select(kvp =>
                    {
                        return (kvp.Key < 0) ? 
                            this.Simulation.Environment.FindResourceNodeById(kvp.Key)
                            : this.Simulation.GetSpeciesOrFossil(kvp.Key);
                    });
            }
        }

        /// <summary>
        /// The agents that this species has been predated by across all time, ordered by number of
        /// occurences.
        /// </summary>
        public IEnumerable<IIsUnique> Predators
        {
            get 
            {
                return this.predatorCounts
                    .OrderByDescending(kvp => kvp.Value)
                    .Select(kvp => this.Simulation.GetSpeciesOrFossil(kvp.Key));
            }
        }

        public string ToShortString()
        {
            return this.ToString();
        }

        public override string ToString()
        {
            return string.Format("S.{0}: {1}", this.id, this.exemplar.ToShortString());
        }
    }
}
