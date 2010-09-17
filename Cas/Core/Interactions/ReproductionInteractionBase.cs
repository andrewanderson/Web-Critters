using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cas.Core.Extensions;
using Cas.Core.Interfaces;

namespace Cas.Core.Interactions
{
    public abstract class ReproductionInteractionBase
    {
        /// <summary>
        /// Should the children of the interaction be mutated?
        /// </summary>
        public bool AllowMutation { get; set; }

        /// <summary>
        /// The percentage of their outstanding resources that parents
        /// donate to newly created children
        /// </summary>
        public double InheritanceFactor { get; set; }

        protected ReproductionInteractionBase(bool allowMutation, double inheritanceFactor)
        {
            AllowMutation = allowMutation;
            InheritanceFactor = inheritanceFactor;
        }

        /// <summary>
        /// The target cell loses resources equal to the size of its chromosome.
        /// This simulates contributing genetic material to the children of the
        /// interaction.
        /// </summary>
        protected void PayInteractionFee(ICell cell)
        {
            if (cell == null) throw new ArgumentNullException("cell");

            cell.RemoveResources(cell.Size);
        }
    }
}
