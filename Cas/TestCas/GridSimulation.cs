using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cas.Core;
using Cas.Core.Events;
using Cas.Core.Extensions;
using Cas.Core.Interactions;
using Cas.Core.Interfaces;

namespace Cas.TestImplementation
{
    public class GridSimulation : SimulationBase
    {
        public int Length { get; private set; }
        public int Width { get; private set; }
        private int MinResourceNodesPerLocation { get; set; }
        private int MaxResourceNodesPerLocation { get; set; }
        private int MinResourcesPerNodePerLocation { get; set; }
        private int MaxResourcesPerNodePerLocation { get; set; }
        private int MinResourceNodeDefense { get; set; }
        private int MaxResourceNodeDefense { get; set; }
        private int StartingTagComplexity { get; set; }
        private int UniqueResourceCount { get; set; }

        private readonly IInteraction<IInteractable, IInteractable, int> attackInteraction;
        private readonly IInteraction<ICell, ICell, IList<ICell>> crossoverInteraction;
        private readonly IInteraction<ICell, ICell, IList<ICell>> multipointCrossoverInteraction;
        private readonly IInteraction<ICell, ICell, ICell> asexualReproductionInteraction;

        public GridSimulation(int length, int width, int minResourceNodes, int maxResourceNodes, int minResourcesPerNode, int maxResourcesPerNode, 
                              int minResourceNodeDefense, int maxResourceNodeDefense, int tagComplexity) 
            : this(length, width, minResourceNodes, maxResourceNodes, minResourcesPerNode, maxResourcesPerNode, minResourceNodeDefense, maxResourceNodeDefense, tagComplexity, 
                   100, 4, 0.25, 1.5, 1.75, 0.2, 0.005, 0.02, 0.005) { }

        public GridSimulation(int length, int width, int minResourceNodes, int maxResourceNodes, int minResourcesPerNode, int maxResourcesPerNode, 
                              int minResourceNodeDefense, int maxResourceNodeDefense, int tagComplexity, int uniqueResourceCount, int maximumUpkeepCostPerLocation, 
                              double upkeepChance, double interactionsPerGeneration, double reproductionThreshold, double reproductionInheritance, 
                              double migrationBaseChance, double maxMigrationBonus, double randomDeathChance)
            : base(interactionsPerGeneration, maximumUpkeepCostPerLocation, upkeepChance, reproductionThreshold, reproductionInheritance, 
                   migrationBaseChance, maxMigrationBonus, randomDeathChance)
        {
            Length = length;
            Width = width;
            MinResourceNodesPerLocation = minResourceNodes;
            MaxResourceNodesPerLocation = maxResourceNodes;
            MinResourcesPerNodePerLocation = minResourcesPerNode;
            MaxResourcesPerNodePerLocation = maxResourcesPerNode;
            MinResourceNodeDefense = minResourceNodeDefense;
            MaxResourceNodeDefense = maxResourceNodeDefense;
            StartingTagComplexity = tagComplexity;
            UniqueResourceCount = uniqueResourceCount;

            attackInteraction = new AttackInteraction(2, -2, 0, 1, 1.0, 1);
            crossoverInteraction = new CrossoverInteraction(true, ReproductionInheritance);
            multipointCrossoverInteraction = new MultipointCrossoverInteraction(true, ReproductionInheritance);
            asexualReproductionInteraction = new AsexualInteraction(true, ReproductionInheritance);
        }

        protected override void InnerInitialize()
        {
            environment = new GridEnvironment(
                Length, 
                Width, 
                MinResourceNodesPerLocation, 
                MaxResourceNodesPerLocation, 
                MinResourcesPerNodePerLocation, 
                MaxResourcesPerNodePerLocation,
                MinResourceNodeDefense,
                MaxResourceNodeDefense,
                StartingTagComplexity,
                UniqueResourceCount,
                this);
        }

        protected override void InnerReset()
        {
            // No-op (for now)
        }

        protected override void InnerDoInteraction(ILocation location, IAgent actor, List<IInteractable> targets)
        {
            if (location == null) throw new ArgumentNullException("location");
            if (targets == null) throw new ArgumentNullException("targets");

            // TODO: Pick an interaction from a list when we have more than one
            // TODO: Other interaction types

            // Pick a target
            var target = SelectRandomTarget(targets, actor);
            if (target is IAgent)
            {
                (target as IAgent).SetInteractionContactPoint();
            }

            // TODO: Check that we should interact (via tags)


            // Resolve action
            var result = this.attackInteraction.Interact(actor, target);

            // Discard used up resource nodes
            if (target is IResourceNode && target.CurrentResourceCount == 0)
            {
                location.CurrentResources.Remove(target as IResourceNode);
            }

            // Log the action 
            if (result > 0)
            {
                actor.Species.RecordConsumptionOf(ToUnique(target), result);

                this.AddEventToAgent(actor, new TargetedEvent(location.Id, CurrentGeneration, ToUnique(target), result));
                if (target is IAgent)
                {
                    var targetAgent = target as IAgent;
                    this.AddEventToAgent(targetAgent, new TargetOfEvent(location.Id, CurrentGeneration, ToUnique(actor), result));
                }
            }
        }

