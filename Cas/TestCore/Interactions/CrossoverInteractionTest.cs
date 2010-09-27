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
    public class CrossoverInteractionTest
    {
        [TestInitialize()]
        public void MyTestInitialize()
        {
            Resource.Initialize(10, 1);
        }

        [TestMethod]
        public void Interact_Identical()
        {
            var interaction = new CrossoverInteraction(false, 0);

            string offenseData = "abc";
            string defenseData = "dcb";
            string exchangeData = "aba";

            var actor = GridCell.New();
            actor.Offense = Tag.New(offenseData);
            actor.Defense = Tag.New(defenseData);
            actor.Exchange = Tag.New(exchangeData);

            var target = GridCell.New();
            target.Offense = Tag.New(offenseData);
            target.Defense = Tag.New(defenseData);
            target.Exchange = Tag.New(exchangeData);

            var results = interaction.Interact(actor, target);

            Assert.AreEqual(2, results.Count);

            Assert.AreEqual(results[0].Offense.ToString(), offenseData);
            Assert.AreEqual(results[0].Defense.ToString(), defenseData);
            Assert.AreEqual(results[0].Exchange.ToString(), exchangeData);

            Assert.AreEqual(results[1].Offense.ToString(), offenseData);
            Assert.AreEqual(results[1].Defense.ToString(), defenseData);
            Assert.AreEqual(results[1].Exchange.ToString(), exchangeData);
        }

        [TestMethod]
        public void Interact_OneHundredRandom()
        {
            int cellCount = 100;

            var interaction = new CrossoverInteraction(false, 0);
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

        [TestMethod]
        public void CrossOverTags_Identical()
        {
            string tagData = "abc";

            Tag tag1 = Tag.New(tagData);
            Tag tag2 = Tag.New(tagData);

            Tag child1 = null;
            Tag child2 = null;

            CrossoverInteraction.CrossOverTags(tag1, tag2, 0, out child1, out child2);

            Assert.AreEqual(tagData, child1.ToString());
            Assert.AreEqual(tagData, child2.ToString());

            CrossoverInteraction.CrossOverTags(tag1, tag2, 1, out child1, out child2);

            Assert.AreEqual(tagData, child1.ToString());
            Assert.AreEqual(tagData, child2.ToString());

            CrossoverInteraction.CrossOverTags(tag1, tag2, 2, out child1, out child2);

            Assert.AreEqual(tagData, child1.ToString());
            Assert.AreEqual(tagData, child2.ToString());
        }

        [TestMethod]
        public void CrossOverTags_SameLength()
        {
            Tag tag1 = Tag.New("abc");
            Tag tag2 = Tag.New("def");

            Tag child1 = null;
            Tag child2 = null;

            CrossoverInteraction.CrossOverTags(tag1, tag2, 0, out child1, out child2);

            Assert.AreEqual("def", child1.ToString());
            Assert.AreEqual("abc", child2.ToString());

            CrossoverInteraction.CrossOverTags(tag1, tag2, 1, out child1, out child2);

            Assert.AreEqual("aef", child1.ToString());
            Assert.AreEqual("dbc", child2.ToString());

            CrossoverInteraction.CrossOverTags(tag1, tag2, 2, out child1, out child2);

            Assert.AreEqual("abf", child1.ToString());
            Assert.AreEqual("dec", child2.ToString());
        }

        [TestMethod]
        public void CrossOverTags_TargetShorter()
        {
            Tag tag1 = Tag.New("abcd");
            Tag tag2 = Tag.New("efg");

            Tag child1 = null;
            Tag child2 = null;

            CrossoverInteraction.CrossOverTags(tag1, tag2, 0, out child1, out child2);

            Assert.AreEqual("efg", child1.ToString());
            Assert.AreEqual("abcd", child2.ToString());

            CrossoverInteraction.CrossOverTags(tag1, tag2, 1, out child1, out child2);

            Assert.AreEqual("afg", child1.ToString());
            Assert.AreEqual("ebcd", child2.ToString());

            CrossoverInteraction.CrossOverTags(tag1, tag2, 2, out child1, out child2);

            Assert.AreEqual("abg", child1.ToString());
            Assert.AreEqual("efcd", child2.ToString());
        }

        [TestMethod]
        public void CrossOverTags_ActorShorter()
        {
            Tag tag1 = Tag.New("abc");
            Tag tag2 = Tag.New("defg");

            Tag child1 = null;
            Tag child2 = null;

            CrossoverInteraction.CrossOverTags(tag1, tag2, 0, out child1, out child2);

            Assert.AreEqual("defg", child1.ToString());
            Assert.AreEqual("abc", child2.ToString());

            CrossoverInteraction.CrossOverTags(tag1, tag2, 1, out child1, out child2);

            Assert.AreEqual("aefg", child1.ToString());
            Assert.AreEqual("dbc", child2.ToString());

            CrossoverInteraction.CrossOverTags(tag1, tag2, 2, out child1, out child2);

            Assert.AreEqual("abfg", child1.ToString());
            Assert.AreEqual("dec", child2.ToString());
        }


    }
}
