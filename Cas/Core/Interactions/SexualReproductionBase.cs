using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cas.Core.Interfaces;

namespace Cas.Core.Interactions
{
    public abstract class SexualReproductionBase : ReproductionInteractionBase, IInteraction<ICell, ICell, IList<ICell>>
    {
        protected SexualReproductionBase(bool allowMutation, double inheritanceFactor) : base(allowMutation, inheritanceFactor) { }

        public IList<ICell> Interact(ICell actor, ICell target)
        {
            if (actor == null) throw new ArgumentNullException("actor");
            if (target == null) throw new ArgumentNullException("target");

            var children = Reproduce(actor, target);

            PayInteractionFee(actor);
            PayInteractionFee(target);

            TransferResourcesToChildren(actor, target, children[0], children[1]);

            return children;
        }

        /// <summary>
        /// Produce two child cells from a set of parents.
        /// </summary>
        protected abstract IList<ICell> Reproduce(ICell actor, ICell target);

        /// <summary>
        /// The two parent cells contribute resources to their children 
        /// according to the InheritanceFactor.
        /// </summary>
        protected void TransferResourcesToChildren(ICell parent1, ICell parent2, ICell child1, ICell child2)
        {
            if (parent1 == null) throw new ArgumentNullException("parent1");
            if (parent2 == null) throw new ArgumentNullException("parent2");
            if (child1 == null) throw new ArgumentNullException("child1");
            if (child2 == null) throw new ArgumentNullException("child2");

            var resourcePool = new List<Resource>();

            // Remove from parents
            int transferCount = (int)(parent1.CurrentResourceCount * this.InheritanceFactor);
            resourcePool.AddRange(parent1.RemoveResources(transferCount));

            transferCount = (int)(parent2.CurrentResourceCount * this.InheritanceFactor);
            resourcePool.AddRange(parent2.RemoveResources(transferCount));

            // Add half the resources to each child
            child1.AddResources(resourcePool.Where((r, i) => i % 2 == 0).ToList());
            child2.AddResources(resourcePool.Where((r, i) => i % 2 == 1).ToList());
        }

    }
}
