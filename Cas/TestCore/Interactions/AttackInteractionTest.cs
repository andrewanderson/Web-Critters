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
    public class AttackInteractionTest
    {
        #region Set up the simulation for these tests

        private const int MatchValue = 2;
        private const int MismatchValue = -2;
        private const int WildcardValue = 0;
        private const int ExtraValue = 1;
        private const double AbsorbFactor = 1.0;
        private const int AbsorbBonus = 1;

        private static readonly AttackInteraction TestAttackInteraction = new AttackInteraction(MatchValue, MismatchValue, WildcardValue, ExtraValue, AbsorbFactor, AbsorbBonus);

        private static void AddRandomResources(IInteractable agent, int numberToAdd)
        {
            var resources = new List<Resource>();
            for (int i = 0; i < numberToAdd; i++)
            {
                resources.Add(Resource.Random(false));
            }
            agent.AddResources(resources);
        }

        private static int CalculateTransferredResources(int result)
        {
            if (result <= 0) return 0;
            else return (int) (result*AbsorbFactor) + AbsorbBonus;
        }


        [TestInitialize()]
        public void MyTestInitialize()
        {
            Resource.Initialize(4, 1);
        }

        #endregion

        [TestMethod]
        public void Interact_AttackEqualsDefense()
        {
            ICell actor = GridCell.New(false);
            actor.Offense = Tag.New("abc");
            AddRandomResources(actor, 10);

            ICell target = GridCell.New(false);
            target.Defense = Tag.New("abc");
            AddRandomResources(target, 10);

            int result = TestAttackInteraction.Interact(actor, target);

            Assert.AreEqual(MatchValue * 3, result);

            // resource check
            int transfer = CalculateTransferredResources(result);
            Assert.AreEqual(10 + transfer, actor.CurrentResourceCount);
            Assert.AreEqual(10 - transfer, target.CurrentResourceCount);
        }

        [TestMethod]
        public void Interact_CompleteMismatch()
        {
            ICell actor = GridCell.New(false);
            actor.Offense = Tag.New("abc");
            AddRandomResources(actor, 10);

            ICell target = GridCell.New(false);
            target.Defense = Tag.New("cca");
            AddRandomResources(target, 10);

            int result = TestAttackInteraction.Interact(actor, target);

            Assert.AreEqual(MismatchValue * 3, result);

            // resource check
            int transfer = CalculateTransferredResources(result);
            Assert.AreEqual(10 + transfer, actor.CurrentResourceCount);
            Assert.AreEqual(10 - transfer, target.CurrentResourceCount);
        }

        [TestMethod]
        public void Interact_WildcardAttacker()
        {
            ICell actor = GridCell.New(false);
            actor.Offense = Tag.New("###");
            AddRandomResources(actor, 10);

            ICell target = GridCell.New(false);
            target.Defense = Tag.New("cca");
            AddRandomResources(target, 10);

            int result = TestAttackInteraction.Interact(actor, target);

            Assert.AreEqual(WildcardValue * 3, result);

            // resource check
            int transfer = CalculateTransferredResources(result);
            Assert.AreEqual(10 + transfer, actor.CurrentResourceCount);
            Assert.AreEqual(10 - transfer, target.CurrentResourceCount);
        }

        [TestMethod]
        public void Interact_WildcardsCancelOut()
        {
            ICell actor = GridCell.New(false);
            actor.Offense = Tag.New("ab#c");
            AddRandomResources(actor, 10);

            ICell target = GridCell.New(false);
            target.Defense = Tag.New("ab#c");
            AddRandomResources(target, 10);

            int result = TestAttackInteraction.Interact(actor, target);

            Assert.AreEqual(MatchValue * 3, result);

            // resource check
            int transfer = CalculateTransferredResources(result);
            Assert.AreEqual(10 + transfer, actor.CurrentResourceCount);
            Assert.AreEqual(10 - transfer, target.CurrentResourceCount);
        }

        [TestMethod]
        public void Interact_LongerAttacker()
        {
            ICell actor = GridCell.New(false);
            actor.Offense = Tag.New("aa");
            AddRandomResources(actor, 10);

            ICell target = GridCell.New(false);
            target.Defense = Tag.New("a");
            AddRandomResources(target, 10);

            int result = TestAttackInteraction.Interact(actor, target);

            Assert.AreEqual(MatchValue * 1 + ExtraValue * 1, result);

            // resource check
            int transfer = CalculateTransferredResources(result);
            Assert.AreEqual(10 + transfer, actor.CurrentResourceCount);
            Assert.AreEqual(10 - transfer, target.CurrentResourceCount);
        }

        [TestMethod]
        public void Interact_LongerTarget()
        {
            ICell actor = GridCell.New(false);
            actor.Offense = Tag.New("a");
            AddRandomResources(actor, 10);

            ICell target = GridCell.New(false);
            target.Defense = Tag.New("aaa");
            AddRandomResources(target, 10);
            
            int result = TestAttackInteraction.Interact(actor, target);

            Assert.AreEqual(MatchValue * 1 - ExtraValue * 2, result);

            // resource check
            int transfer = CalculateTransferredResources(result);
            Assert.AreEqual(10 + transfer, actor.CurrentResourceCount);
            Assert.AreEqual(10 - transfer, target.CurrentResourceCount);
        }

        [TestMethod]
        public void Interact_WildcardToExtend()
        {
            ICell actor = GridCell.New(false);
            actor.Offense = Tag.New("abc#c");
            AddRandomResources(actor, 10);

            ICell target = GridCell.New(false);
            target.Defense = Tag.New("abcac");
            AddRandomResources(target, 10);

            int result = TestAttackInteraction.Interact(actor, target);

            Assert.AreEqual(MatchValue * 4 + WildcardValue * 1, result);

            // resource check
            int transfer = CalculateTransferredResources(result);
            Assert.AreEqual(10 + transfer, actor.CurrentResourceCount);
            Assert.AreEqual(10 - transfer, target.CurrentResourceCount);
        }

        [TestMethod]
        public void Interact_Kill()
        {
            ICell actor = GridCell.New(false);
            actor.Offense = Tag.New("abc");
            AddRandomResources(actor, 4);

            ICell target = GridCell.New(false);
            target.Defense = Tag.New("abc");
            AddRandomResources(target, 4);

            int result = TestAttackInteraction.Interact(actor, target);

            Assert.AreEqual(MatchValue * 3, result);

            // resource check
            Assert.AreEqual(8, actor.CurrentResourceCount);
            Assert.AreEqual(0, target.CurrentResourceCount);
        }

        [TestMethod]
        public void Interact_ResourceNode()
        {
            ICell actor = GridCell.New(false);
            actor.Offense = Tag.New("abc");
            AddRandomResources(actor, 4);

            IResourceNode target = new GridResourceNode(new List<Resource> {Resource.Random(false)});
            target.Defense = Tag.New("abc");
            AddRandomResources(target, 3);  // top the node up to 4 resources

            int result = TestAttackInteraction.Interact(actor, target);

            Assert.AreEqual(MatchValue * 3, result);

            // resource check
            Assert.AreEqual(8, actor.CurrentResourceCount);
            Assert.AreEqual(0, target.CurrentResourceCount);
        }

    }
}
