using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Cas.Core;
using Cas.Core.Interactions;
using Cas.TestImplementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCore.Interactions
{
    [TestClass]
    public class AsexualInteractionTest
    {
        private const int TagSize = 4;

        [TestInitialize()]
        public void MyTestInitialize()
        {
            Resource.Initialize(4, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void EnforceNull()
        {
            var interaction = new AsexualInteraction(false, 0);

            var actor = GridCell.New(TagSize);
            actor.AddRandomResources(actor.Size * 2);

            interaction.Interact(actor, actor);

            Assert.Fail("Expected an exception");
        }

        [TestMethod]
        public void NoInheritanceNoMutation()
        {
            var interaction = new AsexualInteraction(false, 0);

            var actor = GridCell.New(TagSize);
            actor.AddRandomResources(actor.Size * 2);

            var child = interaction.Interact(actor, null);

            Assert.AreEqual(actor.Size, actor.CurrentResourceCount);
            Assert.AreEqual(child.Size, actor.Size);
        }

        [TestMethod]
        public void ControlledInheritanceNoMutation()
        {
            var interaction = new AsexualInteraction(false, 0);

            var actor = GridCell.New(TagSize);
            actor.AddRandomResources(actor.Size * 2);

            // Reverse engineer the interaction percentage so that each child gets 2 resources
            double percentage = 2.0 / actor.Size;
            interaction.InheritanceFactor = percentage;

            var child = interaction.Interact(actor, null);

            Assert.AreEqual(actor.Size - 2, actor.CurrentResourceCount);
            Assert.AreEqual(2, child.CurrentResourceCount);
            Assert.AreEqual(child.Size, actor.Size);
        }

        [TestMethod]
        public void Mutation()
        {
            var interaction = new AsexualInteraction(true, 0);

            for (int i = 0; i < 100; i++)
            {
                var parent = GridCell.New(TagSize);
                parent.AddRandomResources(parent.Size * 2);

                var child = interaction.Interact(parent, null);

                Console.Out.WriteLine("{0}: Asexual reproduction by {1} resulted in {2}", i,parent, child);
            }
        }
    }
}
