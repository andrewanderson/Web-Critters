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
            var config = new Configuration();

            // Agent settings
            config.AgentSettings.InteractionsPerGeneration = 1.5;
            config.AgentSettings.MaximumAttemptsToFindSuitableTarget = 5;
            config.AgentSettings.MaximumMigrationBonus = 5.0 / 100.0;
            config.AgentSettings.MigrationBaseChance = 0.5 / 100.0;
            config.AgentSettings.MutationChance = 0.05 / 100.0;
            config.AgentSettings.RandomDeathChance = 0.005 / 100.0;
            config.AgentSettings.ReproductionInheritance = 30 / 100.0;
            config.AgentSettings.ReproductionThreshold = 200 / 100.0;
            config.AgentSettings.StartingTagComplexity = 3;

            // Environment settings
            config.EnvironmentSettings.GlobalResourcePoolSize = 10;
            config.EnvironmentSettings.SetLocationCapacity(10, 30);
            config.EnvironmentSettings.SetLocationResourceNodeRange(3, 10);
            config.EnvironmentSettings.SetLocationResourcesPerNode(15, 40);
            config.EnvironmentSettings.MaximumUpkeepCostPerLocation = 3;
            config.EnvironmentSettings.UpkeepChance = 50 / 100.0;

            // Resource settings
            config.ResourceSettings.AllowWildcards = false;
            config.ResourceSettings.Count = ResourceCount;
            config.ResourceSettings.NormalToWildcardRatio = 3;

            // Tag settings
            config.TagSettings.MaxSize = 10;

            Cas.TestImplementation.GridSimulation testSimulation = new Cas.TestImplementation.GridSimulation(5, 6, config);
            testSimulation.Initialize();
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
                Assert.IsTrue(c >= testSimulation.Configuration.EnvironmentSettings.MinimumRenewableResourceNodes);
                Assert.IsTrue(c <= testSimulation.Configuration.EnvironmentSettings.MaximumRenewableResourceNodes);
            }

        }

    }
}
