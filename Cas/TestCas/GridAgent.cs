using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cas.Core;
using Cas.Core.Interfaces;

namespace Cas.TestImplementation
{
    public class GridAgent : AgentBase
    {
        /// <summary>
        /// An agent should die if any of its composite parts (cells, sub-agents) 
        /// are eligible for death.
        /// </summary>
        public override bool IsEligableForDeath
        {
            get
            {
                return 
                    this.Cells.Any(cell => cell.CurrentResourceCount == 0) 
                    || 
                    this.Agents.Any(agent => agent.IsEligableForDeath);
            }
        }

        public override string ToString()
        {
            if (!this.IsMultiAgent())
            {
                string cellString = (this.Cells.Count == 0) ? "Empty" : this.Cells[0].ToString();
                return string.Format("{0} - {1} generations", cellString, this.Age);
            }
            else
            {
                return string.Format("Multiagent: {0} cells, {1} agents - {2} generations", this.Cells.Count, this.Agents.Count, this.Age);
            }
        }

        /// <summary>
        /// Copies the structure, but not the state, of an agent.
        /// </summary>
        public override IAgent DeepCopy()
        {
            var clone = new GridAgent();

            foreach (var agent in this.Agents)
            {
                clone.Agents.Add(agent.DeepCopy());
            }

            foreach (var cell in this.Cells)
            {
                clone.Cells.Add(cell.DeepCopy());
            }

            return clone;
        }
    }
}
