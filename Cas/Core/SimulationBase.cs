﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cas.Core.Events;
using Cas.Core.Extensions;
using Cas.Core.Interactions;
using Cas.Core.Interfaces;

namespace Cas.Core
{
    public abstract class SimulationBase : ISimulation
    {
        #region Fields

        protected bool Initialized = false;
        protected bool Initializing = false;

        protected IEnvironment environment;

        #endregion

        protected SimulationBase() 
            : this(1.5, 4, 0.25, 1.75, 0.2, 0.005, 0.02, 0.005) { }

        protected SimulationBase(double interactionsPerGenerationFactor, int maximumUpkeepCostPerLocation, double upkeepChance, 
            double reproductionThreshold, double reproductionInheritance, double migrationBaseChance, double maxMigrationBonus, double randomDeathChance)
        {
            // Set some defaults
            InteractionsPerGenerationFactor = interactionsPerGenerationFactor;
            MaximumUpkeepCostPerLocation = maximumUpkeepCostPerLocation;
            UpkeepChance = upkeepChance;

            ReproductionThreshold = reproductionThreshold;
            ReproductionInheritance = reproductionInheritance;

            MigrationBaseChance = migrationBaseChance;
            MaximumMigrationBonus = maxMigrationBonus;

            RandomDeathChance = randomDeathChance;
        }

        #region ISimulation Members
        
        public event EventHandler GenerationStarted;
        public event EventHandler GenerationFinished;

        protected virtual void OnGenerationStarted(int generationNumber)
        {
            if (GenerationStarted != null)
            {
                GenerationStarted(this, new GenerationEventArgs(generationNumber));
            }
        }

        protected virtual void OnGenerationFinished(int generationNumber)
        {
            if (GenerationFinished != null)
            {
                GenerationFinished(this, new GenerationEventArgs(generationNumber));
            }
        }

        /// <summary>
        /// The environment that is used in this simulation.
        /// </summary>
        public IEnvironment Environment
        {
            get
            {
                return environment;
            }
            protected set
            {
                environment = value;
            }
        }

        /// <summary>
        /// A catalog of all unique species in the simulation.
        /// </summary>
        public List<ISpecies> Species 
        {
            get
            {
                return this.SpeciesByKey
                    .OrderBy(kvp => kvp.Value.Id)
                    .Select(kvp => kvp.Value)
                    .ToList();
            }
        }

        protected readonly Dictionary<string, ISpecies> SpeciesByKey = new Dictionary<string, ISpecies>();

        /// <summary>
        /// Performs work required to add a new agent to the simulation.
        /// </summary>
        /// <remarks>
        /// The key piece of work is to ensure that a species exists for
        /// this agent.
        /// </remarks>
        public void RegisterBirth(IAgent agent, IEvent birthEvent)
        {
            if (agent == null) throw new ArgumentNullException("agent");

            this.AddEventToAgent(agent, birthEvent);

            ISpecies species;
            string key = agent.UniqueKey;
            if (!SpeciesByKey.TryGetValue(key, out species))
            {
                species = new Species(this, agent);
                SpeciesByKey.Add(key, species);
            }

            species.Population++;
            agent.Species = species;
        }

        /// <summary>
        /// Records the death of an agent, which updates the underlying species.
        /// </summary>
        public void RegisterDeath(IAgent agent)
        {
            if (agent == null) throw new ArgumentNullException("agent");
            if (agent.Species == null) throw new ArgumentException("Agent cannot have a null species.", "agent");

            if (--agent.Species.Population <= 0)
            {
                this.SpeciesByKey.Remove(agent.UniqueKey);
            }
        }
        
        public int CurrentGeneration { get; protected set; }

        /// <summary>
        /// Configure the simulation.
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
        /// <exception cref="InvalidOperationException">
        /// Thrown if this method is called when the class is already initialized.
        /// </exception>
        /// <remarks>
        /// This method must be called before any other class methods.  Once it is called,
        /// it may not be called again without first calling <see cref="Reset"/>.
        /// </remarks>
        public void Initialize(int distinctResources, int normalToWildcardResourceRatio, int maximumTagSize, double mutationPercent)
        {
            if (Initialized) throw new InvalidOperationException("The Simulation is already initialized.");

            Initializing = true;

            Resource.Initialize(distinctResources, normalToWildcardResourceRatio);
            Tag.Initialize(maximumTagSize);
            PointMutation.SetMutationPercentage(mutationPercent);

            InnerInitialize();

            if (Environment != null)
            {
                Environment.Initialize();
            }
            else
            {
                throw new InvalidOperationException("Set the Environment prior to calling Initialize.");
            }

            Initializing = false;
            Initialized = true;
        }

