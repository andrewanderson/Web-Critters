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
 
        private readonly IInteraction<IInteractable, IInteractable, int> attackInteraction;
        private readonly IInteraction<ICell, ICell, IList<ICell>> crossoverInteraction;
        private readonly IInteraction<ICell, ICell, IList<ICell>> multipointCrossoverInteraction;
        private readonly IInteraction<ICell, ICell, ICell> asexualReproductionInteraction;

        public GridSimulation(int length, int width, Configuration config) : base(config)
        {
            Length = length;
            Width = width;

            attackInteraction = new AttackInteraction(2, -2, 0, 1, 1.0, 1);
            crossoverInteraction = new CrossoverInteraction(true, config.AgentSettings.ReproductionInheritance);
            multipointCrossoverInteraction = new MultipointCrossoverInteraction(true, config.AgentSettings.ReproductionInheritance);
            asexualReproductionInteraction = new AsexualInteraction(true, config.AgentSettings.ReproductionInheritance);
        }

        protected override void InnerInitialize()
        {
            environment = new GridEnvironment(Length, Width, this);
        }

        protected override void InnerReset()
        {
            // No-op (for now)
        }

        protected override void InnerDoInteraction(ILocation location, IAgent actor, IInteractable target)
        {
            if (location == null) throw new ArgumentNullException("location");
            if (target == null) throw new ArgumentNullException("target");

            // TODO: Pick an interaction from a list when we have more than one
            // TODO: Other interaction types

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

                this.AddEventToAgent(actor, new TargetedEvent(location, CurrentGeneration, ToUnique(target), result));
                if (target is IAgent)
                {
                    var targetAgent = target as IAgent;
                    this.AddEventToAgent(targetAgent, new TargetOfEvent(location, CurrentGeneration, ToUnique(actor), result));
                }
            }
        }
        
        protected override IAgent DoAsexualReproduction(IAgent parent, ILocation location)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            if (location == null) throw new ArgumentNullException("location");

            if (parent.IsMultiAgent())
            {
                return DoMulticellularAsexualReproduction(parent, location);
            }

            // Reproduce assuming this agent has a single cell
            var childCell = asexualReproductionInteraction.Interact(parent.Cells[0], null);
            var childAgent = new GridAgent();
            childAgent.Cells.Add(childCell);
            this.RegisterBirth(childAgent, new BirthEvent(location, CurrentGeneration, asexualReproductionInteraction.GetType(), parent.Species));

            this.AddEventToAgent(parent, new ReproductionEvent(location, CurrentGeneration, asexualReproductionInteraction.GetType(), null, childAgent.Species));

            return childAgent;
        }

        private IAgent DoMulticellularAsexualReproduction(IAgent parent, ILocation location)
        {
            throw new NotImplementedException();
        }

        protected override IList<IAgent> DoSexualReproduction(IAgent parent1, IAgent parent2, ILocation location)
        {
            if (parent1 == null) throw new ArgumentNullException("parent1");
            if (parent2 == null) throw new ArgumentNullException("parent2");
            if (location == null) throw new ArgumentNullException("location");

            int reproSelector = RandomProvider.Next(100);
            if (reproSelector < 50)
            {
                return DoSexualReproduction(parent1, parent2, crossoverInteraction, location);
            }
            else
            {
                return DoSexualReproduction(parent1, parent2, multipointCrossoverInteraction, location);
            }
        }

        private IList<IAgent> DoSexualReproduction(IAgent parent1, IAgent parent2, IInteraction<ICell, ICell, IList<ICell>> interaction, ILocation location)
        {
            if (parent1 == null) throw new ArgumentNullException("parent1");
            if (parent1.Cells.Count == 0) throw new ArgumentException("Agent must have at least one cell", "parent1");
            if (parent2 == null) throw new ArgumentNullException("parent2");
            if (parent2.Cells.Count == 0) throw new ArgumentException("Agent must have at least one cell", "parent2");
            if (interaction == null) throw new ArgumentNullException("interaction");
            if (location == null) throw new ArgumentNullException("location");

            if (parent1.IsMultiAgent() || parent2.IsMultiAgent())
            {
                return DoMulticellularSexualReproduction(parent1, parent2, interaction, location);
            }

            // Reproduce assuming both agents have a single cell
            var childCells = interaction.Interact(parent1.Cells[0], parent2.Cells[0]);

            var child1 = new GridAgent();
            child1.Cells.Add(childCells[0]);
            this.RegisterBirth(child1, new BirthEvent(location, CurrentGeneration, interaction.GetType(), parent1.Species, parent2.Species));

            var child2 = new GridAgent();
            child2.Cells.Add(childCells[1]);
            this.RegisterBirth(child2, new BirthEvent(location, CurrentGeneration, interaction.GetType(), parent1.Species, parent2.Species));

            // Events
            this.AddEventToAgent(parent1, new ReproductionEvent(location, CurrentGeneration, interaction.GetType(), parent2.Species, child1.Species, child2.Species));
            this.AddEventToAgent(parent2, new ReproductionEvent(location, CurrentGeneration, interaction.GetType(), parent1.Species, child1.Species, child2.Species));

            return new[] {child1, child2};
        }

        private IList<IAgent> DoMulticellularSexualReproduction(IAgent parent1, IAgent parent2, IInteraction<ICell, ICell, IList<ICell>> interaction, ILocation location)
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
