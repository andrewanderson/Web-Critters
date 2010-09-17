using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cas.Core.Extensions;
using Cas.Core.Interfaces;

namespace Cas.Core.Interactions
{
    /// <summary>
    /// A post-processing step for reproduction interactions.Mutation involves 
    /// randomly changing the values at one or more locations in the cell chromosomes.
    /// </summary>
    public static class PointMutation
    {
        /// <summary>
        /// The chance for any given resource node to mutate.
        /// 
        /// Expressed as a number between 0.0 and 100.0 %.
        /// </summary>
        private const double MutationChancePerResource = 0.55;           

        /// <summary>
        /// Perform a single point crossover.
        /// </summary>
        public static void Mutate(ICell cell)
        {
            if (cell == null) throw new ArgumentNullException("cell");
            
            // Iterate the tags and the resources within the tags
            for (int i = 0; i < cell.ActiveTagsInModel; i++)
            {
                // Potentially modify existing tag resources
                Tag activeTag = cell.GetTagByIndex(i);
                for (int j = 0; j < activeTag.Data.Count; j++)
                {
                    if (ShouldMutateThisPoint())
                    {
                        activeTag.Data[j] = Resource.Random(true);
                    }
                }

                // Potentially add a resource
                if (activeTag.Data.Count < Tag.MaxSize && ShouldMutateThisPoint())
                {
                    int insertionIndex = RandomProvider.Next(0, activeTag.Data.Count);
                    activeTag.Data.Insert(insertionIndex, Resource.Random(true));
                }

                // Potentially remove a resource
                if (activeTag.Data.Count > 2 && ShouldMutateThisPoint())
                {
                    activeTag.Data.RemoveRandom();
                }
            }
        }

        private static bool ShouldMutateThisPoint()
        {
            double rand = RandomProvider.NextDouble(); // (0 - 100)
            bool shouldMutate = (rand < MutationChancePerResource/100);
            //if (shouldMutate) Console.WriteLine("MUTATING");
            return shouldMutate;
        }

    }
}