        /// <summary>
        /// Discard the current simulation configuration.
        /// </summary>
        public void Reset()
        {
            Initialized = false;

            InnerReset();

            Environment.Dispose();
            Environment = null;

            CurrentGeneration = 0;
        }

        #endregion

        #region Settings

        public bool LogHistory { get; set; }

        public double InteractionsPerGenerationFactor { get; protected set; }

        public int MaximumUpkeepCostPerLocation { get; set; }

        public double UpkeepChance { get; set; }

        public double ReproductionThreshold { get; set; }

        public double ReproductionInheritance { get; set; }

        public double MigrationBaseChance { get; set; }

        public double MaximumMigrationBonus { get; set; }

        public double RandomDeathChance { get; set; }

        #endregion

        #region RunGeneration

        /// <summary>
        /// Execute a single generation of the CAS using the environment as an input.
        /// </summary>
        public void RunGeneration()
        {
            if (!Initialized) throw new InvalidOperationException("Initialize the simulation prior to running a generation.");

            CurrentGeneration++;
            OnGenerationStarted(CurrentGeneration);

            var pendingMigrations = new List<KeyValuePair<IAgent, ILocation>>();
            foreach (var location in this.Environment.Locations)
            {
                ProcessLocation(location, pendingMigrations);
            }

            // Relocate the migrants and charge them upkeep at their new location
            foreach (var migration in pendingMigrations)
            {
                migration.Key.RemoveResources(migration.Value.UpkeepCost);
                migration.Value.Agents.Add(migration.Key);
            }

            OnGenerationFinished(CurrentGeneration);
        }

        /// <summary>
        /// Perform all of the activities for a generation at the specified location.
        /// </summary>
        /// <param name="location">
        /// The location to operate on.
        /// </param>
        /// <param name="pendingMigrations">
        /// A list to append agents that wish to migrate on to.
        /// </param>
        private void ProcessLocation(ILocation location, List<KeyValuePair<IAgent, ILocation>> pendingMigrations)
        {
            if (location == null) throw new ArgumentNullException("location");
            if (pendingMigrations == null) throw new ArgumentNullException("pendingMigrations");

            // Iterate all agents and have them interact with something (agent/resource node) or migrate (if healthy enough)
            int interactionsToPerform = (int)(location.Agents.Count*InteractionsPerGenerationFactor);
            DoInteractions(location, interactionsToPerform);

            // Prior to upkeep, select agents for migration
            QueueForMigration(location, pendingMigrations);

            // Change upkeep for the location
            if (location.UpkeepCost > 0 && RandomProvider.NextDouble() < this.UpkeepChance)
            {
                location.ChargeUpkeep(CurrentGeneration);
            }

            // Iterate all agents and calculate fitness
            var breeders = new List<IAgent>();
            var deathIndecies = new List<int>();
            for (int i = 0; i < location.Agents.Count; i++)
            {
                var agent = location.Agents[i];

                if (RandomProvider.NextDouble() < this.RandomDeathChance || agent.IsEligableForDeath)
                {
                    deathIndecies.Add(i);
                    continue;
                }

                if (agent.CanReplicate(ReproductionThreshold))
                {
                    breeders.Add(agent);
                }
            }

            // Deaths (remove from the back first)
            for (int i = deathIndecies.Count - 1; i >= 0; i--)
            {
                var condemned = location.Agents[deathIndecies[i]];
                location.Agents.RemoveAt(deathIndecies[i]);
                this.RegisterDeath(condemned);
            }

            // Age all surviving agents
            location.Agents.ForEach(agent => agent.IncrementAge());

            // Breed fit agents (sexual / asexual)
            DoReproduction(location, breeders);

            location.RefreshResourcePool();
        }

        private void DoInteractions(ILocation location, int interactionsToPerform)
        {
            if (location == null) throw new ArgumentNullException("location");
            if (interactionsToPerform < 0) throw new ArgumentOutOfRangeException("interactionsToPerform");

            // The targets of interactions are resource nodes and other agents
            var allTargets = location.Agents.ConvertAll(x => x as IInteractable).Concat(location.CurrentResources.ConvertAll(x => x as IInteractable)).ToList();

            // Perform the interactions, ensuring that all agents get a chance 
            // to perform one action before any agent performs its second action.
            var candidateActors = new List<IAgent>();
            for (int i = 0; i < interactionsToPerform; i++)
            {
                // Recharge the candidate list if necessary
                if (candidateActors.Count == 0)
                {
                    candidateActors.AddRange(location.Agents);
                }

                var actor = candidateActors.RemoveRandom();
                actor.SetInteractionContactPoint();

                InnerDoInteraction(location, actor, allTargets);
            }

            // TODO: We need to do interactions within multi-agents here
        } 

