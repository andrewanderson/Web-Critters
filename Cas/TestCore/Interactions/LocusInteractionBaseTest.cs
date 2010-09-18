using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Cas.Core.Interactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cas.Core;

namespace TestCore.Interactions
{
    [TestClass]
    public class LocusInteractionBaseTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        #region Set up the simulation for these tests

        private const int MatchValue = 2;
        private const int MismatchValue = -2;
        private const int WildcardValue = 0;
        private const int ExtraValue = 1;

        private static readonly TestLocusInteraction Interaction = new TestLocusInteraction(MatchValue, MismatchValue, WildcardValue, ExtraValue);


        [TestInitialize()]
        public void MyTestInitialize()
        {
            Resource.Initialize(4, 1);
        }

        #endregion

        [TestMethod]
        public void Interact_AttackEqualsDefense()
        {
            int result = Interaction.Interact(Tag.New("abc"), Tag.New("abc"));

            Assert.AreEqual(MatchValue * 3, result);
        }

        [TestMethod]
        public void Interact_CompleteMismatch()
        {
            int result = Interaction.Interact(Tag.New("abc"), Tag.New("cca"));

            Assert.AreEqual(MismatchValue * 3, result);
        }

        [TestMethod]
        public void Interact_WildcardAttacker()
        {
            int result = Interaction.Interact(Tag.New("###"), Tag.New("cca"));

            Assert.AreEqual(WildcardValue * 3, result);
        }

        [TestMethod]
        public void Interact_WildcardsCancelOut()
        {
            int result = Interaction.Interact(Tag.New("ab#c"), Tag.New("ab#c"));

            Assert.AreEqual(MatchValue * 3, result);
        }

        [TestMethod]
        public void Interact_LongerAttacker()
        {
            int result = Interaction.Interact(Tag.New("aa"), Tag.New("a"));

            Assert.AreEqual(MatchValue * 1 + ExtraValue * 1, result);
        }

        [TestMethod]
        public void Interact_LongerAttackerWithWildcards()
        {
            int result = Interaction.Interact(Tag.New("aa##"), Tag.New("a"));

            Assert.AreEqual(MatchValue * 1 + ExtraValue * 1 + WildcardValue * 2, result);
        }

        [TestMethod]
        public void Interact_LongerTarget()
        {
            int result = Interaction.Interact(Tag.New("a"), Tag.New("aaa"));

            Assert.AreEqual(MatchValue * 1 - ExtraValue * 2, result);
        }

        [TestMethod]
        public void Interact_WildcardToExtend()
        {
            int result = Interaction.Interact(Tag.New("abc#c"), Tag.New("abcac"));

            Assert.AreEqual(MatchValue * 4 + WildcardValue * 1, result);
        }
    }

    public class TestLocusInteraction : LocusInteractionBase<Tag,Tag>
    {
        public TestLocusInteraction(int match, int mismatch, int wildcard, int extra) : base(match, mismatch, wildcard, extra) { }

        public override int Interact(Tag actor, Tag target)
        {
            return CalculateInteractionResult(actor, target);
        }
    }
}
