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
    public class ExchangeInteractionTest
    {
        [TestInitialize()]
        public void MyTestInitialize()
        {
            Resource.Initialize(4, 1, true);
        }

        [TestMethod]
        public void Interact_MatchExact()
        {
            var interaction = new ExchangeInteraction(0);

            ICell actor = GridCell.New();
            actor.Offense = Tag.New("cba");
            actor.Exchange = Tag.New("abc");

            ICell target = GridCell.New();
            target.Offense = Tag.New("abc");
            target.Exchange = Tag.New("cba");

            bool result = interaction.Interact(actor, target);

            Assert.AreEqual(true,result);
        }

        [TestMethod]
        public void Interact_MatchExtraChars()
        {
            var interaction = new ExchangeInteraction(0);

            ICell actor = GridCell.New();
            actor.Offense = Tag.New("cbadd");
            actor.Exchange = Tag.New("abcdd");

            ICell target = GridCell.New();
            target.Offense = Tag.New("abc");
            target.Exchange = Tag.New("cba");

            bool result = interaction.Interact(actor, target);

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void Interact_MatchWildcards()
        {
            var interaction = new ExchangeInteraction(0);

            ICell actor = GridCell.New();
            actor.Offense = Tag.New("c##dd");
            actor.Exchange = Tag.New("abc");

            ICell target = GridCell.New();
            target.Offense = Tag.New("ab#");
            target.Exchange = Tag.New("cba");

            bool result = interaction.Interact(actor, target);

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void Interact_CompleteMismatch()
        {
            var interaction = new ExchangeInteraction(0);

            ICell actor = GridCell.New();
            actor.Offense = Tag.New("c#a#");
            actor.Exchange = Tag.New("abc");

            ICell target = GridCell.New();
            target.Offense = Tag.New("db#");
            target.Exchange = Tag.New("cba");

            bool result = interaction.Interact(actor, target);

            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void Interact_PartialMatchFlee()
        {
            var interaction = new ExchangeInteraction(0);

            ICell actor = GridCell.New();
            actor.Offense = Tag.New("cba");
            actor.Exchange = Tag.New("abc");

            ICell target = GridCell.New();
            target.Offense = Tag.New("dbc");
            target.Exchange = Tag.New("cba");

            bool result = interaction.Interact(actor, target);

            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void Interact_PartialMatchStay()
        {
            var interaction = new ExchangeInteraction(100);

            ICell actor = GridCell.New();
            actor.Offense = Tag.New("cba");
            actor.Exchange = Tag.New("abc");

            ICell target = GridCell.New();
            target.Offense = Tag.New("dbc");
            target.Exchange = Tag.New("cba");

            bool result = interaction.Interact(actor, target);

            Assert.AreEqual(true, result);
        }

    }
}
