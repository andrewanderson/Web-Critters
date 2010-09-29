using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cas.Core.Events;
using Cas.Core.Extensions;
using Cas.Core.Interfaces;

namespace Cas.Core
{
    public abstract class LocationBase : ILocation
    {
        public Guid Id { get; private set; }

        protected ISimulation Simulation { get; set; }

        /// <summary>
        /// The cost, in resources, that the location levies on its agents.  Must be
        /// a positive whole number.
        /// </summary>
        public int UpkeepCost { get; set; }

        /// <summary>
        /// A list of locations that are considered connected to this location.
        /// </summary>
        public List<ILocation> Connections { get; protected set; }

        /// <summary>
        /// The individual agents that are currently living in this location.
        /// </summary>
        public List<IAgent> Agents { get; protected set; }

        /// <summary>
        /// The resources that are currently available at this location.
        /// </summary>
        public List<IResourceNode> CurrentResources { get; protected set; }

        /// <summary>
        /// The amount of resources of each type that are available at the start of every generation for this location
        /// </summary>
        public List<IResourceNode> ResourceAllocation { get; protected set; }

        protected LocationBase(ISimulation simulation)
        {
            if (simulation == null) throw new ArgumentNullException("simulation");

            this.Id = Guid.NewGuid();
            this.Simulation = simulation;
        }

        public void ConnectTo(ILocation location)
        {
            if (location == null) return;

            if (!Connections.Contains(location))
            {
                Connections.Add(location);
            }

            if (!location.Connections.Contains(this))
            {
                location.Connections.Add(this);
            }
        }

        /// <summary>
        /// Replenish all of the resources at this location
        /// </summary>
        public void RefreshResourcePool()
        {
            CurrentResources.Clear();
            CurrentResources.AddRange(ResourceAllocation.Select(x => x.DeepCopy()));
        }

        /// <summary>
        /// Remove resources equal to the UpkeepCost from all agents in this location.
        /// </summary>
        public void ChargeUpkeep(int generation)
        {
            foreach (var agent in Agents)
            {
                this.Simulation.AddEventToAgent(agent, new PayUpkeepEvent(this.Id, this.UpkeepCost, generation));
                if (agent.IsMultiAgent() || agent.Cells.Count == 0)
                {
                    // TODO: How do we extract payment if the base agent has no resources?  
                    // Need to drill into children.
                    throw new NotImplementedException();
                }
                else
                {
                    agent.Cells[0].RemoveResources(UpkeepCost);
                }
            }
        }
    }
}
