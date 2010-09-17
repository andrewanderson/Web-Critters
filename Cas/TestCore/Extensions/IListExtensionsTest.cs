using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Cas.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCore.Extensions
{
    [TestClass]
    public class IListExtensionsTest
    {
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

        [TestMethod]
        public void RemoveRandom_Empty()
        {
            List<int> list = new List<int>();

            var result = list.RemoveRandom();

            Assert.AreEqual(default(int), result);
        }

        [TestMethod]
        public void RemoveRandom_One()
        {
            List<int> list = new List<int>() { 1 };

            var result = list.RemoveRandom();

            Assert.AreEqual(1, result);
            Assert.AreEqual(0, list.Count);
        }

        [TestMethod]
        public void RemoveRandom_Many()
        {
            List<int> list = new List<int>() { 1, 2, 3, 4, 5 };

            var result = list.RemoveRandom();

            Assert.AreEqual(4, list.Count);
            Assert.IsFalse(list.Contains(result));

            result = list.RemoveRandom();
            Assert.AreEqual(3, list.Count);
        }

    }
}
