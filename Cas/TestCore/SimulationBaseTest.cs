using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Cas.TestImplementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cas.Core;

namespace TestCore
{
    [TestClass]
    public class SimulationBaseTest
    {
        private static int ResourceCount = 4;

        private static GridSimulation GetTestSimulation()
        {
            Cas.TestImplementation.GridSimulation testSimulation = new Cas.TestImplementation.GridSimulation(5, 6, 2, 25, 10, 50, 1, 4, 4);
            testSimulation.Initialize(ResourceCount, true, 1, 10, 0.55);
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
                Assert.IsTrue(c >= (testSimulation.Environment as GridEnvironment).MinResourceNodesPerLocation);
                Assert.IsTrue(c <= (testSimulation.Environment as GridEnvironment).MaxResourceNodesPerLocation);
            }

        }

    }
}
