using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Cas.Core;
using Cas.TestImplementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCore
{
    [TestClass]
    public class TagTest
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
      
        [TestInitialize()]
        public void MyTestInitialize()
        {
            Resource.Initialize(4, 1);
        }
        
        [TestMethod]
        public void ConstructorTest()
        {
            string tagString1 = "aaac";
            string tagString2 = "#b#d";
            string tagString3 = "abcd";
            string tagString4 = "dd#c";

            Tag tag1 = Tag.New(tagString1);
            Assert.AreEqual(tagString1, tag1.ToString());

            Tag tag2 = Tag.New(tagString2);
            Assert.AreEqual(tagString2, tag2.ToString());

            Tag tag3 = Tag.New(tagString3);
            Assert.AreEqual(tagString3, tag3.ToString());

            Tag tag4 = Tag.New(tagString4);
            Assert.AreEqual(tagString4, tag4.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void ConstructorOutOfRangeTest()
        {
            string tagString1 = "e";

            Tag tag1 = Tag.New(tagString1);
        }

        [TestMethod]
        public void NewTest()
        {
            // Verify that we can generate new Tags of varying size without error.
            for (int i = 0; i < 100; i++)
            {
                Tag t = Tag.New();
                Console.WriteLine("Tag: " + t);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TagExceedsMaxSize()
        {
            string tagString = "abababababab";

            Tag tag = Tag.New(tagString);
        }

    }
}
