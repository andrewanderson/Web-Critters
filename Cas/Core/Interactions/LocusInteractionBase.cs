﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cas.Core.Interfaces;

namespace Cas.Core.Interactions
{
    public abstract class LocusInteractionBase<TActor, TTarget> : IInteraction<TActor, TTarget, int>
    {
        /// <summary>
        /// Indexes to the values within the InteractionLocus array
        /// </summary>
        public enum InteractionLocusIndecies
        {
            Match = 0,
            Mismatch = 1,
            Wildcard = 2,
            Extra = 3,
        }

        /// <summary>
        /// The mapping of values that will be used to calculate the interaction score.
        /// </summary>
        protected int[] InteractionLocus { get; set; }

        protected LocusInteractionBase(int match, int mismatch, int wildcard, int extra)
        {
            InteractionLocus = new int[] {match, mismatch, wildcard, extra};
        }

        public abstract int Interact(TActor actor, TTarget target);

        /// <summary>
        /// Apply some heuristics to the participating tags to determine the outcome
        /// of the interaction.  A positive net result signifies a "victory" for the actor,
        /// while a zero or negative result signifies a successful defense.
        /// </summary>
        protected int CalculateInteractionResult(Tag actorTag, Tag targetTag)
        {
            int result = 0;

            for (int actorIndex = 0; actorIndex < actorTag.Data.Count; actorIndex++)
            {
                Resource actorData = actorTag.Data[actorIndex];

                if (actorIndex >= targetTag.Data.Count)
                {
                    result += 
                        actorData.Equals(Resource.WildcardResource)
                        ? InteractionLocus[(int)InteractionLocusIndecies.Wildcard] 
                        : InteractionLocus[(int)InteractionLocusIndecies.Extra];
                    continue;
                }

                Resource targetData = targetTag.Data[actorIndex];

                if (actorData.Equals(Resource.WildcardResource) && targetData.Equals(Resource.WildcardResource))
                {
                    // skip matching wildcards
                    continue;
                }
                else if (actorData.Equals(Resource.WildcardResource))
                {
                    result += InteractionLocus[(int)InteractionLocusIndecies.Wildcard];
                }
                else if (actorData == targetData)
                {
                    result += InteractionLocus[(int)InteractionLocusIndecies.Match];
                }
                else
                {
                    result += InteractionLocus[(int)InteractionLocusIndecies.Mismatch];
                }
            }

            // target is longer, and receives a bonus for each extra non-wildcard
            //if (targetTag.Data.Count > actorTag.Data.Count)
            //{
            //    for (int i = actorTag.Data.Count; i < targetTag.Data.Count; i++)
            //    {
            //        Resource targetData = targetTag.Data[i];
            //        result -= targetData.Equals(Resource.WildcardResource) ? 0 : InteractionLocus[(int)InteractionLocusIndecies.Extra];
            //    }
            //}

            return result;
        }
    }
}
