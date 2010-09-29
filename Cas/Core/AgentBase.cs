using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cas.Core.Extensions;
using Cas.Core.Interfaces;
using Cas.Core.Events;

namespace Cas.Core
{
    public abstract class AgentBase : IAgent
    {
        protected readonly List<IAgent> agents;
        protected readonly List<ICell> cells;
        protected readonly List<IResourceNode> currentResourceNodes;
        protected readonly List<Resource> newResources;
        protected ICell currentInteractionContactPoint;
        protected readonly List<IEvent> history;
        private long age;

        protected AgentBase()
        {
            agents = new List<IAgent>();
            cells = new List<ICell>();
            currentResourceNodes = new List<IResourceNode>();
            newResources = new List<Resource>();
            history = new List<IEvent>();
            age = 0;
        }

        /// <summary>
        /// The species to which this agent is a member.
        /// </summary>
        public ISpecies Species { get; set; }

        /// <summary>
        /// A key that can be used to uniquely identify the agent based on its entire genome.
        /// </summary>
        public abstract string UniqueKey { get; }

        public List<ICell> Cells
        {
            get
            {
                return cells;
            }
        }

        private void EnsureContactPointSet()
        {
            if (currentInteractionContactPoint == null) throw new InvalidOperationException("Call SetInteractionContactPoint prior to accessing this property.");
        }

        public void SetInteractionContactPoint()
        {
            currentInteractionContactPoint = Cells.GetRandom();
        }

        /// <summary>
        /// Retrieves the total size of the agent's genome, including all sub-agents/cells.
        /// </summary>
        public int Size
        {
            get
            {
                return this.Cells.Sum(cell => cell.Size) + this.Agents.Sum(agent => agent.Size);
            }
        }

        /// <summary>
        /// The number of generations that an agent has existed for.
        /// </summary>
        public long Age
        {
            get
            {
                return this.age;
            }
        }

        /// <summary>
        /// Adds a tick to the agent's age.
        /// </summary>
        public void IncrementAge()
        {
            this.age++;
        }

        public List<IEvent> History
        {
            get
            {
                return this.history;
            }
        }

        public abstract IAgent DeepCopy();

        #region IBoundary implementation

        /// <summary>
        /// The sub-Agents of this agent.
        /// </summary>
        public List<IAgent> Agents
        {
            get
            {
                return agents;
            }
        }

        /// <summary>
        /// The resources that are common to this agent, and have
        /// been present since the start of the generation.
        /// </summary>
        public List<IResourceNode> CurrentResources
        {
            get 
            {
                return currentResourceNodes;
            }
        }

        #endregion

        #region IIsAlive Members

        public bool CanReplicate(double reproductionThreshold)
        {
            return cells.All(cell => cell.CanReplicate(reproductionThreshold))
                && agents.All(agent => agent.CanReplicate(reproductionThreshold));
        }

        public abstract bool IsEligableForDeath { get; }

        #endregion

        #region IContainsResources Members

        public int CurrentResourceCount
        {
            get
            {
                return this.Cells.Sum(cell => cell.CurrentResourceCount) + this.Agents.Sum(agent => agent.CurrentResourceCount);
            }
        }

        public List<Resource> RemoveResources(int count)
        {
            if (count < 0) throw new ArgumentOutOfRangeException("count");
            EnsureContactPointSet();

            return currentInteractionContactPoint.RemoveResources(count);
        }

        public void AddResources(List<Resource> resources)
        {
            if (resources == null) throw new ArgumentNullException("resources");
            EnsureContactPointSet();

            currentInteractionContactPoint.AddResources(resources);
        }

        public string ShowResourcePool(string delimiter)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IInteractable implementation

        public Tag Offense
        {
            get 
            {
                EnsureContactPointSet();
                return currentInteractionContactPoint.Offense;
            }
            set { throw new InvalidOperationException("Cannot set an Agent's tag."); }
        }

        public Tag Defense
        {
            get
            {
                EnsureContactPointSet();
                return currentInteractionContactPoint.Defense;
            }
            set { throw new InvalidOperationException("Cannot set an Agent's tag."); }
        }

        public Tag Exchange
        {
            get
            {
                EnsureContactPointSet();
                return currentInteractionContactPoint.Exchange;
            }
            set { throw new InvalidOperationException("Cannot set an Agent's tag."); }
        }

        public bool IsMultiAgent()
        {
            return (this.Agents.Count > 1 || this.Cells.Count > 1);
        }

        #endregion
    }
}
