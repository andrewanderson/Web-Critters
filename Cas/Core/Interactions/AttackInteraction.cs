using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cas.Core.Extensions;
using Cas.Core.Interfaces;

namespace Cas.Core.Interactions
{
    /// <summary>
    /// A basic interaction whereby one agent attempts to take resources from another.
    /// </summary>
    /// <remarks>
    /// Don't let the "attack" verb colour your understanding of this interaction. Not
    /// only could an AttackInteraction represent a forcible resource transfer, but it
    /// could also be one half of a symbiotic relationship.
    /// </remarks>
    public class AttackInteraction : LocusInteractionBase<IInteractable, IInteractable>
    {
        /// <summary>
        /// The percentage of the result differential that is taken from
        /// the target's resource pool if the actor wins the interaction.
        /// </summary>
        public double AbsorbFactor { get; private set; }

        /// <summary>
        /// The number of resources (over and above the AbsorbFactor) that are
        /// taken from the target's resource pool if the actor wins the interaction.
        /// </summary>
        public int AbsorbBonus { get; private set; }

        public AttackInteraction() : this(2, -2, 0, 1, 1.0, 1) { }

        public AttackInteraction(int match, int mismatch, int wildcard, int extra, double absorbFactor, int absorbBonus) 
            : base(match, mismatch, wildcard, extra)
        {
            AbsorbFactor = absorbFactor;
            AbsorbBonus = absorbBonus;
        }

        /// <summary>
        /// The actor attempts to attack (take resources from) the target.
        /// </summary>
        public override int Interact(IInteractable actor, IInteractable target)
        {
            if (actor == null) throw new ArgumentNullException("actor");
            if (target == null) throw new ArgumentNullException("target");

            int result = CalculateInteractionResult(actor.Offense, target.Defense);

            ApplyResult(actor, target, result);

            return result;
        }

        /// <summary>
        /// If the result is positive, transfer resources (at random) from the
        /// target to the actor.  If the result is zero or negative, do nothing.
        /// </summary>
        protected void ApplyResult(IInteractable actor, IInteractable target, int result)
        {
            if (actor == null) throw new ArgumentNullException("actor");
            if (target == null) throw new ArgumentNullException("target");

            if (result <= 0) return;

            int transferredResources = (int) (result*AbsorbFactor) + AbsorbBonus;
            var resources = target.RemoveResources(transferredResources);
            actor.AddResources(resources);
        }

    }
}
