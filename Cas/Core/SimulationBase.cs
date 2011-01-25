using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cas.Core.Events;
using Cas.Core.Extensions;
using Cas.Core.Interactions;
using Cas.Core.Interfaces;
using System.Text.RegularExpressions;

namespace Cas.Core
{
    public abstract class SimulationBase : ISimulation
    {
        #region Fields

        protected bool Initialized = false;
        protected bool Initializing = false;

        protected IEnvironment environment;
        private object pendingMigrationsSyncRoot = new object();

        #endregion

        protected SimulationBase(Configuration config)
        {
            if (config == null) throw new ArgumentNullException("config");

            this.Configuration = config;
        }

        #region ISimulation Members

        public event EventHandler GenerationStarted;
        public event EventHandler GenerationFinished;

        public bool LogHistory { get; set; }

        public Configuration Configuration { get; set; }

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
            if (birthEvent == null) throw new ArgumentNullException("birthEvent");

            this.AddEventToAgent(agent, birthEvent);

            ISpecies species;
            string key = agent.UniqueKey;
            if (!SpeciesByKey.TryGetValue(key, out species))
            {
                // Work out the ancestors
                var parentSpeciesIds = new List<long>();
                if (birthEvent is BirthEvent)
                {
                    parentSpeciesIds.AddRange((birthEvent as BirthEvent).Parents.Select(s => s.Id));
                }

                species = new Species(this, agent, parentSpeciesIds.ToArray());
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

        /// <summary>
        /// Retrieves a species from the list of currently active species, or retrieves the 
        /// fossil record of the species.
        /// </summary>
        public IIsUnique GetSpeciesOrFossil(long id)
        {
            // TODO: Try to get the fossil from any saved records

            var species = this.Species.Where(s => s.Id == id).FirstOrDefault();
            return species ?? (IIsUnique)new Fossil(id);
        }

        public int CurrentGeneration { get; protected set; }

        /// <summary>
        /// Configure the simulation.
        /// </summary>
        /// <param name="distinctResources">
        /// The number of resources available in the simulation.  These will be represented using letters starting with 'a'.
        /// </param>
        /// <param name="allowWildcards">
        /// Should tags be allowed the use of the wildcard resource?
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
        public void Initialize()
        {
            if (Initialized) throw new InvalidOperationException("The Simulation is already initialized.");

            Initializing = true;

            Resource.Initialize(this.Configuration);
            Tag.Initialize(this.Configuration);
            PointMutation.SetMutationPercentage(this.Configuration.AgentSettings.MutationChance);

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

        #region RunGeneration

        private SynchronizedCollection<KeyValuePair<IAgent, ILocation>> pendingMigrations;

        /// <summary>
        /// Queue up an agent to move to a new location at the end of the current generation.
        /// </summary>
        private void RegisterMigration(IAgent agent, ILocation location)
        {
            pendingMigrations.Add(new KeyValuePair<IAgent, ILocation>(agent, location));
        }


        /// <summary>
        /// Execute a single generation of the CAS using the environment as an input.
        /// </summary>
        public void RunGeneration()
        {
            if (!Initialized) throw new InvalidOperationException("Initialize the simulation prior to running a generation.");

            CurrentGeneration++;
            OnGenerationStarted(CurrentGeneration);

            pendingMigrations = new SynchronizedCollection<KeyValuePair<IAgent, ILocation>>(pendingMigrationsSyncRoot);

            var generationTasks = new Task[Environment.Locations.Count];
            for (int x = 0; x < this.Environment.Locations.Count; x++)
            {
                var location = Environment.Locations[x];
                generationTasks[x] = Task.Factory.StartNew(() => ProcessLocation(location));
            }

            Task.WaitAll(generationTasks);

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
        private void ProcessLocation(ILocation location)
        {
            if (location == null) throw new ArgumentNullException("location");

            // Iterate all agents and have them interact with something (agent/resource node) or migrate (if healthy enough)
            int interactionsToPerform = (int)(location.Agents.Count * this.Configuration.AgentSettings.InteractionsPerGeneration);
            DoInteractions(location, interactionsToPerform);

            // Prior to upkeep, select agents for migration
            QueueForMigration(location);

            // Change upkeep for the location
            if (location.UpkeepCost > 0 && RandomProvider.NextDouble() < this.Configuration.EnvironmentSettings.UpkeepChance)
            {
                location.ChargeUpkeep(CurrentGeneration);
            }

            // Iterate all agents and calculate fitness
            var breeders = new List<IAgent>();
            var deathIndecies = new List<int>();
            for (int i = 0; i < location.Agents.Count; i++)
            {
                var agent = location.Agents[i];

                if (RandomProvider.NextDouble() < this.Configuration.AgentSettings.RandomDeathChance || agent.IsEligableForDeath)
                {
                    deathIndecies.Add(i);
                    continue;
                }

                if (agent.CanReplicate(this.Configuration.AgentSettings.ReproductionThreshold))
                {
                    breeders.Add(agent);
                }
            }

            // Deaths (remove from the back first)
            for (int i = deathIndecies.Count - 1; i >= 0; i--)
            {
                // Kill the agent
                var condemned = location.Agents[deathIndecies[i]];
                location.Agents.RemoveAt(deathIndecies[i]);
                this.RegisterDeath(condemned);

                // Add a corpse object with the dead agent's genetic material to the location
                var corpse = new Corpse(condemned);
                location.CurrentResources.Add(corpse);
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

            // Abort early if there are no agents.
            if (location.Agents.Count == 0) return;

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

                // Pick a target
                var target = SelectTarget(allTargets, actor);
                if (target != null)
                {
                    InnerDoInteraction(location, actor, target);
                }
            }

            // TODO: We need to do interactions within multi-agents here
        }

        protected abstract void InnerDoInteraction(ILocation location, IAgent actor, IInteractable target);

        private void DoReproduction(ILocation location, List<IAgent> breeders)
        {
            if (location == null) throw new ArgumentNullException("location");
            if (breeders == null) throw new ArgumentNullException("breeders");

            // Agents reproduce sexually if they can find a suitable mate, otherwise
            // they reproduce asexually.
            while (breeders.Count > 0)
            {
                var children = new List<IAgent>();

                var first = breeders.GetRandom();
                breeders.Remove(first);

                var mate = FindMateFor(first, breeders);
                if (mate != null)
                {
                    breeders.Remove(mate);
                    children.AddRange(DoSexualReproduction(first, mate, location));
                }
                else
                {
                    children.Add(DoAsexualReproduction(first, location));
                }

                location.Agents.AddRange(children);
            }
        }

        private IAgent FindMateFor(IAgent agent, List<IAgent> breeders)
        {
            // Agents are considered valid mates if their mating tags match
            // exactly (taking into account wildcards)

            foreach (var b in breeders)
            {
                b.SetInteractionContactPoint();
                if (b.Mating.Data.Count != agent.Mating.Data.Count) continue;

                bool match = true;
                for (int i = 0; i < b.Mating.Data.Count; i++)
                {
                    if (b.Mating.Data[i] == Resource.WildcardResource || agent.Mating.Data[i] == Resource.WildcardResource) continue;
                    if (b.Mating.Data[i] != agent.Mating.Data[i])
                    {
                        match = false;
                        break;
                    }
                }

                if (match) return b;
            }

            return null;
        }

        protected abstract IAgent DoAsexualReproduction(IAgent parent, ILocation location);

        protected abstract IList<IAgent> DoSexualReproduction(IAgent parent1, IAgent parent2, ILocation location);

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
        private void QueueForMigration(ILocation location)
        {
            if (location == null) throw new ArgumentNullException("location");
            
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
                this.AddEventToAgent(agent, new MigrationEvent(location, destination, destination.UpkeepCost, this.CurrentGeneration));
                this.RegisterMigration(agent, destination);
            });
        }

        /// <summary>
        /// Determines the chance that an agent has to migrate, based on
        /// it's relative fitness.
        /// </summary>
        protected virtual double CalculateMigrationChance(IAgent agent)
        {
            double percentFull = Math.Min(1.0, (double)agent.CurrentResourceCount / (double)agent.Size);

            return this.Configuration.AgentSettings.MigrationBaseChance + ((1.0 - percentFull) * this.Configuration.AgentSettings.MaximumMigrationBonus);
        }

        public void AddEventToAgent(IAgent agent, IEvent newEvent)
        {
            if (agent == null) throw new ArgumentNullException("agent");
            if (newEvent == null) throw new ArgumentNullException("newEvent");

            if (this.LogHistory) agent.History.Add(newEvent);
        }

        /// <summary>
        /// Selects a target for the actor to interact with, taking into account
        /// the conditional exchange tags.
        /// </summary>
        protected IInteractable SelectTarget(List<IInteractable> allTargets, IAgent actor)
        {
            if (allTargets == null) throw new ArgumentNullException("allTargets");
            if (actor == null) throw new ArgumentNullException("actor");

            IInteractable target = null;
            IInteractable bestTarget = null;
            int bestScore = -1;
            for (int i = 0; i < this.Configuration.AgentSettings.MaximumAttemptsToFindSuitableTarget; i++)
            {
                target = SelectRandomTarget(allTargets, actor);
                if (target == null) continue;

                if (target is IAgent) (target as IAgent).SetInteractionContactPoint();

                int exchangeScore = CalculateConditionalExchangeMatch(actor.Exchange, target.Offense);

                if (exchangeScore > bestScore)
                {
                    bestScore = exchangeScore;
                    bestTarget = target;
                }

                // If we've already found a perfect match, stop looking.
                if (bestScore == actor.Exchange.Data.Count) break;
            }

            return bestTarget;
        }

        /// <summary>
        /// Determines whether or not the exchange tag of one individual is well matched
        /// to the offense tag of another individual.
        /// 
        /// The best possible match score is equal to the length of the exchange tag.
        /// </summary>
        protected static int CalculateConditionalExchangeMatch(Tag exchange, Tag offense)
        {
            if (exchange == null) throw new ArgumentNullException("exchange");
            if (offense == null) throw new ArgumentNullException("offense");

            int matchStrength = 0;
            for (int i = 0; i < exchange.Data.Count; i++)
            {
                if (i >= offense.Data.Count) break;
                if ((exchange.Data[i] == Resource.WildcardResource) || (exchange.Data[i] == offense.Data[i]))
                    matchStrength++;
            }

            return matchStrength;
        }

        /// <summary>
        /// Retrieves a random target from the supplied list, ensuring that it
        /// does not match the supplied actor.
        /// </summary>
        protected static TBase SelectRandomTarget<TBase, TSpecific>(List<TBase> allTargets, TSpecific actor) where TBase : class
        {
            if (allTargets == null) throw new ArgumentNullException("allTargets");
            if (actor == null) throw new ArgumentNullException("actor");

            TBase target = null;

            if (allTargets.Count == 0 || (allTargets.Count == 1 && Object.ReferenceEquals(allTargets[0], actor)))
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
