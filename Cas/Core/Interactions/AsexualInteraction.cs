using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cas.Core.Interfaces;

namespace Cas.Core.Interactions
{
    /// <summary>
    /// An interaction whereby a single cell clones itself, and the offspring potentially mutates.
    /// </summary>
    public class AsexualInteraction : ReproductionInteractionBase, IInteraction<ICell, ICell, ICell>
    {
        public AsexualInteraction(bool allowMutation, double inheritanceFactor) : base(allowMutation, inheritanceFactor) { }
        
        public ICell Interact(ICell actor, ICell target)
        {
            if (actor == null) throw new ArgumentNullException("actor");
            if (target != null) throw new ArgumentException("target must be null for an asexual interaction", "target");

            // Clone the parent
            var child = actor.CreateEmptyCell();
            for (int i = 0; i < actor.ActiveTagsInModel; i++)
            {
                child.SetTagByIndex(i, actor.GetTagByIndex(i));
            }

            PayInteractionFee(actor);

            TransferResourcesToChild(actor, child);

            if (AllowMutation)
            {
                PointMutation.Mutate(child);
            }

            return child;
        }

        protected void TransferResourcesToChild(ICell parent, ICell child)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            if (child == null) throw new ArgumentNullException("child");

            // Remove from parents
            int transferCount = (int) (parent.CurrentResourceCount*this.InheritanceFactor);
            var resourcePool = parent.RemoveResources(transferCount);

            // Add to children
            child.AddResources(resourcePool);
        }
    }
}
