using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cas.Core.Interfaces;

namespace Cas.Core.Interactions
{
    /// <summary>
    /// A reproduction interaction in which two cells combine to produce two
    /// children.  Multiple points is chosen within each cell to perform the crossover.
    /// </summary>
    public class MultipointCrossoverInteraction : SexualReproductionBase
    {
        /// <summary>
        /// Factor to multiply the base crossover percentage by when 
        /// determining how frequently tags are crossed.
        /// 
        /// Must be a positive number.
        /// </summary>
        protected const double CrossoverFactor = 1.1;

        public MultipointCrossoverInteraction() : this(true, 0.5) { }
        public MultipointCrossoverInteraction(bool allowMutation, double inheritanceFactor) : base(allowMutation, inheritanceFactor) { }

        /// <summary>
        /// Perform a multi-point crossover.
        /// </summary>
        protected override IList<ICell> Reproduce(ICell actor, ICell target)
        {
            if (actor == null) throw new ArgumentNullException("actor");
            if (target == null) throw new ArgumentNullException("target");

            // Percentage of each tag crossing over is normalized to produce one
            // cross on average by default.  This is modified by the CrossoverFactor.  
            int crossoverPercent = (int) (100 / actor.ActiveTagsInModel * CrossoverFactor);

            ICell child1 = actor.CreateEmptyCell();
            ICell child2 = actor.CreateEmptyCell();

            for (int i = 0; i < actor.ActiveTagsInModel; i++)
            {
                if (RandomProvider.Next(100) < crossoverPercent)
                {
                    Tag actorTag = actor.GetTagByIndex(i);
                    Tag targetTag = target.GetTagByIndex(i);

                    Tag childTag1 = null;
                    Tag childTag2 = null;

                    CrossoverInteraction.CrossOverTags(actorTag, targetTag, out childTag1, out childTag2);

                    child1.SetTagByIndex(i, childTag1);
                    child2.SetTagByIndex(i, childTag2);
                }
                else
                {
                    child1.SetTagByIndex(i, Tag.New(actor.GetTagByIndex(i)));
                    child2.SetTagByIndex(i, Tag.New(target.GetTagByIndex(i)));
                }
            }
            
            if (AllowMutation)
            {
                PointMutation.Mutate(child1);
                PointMutation.Mutate(child1);
            }

            return new List<ICell>() { child1, child2 };
        }


    }
}
