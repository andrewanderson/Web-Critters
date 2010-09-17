using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Cas.TestImplementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cas.Core;

namespace TestCore
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class SimulationBaseTest
    {
        public SimulationBaseTest() { }

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

        private static int ResourceCount = 4;

        private static GridSimulation GetTestSimulation()
        {
            Cas.TestImplementation.GridSimulation testSimulation = new Cas.TestImplementation.GridSimulation(5, 6);
            testSimulation.Initialize(ResourceCount, 1);
            return testSimulation;
        }


        [TestMethod]
        public void BasicInitializationTest()
        {
            GridSimulation testSimulation = GetTestSimulation();

            Assert.AreEqual(ResourceCount, Resource.Count); // + 1 for the # extra resource
            Assert.AreEqual(30, testSimulation.Environment.Locations.Count);
        }

        [TestMethod]
        public void ResourceMappingTest()
        {
            GridSimulation testSimulation = GetTestSimulation();

            char letter = 'a';
            for (int i = 0; i < Resource.Count; i++)
            {
                Assert.AreEqual(letter, Resource.Get(i).Label);
                letter = Convert.ToChar(Convert.ToInt32(letter) + 1);
            }
            Assert.AreEqual('#', Resource.WildcardResource.Label);
        }

        [TestMethod]
        public void ResourceAllocationTest()
        {
            GridSimulation testSimulation = GetTestSimulation();

            var resourceCounts = (from location in testSimulation.Environment.Locations
                                  select location.ResourceAllocation.Count).Distinct();

            Assert.IsTrue(resourceCounts.Count() > 0);
            foreach (int c in resourceCounts)
            {
                Assert.IsTrue(c >= GridEnvironment.MinimumResourcesGenerated);
                Assert.IsTrue(c <= GridEnvironment.MaximumResourcesGenerated);
            }

        }

    }
}
