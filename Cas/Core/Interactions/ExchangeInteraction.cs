using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cas.Core.Interfaces;

namespace Cas.Core.Interactions
{
    /// <summary>
    /// An interaction that is used to determine if further interactions take place.
    /// 
    /// The Actor's exchange tag is compared to the target's offense and vice versa.
    /// - If both match an interaction takes place
    /// - If neither match, no interaction occurs
    /// - If one matches, there is a probability of an interaction occurring.
    /// </summary>
    public class ExchangeInteraction : IInteraction<IInteractable, IInteractable, bool>
    {
        /// <summary>
        /// The percent chance an interaction will occur if the agents are
        /// mismatched.  0-100.
        /// </summary>
        public int MismatchInteractionProbability { get; private set; }

        private const int DefaultMismatchInteractionProbability = 50;

        public ExchangeInteraction(): this(DefaultMismatchInteractionProbability) { }

        public ExchangeInteraction(int mismatchInteractionProbability)
        {
            MismatchInteractionProbability = mismatchInteractionProbability;
        }

        /// <summary>
        /// Perform the interaction, returning whether or not a
        /// further interaction should take place based on the results.
        /// </summary>
        public bool Interact(IInteractable actor, IInteractable target)
        {
            if (actor == null) throw new ArgumentNullException("actor");
            if (target == null) throw new ArgumentNullException("target");

            bool actorMatch = IsMatch(actor, target);
            bool targetMatch = IsMatch(target, actor);        

            if (actorMatch && targetMatch)
            {
                return true;
            }
            else if (!(actorMatch || targetMatch))
            {
                return false;
            }
            else
            {
                int rand = RandomProvider.Next(100);
                return (rand < MismatchInteractionProbability);
            }
        }

        /// <summary>
        /// Determine if 'a' matches 'b'.  A match in the sense of this interaction is
        /// determined by ensuring that the Resources in a's Exchange string match exactly
        /// the Resources in b's Offense string, disregarding wildcards.  
        /// 
        /// Both a and b's Exchange string are assumed to contain infinite wildcards at the end.
        /// </summary>
        private static bool IsMatch(IInteractable a, IInteractable b)
        {
            if (a == null) throw new ArgumentNullException("a");
            if (b == null) throw new ArgumentNullException("b");

            for(int i = 0; (i < a.Exchange.Data.Count && i < b.Offense.Data.Count); i++)
            {
                var aResource = a.Exchange.Data[i];
                var bResource = b.Offense.Data[i];
                if (!(aResource.Equals(Resource.WildcardResource) || bResource.Equals(Resource.WildcardResource) || aResource.Equals(bResource)))
                {
                    return false;
                }
            }

            return true;
        }

    }
}