        protected abstract void InnerDoInteraction(ILocation location, IAgent actor, List<IInteractable> targets);
        
        private void DoReproduction(ILocation location, List<IAgent> breeders)
        {
            if (location == null) throw new ArgumentNullException("location");
            if (breeders == null) throw new ArgumentNullException("breeders");

            while (breeders.Count > 0)
            {
                DoOneReproduction(location, breeders);
            }
        }

        protected abstract void DoOneReproduction(ILocation location, List<IAgent> breeders);

        /// <summary>
        /// Selects agents from the location at random, and queues them to
        /// move into a new location at the end of this generation.
        /// 
        /// As suggested in Hidden Order pg 151, agents with relatively less fitness
        /// have a better chance at migrating.
        /// </summary>
        /// <param name="location">
        /// The location to perform migration actions on
        /// </param>
        /// <param name="pendingMigrations">
        /// A list to add all migrants to.
        /// </param>
        private void QueueForMigration(ILocation location, List<KeyValuePair<IAgent, ILocation>> pendingMigrations)
        {
            if (location == null) throw new ArgumentNullException("location");
            if (pendingMigrations == null) throw new ArgumentNullException("pendingMigrations");

            if (location.Connections.Count == 0) return;

            var migrants = location.Agents
                    .Where(agent => (RandomProvider.NextDouble() < CalculateMigrationChance(agent)))
                    .Select(agent => new KeyValuePair<IAgent, ILocation>(agent, location.Connections.GetRandom()))
                    .ToList();

            migrants.ForEach(kvp =>
            {
                var agent = kvp.Key;
                var destination = kvp.Value;

                location.Agents.Remove(agent);
                this.AddEventToAgent(agent, new MigrationEvent(location.Id, destination.Id, destination.UpkeepCost, this.CurrentGeneration));
                pendingMigrations.Add(kvp);
            });
        }

        /// <summary>
        /// Determines the chance that an agent has to migrate, based on
        /// it's relative fitness.
        /// </summary>
        protected virtual double CalculateMigrationChance(IAgent agent)
        {
            double percentFull = Math.Max(1, agent.CurrentResourceCount / agent.Size);
            
            return MigrationBaseChance + ((1 - percentFull) * MaximumMigrationBonus);
        }

        public void AddEventToAgent(IAgent agent, IEvent newEvent) 
        {
            if (agent == null) throw new ArgumentNullException("agent");
            if (newEvent == null) throw new ArgumentNullException("newEvent");

            if (this.LogHistory) agent.History.Add(newEvent);
        }

        /// <summary>
        /// Retrieves a random target from the supplied list, ensuring that it
        /// does not match the supplied actor.
        /// </summary>
        protected static TBase SelectRandomTarget<TBase, TSpecific>(List<TBase> allTargets, TSpecific actor) where TBase : class
        {
            if (allTargets == null) throw new ArgumentNullException("allTargets");
            if (allTargets == null) throw new ArgumentNullException("actor");

            TBase target = null;

            if (allTargets.Count == 0 || (allTargets.Count == 1 && Object.ReferenceEquals(allTargets[0],actor)))
            {
                return null;
            }

            while (target == null)
            {
                var potentialTarget = allTargets.GetRandom();
                if (!Object.ReferenceEquals(target, actor))
                {
                    target = potentialTarget;
                }
            }

            return target;
        }

        #endregion

        #region IDisposable Members

        public abstract void Dispose();

        #endregion

        /// <summary>
        /// Perform any implementation-specific set-up.
        /// </summary>
        protected abstract void InnerInitialize();

        /// <summary>
        /// Perform any implementation-specific tear-down.
        /// </summary>
        protected abstract void InnerReset();

        public static IIsUnique ToUnique(IInteractable agentOrResourceNode)
        {
            if (agentOrResourceNode is IResourceNode)
            {
                var rn = agentOrResourceNode as IResourceNode;
                return rn.Source ?? rn;
            }
            else if (agentOrResourceNode is IAgent)
            {
                return (agentOrResourceNode as IAgent).Species;
            }
            else
            {
                throw new InvalidCastException("Could not cast to IIsUnique");
            }
        }
    }
}