        protected override void DoOneReproduction(ILocation location, List<IAgent> breeders)
        {
            var first = breeders.GetRandom();
            breeders.Remove(first);

            int reproSelector = RandomProvider.Next(100);
            if (reproSelector < 50 || breeders.Count == 0)
            {
                // Asexual
                var child = DoAsexualReproduction(first, location);
                location.Agents.Add(child);
            }
            else
            {
                // Sexual
                var second = breeders.GetRandom();
                breeders.Remove(second);

                IList<IAgent> children;
                if (reproSelector < 76)
                {
                    children = DoSexualReproduction(first, second, crossoverInteraction, location);
                }
                else
                {
                    children = DoSexualReproduction(first, second, multipointCrossoverInteraction, location);
                }

                location.Agents.AddRange(children);
            }
        }
        
        private IAgent DoAsexualReproduction(IAgent parent, ILocation location)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            if (location == null) throw new ArgumentNullException("location");
            if (parent.Cells.Count == 0) throw new ArgumentException("Agent must have at least one cell", "parent");

            if (parent.Cells.Count > 1)
            {
                return DoMulticellularAsexualReproduction(parent);
            }

            // Reproduce assuming this agent has a single cell
            var childCell = asexualReproductionInteraction.Interact(parent.Cells[0], null);
            var childAgent = new GridAgent();
            childAgent.Cells.Add(childCell);
            this.RegisterBirth(childAgent, new BirthEvent(location.Id, CurrentGeneration, asexualReproductionInteraction.GetType(), parent.Species));

            this.AddEventToAgent(parent, new ReproductionEvent(location.Id, CurrentGeneration, asexualReproductionInteraction.GetType(), null, childAgent.Species));

            return childAgent;
        }

        private IAgent DoMulticellularAsexualReproduction(IAgent parent)
        {
            throw new NotImplementedException();
        }

        private IList<IAgent> DoSexualReproduction(IAgent parent1, IAgent parent2, IInteraction<ICell, ICell, IList<ICell>> interaction, ILocation location)
        {
            if (parent1 == null) throw new ArgumentNullException("parent1");
            if (parent1.Cells.Count == 0) throw new ArgumentException("Agent must have at least one cell", "parent1");
            if (parent2 == null) throw new ArgumentNullException("parent2");
            if (parent2.Cells.Count == 0) throw new ArgumentException("Agent must have at least one cell", "parent2");
            if (interaction == null) throw new ArgumentNullException("interaction");
            if (location == null) throw new ArgumentNullException("location");

            if (parent1.Cells.Count > 1 || parent2.Cells.Count > 1)
            {
                return DoMulticellularSexualReproduction(parent1, parent2, interaction);
            }

            // Reproduce assuming both agents have a single cell
            var childCells = interaction.Interact(parent1.Cells[0], parent2.Cells[0]);

            var child1 = new GridAgent();
            child1.Cells.Add(childCells[0]);
            this.RegisterBirth(child1, new BirthEvent(location.Id, CurrentGeneration, interaction.GetType(), parent1.Species, parent2.Species));

            var child2 = new GridAgent();
            child2.Cells.Add(childCells[1]);
            this.RegisterBirth(child2, new BirthEvent(location.Id, CurrentGeneration, interaction.GetType(), parent1.Species, parent2.Species));

            // Events
            this.AddEventToAgent(parent1, new ReproductionEvent(location.Id, CurrentGeneration, interaction.GetType(), parent2.Species, child1.Species, child2.Species));
            this.AddEventToAgent(parent2, new ReproductionEvent(location.Id, CurrentGeneration, interaction.GetType(), parent1.Species, child1.Species, child2.Species));

            return new[] {child1, child2};
        }

        private IList<IAgent> DoMulticellularSexualReproduction(IAgent parent1, IAgent parent2, IInteraction<ICell, ICell, IList<ICell>> interaction)
        {
            throw new NotImplementedException();
        }

        #region IDisposable Members

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Environment.Dispose();
            }
        }

        #endregion

    }
}
