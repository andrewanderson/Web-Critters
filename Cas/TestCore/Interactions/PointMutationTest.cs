using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Cas.Core;
using Cas.Core.Interactions;
using Cas.Core.Interfaces;
using Cas.TestImplementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCore.Interactions
{
    [TestClass]
    public class PointMutationTest
    {
        [TestInitialize()]
        public void MyTestInitialize()
        {
            var config = new Configuration();
            config.ResourceSettings.AllowWildcards = true;
            config.ResourceSettings.Count = 10;
            config.ResourceSettings.NormalToWildcardRatio = 1;

            Resource.Initialize(config);
        }

        [TestMethod]
        public void Mutate_OneHundredRandom()
        {
            int cellCount = 100;

            var candidates = new List<ICell>();

            // Make random cells
            for (int i = 0; i < cellCount; i++)
            {
                var cell = GridCell.New(4);
                candidates.Add(cell);
            }

            Console.Out.WriteLine(string.Empty);

            // Mutate each cell
            Console.Out.WriteLine("Starting mutations:");
            for (int i = 0; i < cellCount; i++)
            {
                string orig = candidates[i].ToString();
                PointMutation.Mutate(candidates[i]);
                Console.Out.WriteLine("Cell {0}: {1} -> {2}", i, orig, candidates[i]);
            }

            Console.Out.WriteLine("Done!");
        }
    }
}
