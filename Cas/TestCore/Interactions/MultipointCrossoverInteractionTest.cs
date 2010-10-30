using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cas.Core;
using Cas.Core.Interactions;
using Cas.TestImplementation;
using Cas.Core.Interfaces;

namespace TestCore.Interactions
{
    [TestClass]
    public class MultipointCrossoverInteractionTest
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
        public void Interact_Identical()
        {
            var interaction = new MultipointCrossoverInteraction(false, 0);

            string offenseData = "abc";
            string defenseData = "dcb";
            string exchangeData = "cca";
            string matingData = "dca";

            var actor = GridCell.New();
            actor.Offense = Tag.New(offenseData);
            actor.Defense = Tag.New(defenseData);
            actor.Exchange = Tag.New(exchangeData);
            actor.Mating = Tag.New(matingData);

            var target = GridCell.New();
            target.Offense = Tag.New(offenseData);
            target.Defense = Tag.New(defenseData);
            target.Exchange = Tag.New(exchangeData);
            target.Mating = Tag.New(matingData);

            var results = interaction.Interact(actor, target);

            Assert.AreEqual(2, results.Count);

            Assert.AreEqual(results[0].Offense.ToString(), offenseData);
            Assert.AreEqual(results[0].Defense.ToString(), defenseData);
            Assert.AreEqual(results[0].Exchange.ToString(), exchangeData);
            Assert.AreEqual(results[0].Mating.ToString(), matingData);

            Assert.AreEqual(results[1].Offense.ToString(), offenseData);
            Assert.AreEqual(results[1].Defense.ToString(), defenseData);
            Assert.AreEqual(results[1].Exchange.ToString(), exchangeData);
            Assert.AreEqual(results[1].Mating.ToString(), matingData);
        }

        [TestMethod]
        public void Interact_OneHundredRandom()
        {
            int cellCount = 100;

            var interaction = new MultipointCrossoverInteraction(false, 0);
            var candidates = new List<ICell>();

            // Make random cells
            Console.Out.WriteLine("Cells in this test run:");
            for (int i = 0; i < cellCount; i++)
            {
                var cell = GridCell.New(4);
                candidates.Add(cell);

                Console.Out.WriteLine("Cell {0}: {1}", i, cell);
            }

            Console.Out.WriteLine(string.Empty);

            // Crossover every cell with every other cell
            int crossCount = 0;
            Console.Out.WriteLine("Starting crossovers:");
            for (int i = 0; i < cellCount; i++)
            {
                for (int j = i + 1; j < cellCount; j++)
                {
                    Console.Out.WriteLine("{4}: Crossing cell {0} with cell {1} ({2} x {3})", i, j, candidates[i], candidates[j], ++crossCount);

                    var results = interaction.Interact(candidates[i], candidates[j]);

                    Console.Out.WriteLine("Result: {0} and {1}", results[0], results[1]);
                    Console.Out.WriteLine(string.Empty);
                }
            }

            Console.Out.WriteLine("Done!");
        }

    }
}
