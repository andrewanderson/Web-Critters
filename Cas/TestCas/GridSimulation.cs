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
        private int StartingTagComplexity { get; set; }
        private int UniqueResourceCount { get; set; }

        private readonly IInteraction<IInteractable, IInteractable, int> attackInteraction;
        private readonly IInteraction<ICell, ICell, IList<ICell>> crossoverInteraction;
        private readonly IInteraction<ICell, ICell, IList<ICell>> multipointCrossoverInteraction;
        private readonly IInteraction<ICell, ICell, ICell> asexualReproductionInteraction;

        public GridSimulation(int length, int width, int minResourceNodes, int maxResourceNodes, int minResourcesPerNode, int maxResourcesPerNode, int tagComplexity) 
            : this(length, width, minResourceNodes, maxResourceNodes, minResourcesPerNode, maxResourcesPerNode, tagComplexity, 100, 4, 0.25, 1.5, 1.75, 0.2, 0.005, 0.02, 0.005) { }

        public GridSimulation(int length, int width, int minResourceNodes, int maxResourceNodes, int minResourcesPerNode, int maxResourcesPerNode, int tagComplexity,
                              int uniqueResourceCount, int maximumUpkeepCostPerLocation, double upkeepChance, double interactionsPerGeneration, 
                              double reproductionThreshold, double reproductionInheritance, double migrationBaseChance, double maxMigrationBonus, double randomDeathChance)
            : base(interactionsPerGeneration, maximumUpkeepCostPerLocation, upkeepChance, reproductionThreshold, reproductionInheritance, 
                   migrationBaseChance, maxMigrationBonus, randomDeathChance)
        {
            Length = length;
            Width = width;
            MinResourceNodesPerLocation = minResourceNodes;
            MaxResourceNodesPerLocation = maxResourceNodes;
            MinResourcesPerNodePerLocation = minResourcesPerNode;
            MaxResourcesPerNodePerLocation = maxResourcesPerNode;
            StartingTagComplexity = tagComplexity;
            UniqueResourceCount = uniqueResourceCount;

            attackInteraction = new AttackInteraction();
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
            // TODO: How about migration?
            // TODO: Other interaction types

            // Pick a target
            var target = SelectRandomTarget(targets, actor);
            if (target is IAgent)
            {
                (target as IAgent).SetInteractionContactPoint();
            }

            // TODO: Check that we should interact (via tags)

            // TODO: log if target evaded

            // Resolve action
            var result = this.attackInteraction.Interact(actor, target);

            // Discard used up resource nodes
            if (target is IResourceNode && target.CurrentResourceCount == 0)
            {
                location.CurrentResources.Remove(target as IResourceNode);
            }

            // Log the action 
            // TODO: For now we only log successful attacks due to history bloat
            if (result > 0)
            {
                actor.History.Add(new TargetedEvent(actor.Id, location.Id, CurrentGeneration, target.Id, target.GetType(), result.ToString()));
                if (target is IAgent)
                {
                    var targetAgent = target as IAgent;
                    targetAgent.History.Add(new TargetOfEvent(targetAgent.Id, location.Id, CurrentGeneration, actor.Id, actor.GetType(), result.ToString()));
                }
            }
        }

        protected override void DoOneReproduction(ILocation location, List<IAgent> breeders)
        {
            var first = breeders.GetRandom();
            breeders.Remove(first);

            int reproSelector = RandomProvider.Next(100);
            if (reproSelector < 33 || breeders.Count == 0)
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
                if (reproSelector < 66)
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

            parent.History.Add(new ReproductionEvent(parent.Id, location.Id, CurrentGeneration, asexualReproductionInteraction.GetType(), Guid.Empty, childAgent.Id));
            childAgent.History.Add(new BirthEvent(childAgent.Id, location.Id, CurrentGeneration, asexualReproductionInteraction.GetType(), parent.Id));

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

            var child2 = new GridAgent();
            child2.Cells.Add(childCells[1]);

            // Events
            parent1.History.Add(new ReproductionEvent(parent1.Id, location.Id, CurrentGeneration, interaction.GetType(), Guid.Empty, child1.Id, child2.Id));
            parent2.History.Add(new ReproductionEvent(parent2.Id, location.Id, CurrentGeneration, interaction.GetType(), Guid.Empty, child1.Id, child2.Id));
            child1.History.Add(new BirthEvent(child1.Id, location.Id, CurrentGeneration, interaction.GetType(), parent1.Id, parent2.Id));
            child2.History.Add(new BirthEvent(child2.Id, location.Id, CurrentGeneration, interaction.GetType(), parent1.Id, parent2.Id));

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
