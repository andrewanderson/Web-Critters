using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cas.Core.Interfaces;

namespace Cas.Core.Interactions
{
    /// <summary>
    /// A reproduction interaction in which two cells combine to produce two
    /// children.  A single point is chosen in each cell to perform the crossover.
    /// </summary>
    public class CrossoverInteraction : SexualReproductionBase
    {
        public CrossoverInteraction() : this(true, 0.5) { }
        public CrossoverInteraction(bool allowMutation, double inheritanceFactor) : base(allowMutation, inheritanceFactor) { }

        /// <summary>
        /// Perform a single point crossover.
        /// </summary>
        protected override IList<ICell> Reproduce(ICell actor, ICell target)
        {
            if (actor == null) throw new ArgumentNullException("actor");
            if (target == null) throw new ArgumentNullException("target");

            // Select the tag to crossover
            int crossoverIndex = RandomProvider.Next(0, actor.ActiveTagsInModel - 1);

            // Create two new cells like this (assuming crossover in tag 3): 
            //
            // AAAA AAAA AATT TTTT
            // TTTT TTTT TTAA AAAA

            ICell child1 = actor.CreateEmptyCell();
            ICell child2 = actor.CreateEmptyCell();

            for (int i = 0; i < actor.ActiveTagsInModel; i++)
            {
                if (i < crossoverIndex)
                {
                    child1.SetTagByIndex(i, actor.GetTagByIndex(i));
                    child2.SetTagByIndex(i, target.GetTagByIndex(i));
                }
                else if (i > crossoverIndex)
                {
                    child1.SetTagByIndex(i, target.GetTagByIndex(i));
                    child2.SetTagByIndex(i, actor.GetTagByIndex(i));                    
                }
                else // i == crossoverIndex
                {
                    // There are length+1 crossover points for a Tag (start, end, length-1 midpoints)
                    Tag actorTag = actor.GetTagByIndex(i);
                    Tag targetTag = target.GetTagByIndex(i);

                    Tag childTag1 = null;
                    Tag childTag2 = null;

                    CrossOverTags(actorTag, targetTag, out childTag1, out childTag2);

                    child1.SetTagByIndex(i, childTag1);
                    child2.SetTagByIndex(i, childTag2);
                }
            }

            if (AllowMutation)
            {
                PointMutation.Mutate(child1);
                PointMutation.Mutate(child1);
            }

            return new List<ICell>() {child1, child2};
        }

        /// <summary>
        /// Crossover a pair of tags to produce two new "child" tags.
        /// </summary>
        public static void CrossOverTags(Tag actorTag, Tag targetTag, out Tag childTag1, out Tag childTag2)
        {
            // There are 'length' crossover points for a Tag (start/end (same thing), length-1 midpoints)
            int crossPoint = RandomProvider.Next(0, Math.Max(actorTag.Data.Count, targetTag.Data.Count) - 1);

            CrossOverTags(actorTag, targetTag, crossPoint, out childTag1, out childTag2);
        }

        /// <summary>
        /// Crossover a pair of tags to produce two new "child" tags.
        /// </summary>
        internal static void CrossOverTags(Tag actorTag, Tag targetTag, int crossPoint, out Tag childTag1, out Tag childTag2)
        {
            StringBuilder tag1string = new StringBuilder();
            StringBuilder tag2string = new StringBuilder();

            // Copy across the actor data
            for (int j = 0; j < actorTag.Data.Count; j++)
            {
                if (j < crossPoint)
                {
                    tag1string.Append(actorTag.Data[j].Label);
                }
                else
                {
                    tag2string.Append(actorTag.Data[j].Label);
                }
            }

            // Copy across the target data
            for (int j = 0; j < targetTag.Data.Count; j++)
            {
                if (j < crossPoint)
                {
                    tag2string.Insert(j, targetTag.Data[j].Label);
                }
                else
                {
                    tag1string.Append(targetTag.Data[j].Label);
                }
            }

            childTag1 = Tag.New(tag1string.ToString());
            childTag2 = Tag.New(tag2string.ToString());
        }
    }
}
