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
        private const string TagSeparator = "^";

        /// <summary>
        /// A key that can be used to uniquely identify the agent based on its entire genome.
        /// </summary>
        public override string UniqueKey
        {
            get
            {
                if (uniqueKey == null)
                {
                    if (this.IsMultiAgent() || this.Cells.Count == 0)
                    {
                        // TODO: This needs to work for multiagents
                        throw new NotImplementedException();
                    }
                    else
                    {
                        var cell = this.Cells[0];
                        uniqueKey = string.Join(TagSeparator, cell.Offense.ToString(), cell.Defense.ToString(), cell.Exchange.ToString());
                    }
                }
                return uniqueKey;
            }
        }
        private string uniqueKey = null;

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
            string speciesId = (Species == null) ? string.Empty : string.Format("#{0}, ", this.Species.Id);
            if (!this.IsMultiAgent())
            {
                string cellString = (this.Cells.Count == 0) ? "Empty" : this.Cells[0].ToString();
                return string.Format("{0}{1} - {2} generations", speciesId, cellString, this.Age);
            }
            else
            {
                return string.Format("{0}{1} cells, {2} agents - {3} generations", speciesId, this.Cells.Count, this.Agents.Count, this.Age);
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
