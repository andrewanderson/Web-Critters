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
        /// Expressed as a number between 0.0 and 1.0.
        /// 
        /// Default: 0.15%
        /// </summary>
        private static double PointMutationChance = 0.0015;

        public static void SetMutationPercentage(double percent)
        {
            if (percent < 0 || percent > 100) throw new ArgumentOutOfRangeException("percent", "Must be between 0 and 100");

            PointMutationChance = percent / 100.0;
        }

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
            double rand = RandomProvider.NextDouble();
            bool shouldMutate = (rand < PointMutationChance);
            if (shouldMutate) Console.WriteLine("MUTATING");
            return shouldMutate;
        }

    }
}
