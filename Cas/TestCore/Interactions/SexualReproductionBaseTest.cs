using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Cas.Core;
using Cas.Core.Interactions;
using Cas.Core.Interfaces;
using Cas.TestImplementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCore
{
    [TestClass]
    public class SexualReproductionBaseTest
    {
        private const int TagSize = 4;

        [TestInitialize()]
        public void MyTestInitialize()
        {
            Resource.Initialize(4, 1, true);
        }
        
        [TestMethod]
        public void PayInteractionFee()
        {
            var interaction = new TestReproductionInteraction(0);

            var actor = GridCell.New(TagSize);
            actor.AddRandomResources(actor.Size*2);

            var target = GridCell.New(TagSize);
            target.AddRandomResources(target.Size * 2);
            
            var children = interaction.Interact(actor, target);

            // Verify that the actor and target have paid their fees to the children
            Assert.AreEqual(actor.Size, actor.CurrentResourceCount);
            Assert.AreEqual(target.Size, target.CurrentResourceCount);
            Assert.AreEqual(0, children[0].CurrentResourceCount);
            Assert.AreEqual(0, children[1].CurrentResourceCount);
        }

        [TestMethod]
        public void DonateToChildren()
        {
            var actor = GridCell.New();
            actor.Offense = Tag.New("abcd");
            actor.Defense = Tag.New("abcd");
            actor.Exchange = Tag.New("abcd");
            actor.AddRandomResources(actor.Size * 2);

            var target = GridCell.New();
            target.Offense = Tag.New("abcd");
            target.Defense = Tag.New("abcd");
            target.Exchange = Tag.New("abcd");
            target.AddRandomResources(target.Size * 2);

            // Reverse engineer the interaction percentage so that each child gets 2 resources
            double percentage = 2.0/actor.Size;

            var interaction = new TestReproductionInteraction(percentage);

            var children = interaction.Interact(actor, target);

            // Verify that the actor and target have paid their fees to the children
            Assert.AreEqual(actor.Size - 2, actor.CurrentResourceCount);
            Assert.AreEqual(target.Size - 2, target.CurrentResourceCount);
            Assert.AreEqual(2, children[0].CurrentResourceCount);
            Assert.AreEqual(2, children[1].CurrentResourceCount);

        }
    }

    public class TestReproductionInteraction : SexualReproductionBase
    {
        public TestReproductionInteraction(double inheritanceFactor) : base(false, inheritanceFactor) { }
        private const int TagSize = 4;

        protected override IList<ICell> Reproduce(ICell actor, ICell target)
        {
            // Just generate two random children
            var child1 = GridCell.New(TagSize);
            var child2 = GridCell.New(TagSize);

            return new List<ICell>() {child1, child2};
        }
    }
}
