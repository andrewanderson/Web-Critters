using System;

namespace Cas.Core
{
    /// <summary>
    /// Encapsulates all of the configuration points within the simulation.
    /// </summary>
    public class Configuration
    {
        public readonly Environment EnvironmentSettings;
        public readonly Agent AgentSettings;
        public readonly Tag TagSettings;
        public readonly Resource ResourceSettings;

        public Configuration()
        {
            EnvironmentSettings = new Environment();
            AgentSettings = new Agent();
            TagSettings = new Tag();
            ResourceSettings = new Resource();
        }

        public class Environment
        {
            private int maximumUpkeepCostPerLocation;
            /// <summary>
            /// The upper bounds on the upkeep cost that a location can charge its agents.
            /// The lower bounds is always 1.
            /// </summary>
            public int MaximumUpkeepCostPerLocation
            {
                get { return maximumUpkeepCostPerLocation; }
                set
                {
                    if (value <= 1) throw new ArgumentOutOfRangeException("value", "MaximumUpkeepCostPerLocation must be greater than one");
                    maximumUpkeepCostPerLocation = value;
                }
            }

            private double upkeepChance;
            /// <summary>
            /// The percentage chance that a given location will charge its upkeep cost
            /// every generation.
            /// </summary>
            public double UpkeepChance
            {
                get { return upkeepChance; }
                set
                {
                    if (value < 0 || value > 1) throw new ArgumentOutOfRangeException("value", "UpkeepChance must be between zero and one.");
                    upkeepChance = value;
                }
            }

            private int minimumLocationCapacity;
            /// <summary>
            /// The lower bounds on a location's resource node capacity.
            /// </summary>
            public int MinimumLocationCapacity
            {
                get { return minimumLocationCapacity; }
            }

            private int maximumLocationCapacity;
            /// <summary>
            /// The upper bounds on a location's resource node capacity.
            /// </summary>
            public int MaximumLocationCapacity
            {
                get { return maximumLocationCapacity; }
            }

            public void SetLocationCapacity(int minimum, int maximum)
            {
                if (minimum <= 0) throw new ArgumentOutOfRangeException("minimum", "minimum must be greater than zero");
                if (maximum < minimum) throw new ArgumentOutOfRangeException("maximum", "maximum must be greater than or equal to minimum");

                minimumLocationCapacity = minimum;
                maximumLocationCapacity = maximum;
            }

            private int globalResourcePoolSize;
            /// <summary>
            /// The total number of distinct resources that are available for
            /// the simulation.
            /// </summary>
            public int GlobalResourcePoolSize
            {
                get
                {
                    return globalResourcePoolSize;
                }
                set
                {
                    if (value <= 0) throw new ArgumentOutOfRangeException("value", "GlobalResourcePoolSize must be greater than zero");
                    globalResourcePoolSize = value;
                }
            }

            private int minimumRenewableResourceNodes;
            /// <summary>
            /// The lower bounds on the number of renewable resource nodes that a location 
            /// will support.
            /// </summary>
            public int MinimumRenewableResourceNodes
            {
                get { return minimumRenewableResourceNodes; }
            }

            private int maximumRenewableResourceNodes;
            /// <summary>
            /// The upper bounds on the number of renewable resource nodes that a location 
            /// will support.
            /// </summary>
            public int MaximumRenewableResourceNodes
            {
                get { return maximumRenewableResourceNodes; }
            }

            public void SetLocationResourceNodeRange(int minimum, int maximum)
            {
                if (minimum <= 0) throw new ArgumentOutOfRangeException("minimum", "minimum must be greater than zero");
                if (maximum < minimum) throw new ArgumentOutOfRangeException("maximum", "maximum must be greater than or equal to minimum");

                minimumRenewableResourceNodes = minimum;
                maximumRenewableResourceNodes = maximum;
            }

            private int minimumResourcesPerNode;
            /// <summary>
            /// A lower bounds on the number of resources that a resource node will contain.
            /// </summary>
            public int MinimumResourcesPerNode
            {
                get { return minimumResourcesPerNode; }
            }

            private int maximumResourcesPerNode;
            /// <summary>
            /// A lower bounds on the number of resources that a resource node will contain.
            /// </summary>
            public int MaximumResourcesPerNode
            {
                get { return maximumResourcesPerNode; }
            }

            public void SetLocationResourcesPerNode(int minimum, int maximum)
            {
                if (minimum <= 0) throw new ArgumentOutOfRangeException("minimum", "minimum must be greater than zero");
                if (maximum < minimum) throw new ArgumentOutOfRangeException("maximum", "maximum must be greater than or equal to minimum");

                minimumResourcesPerNode = minimum;
                maximumResourcesPerNode = maximum;
            }
        }

        public class Agent
        {
            private double interactionsPerGeneration;
            /// <summary>
            /// The number of interactions that each agent in the simulation receives 
            /// per generation.  If this number is fractional then some agents will
            /// receive one extra action per generation.
            /// </summary>
            public double InteractionsPerGeneration
            {
                get { return interactionsPerGeneration; }
                set
                {
                    if (value <= 0) throw new ArgumentOutOfRangeException("value", "InteractionsPerGeneration must be greater than zero");
                    interactionsPerGeneration = value;
                }
            }  

            private double reproductionThreshold;
            /// <summary>
            /// The percentage of its resources that an agent must gather before being eligible
            /// for reproduction.
            /// </summary>
            public double ReproductionThreshold
            {
                get { return reproductionThreshold; }
                set
                {
                    if (value < 1) throw new ArgumentOutOfRangeException("value", "ReproductionThreshold must be greater than one.");
                    reproductionThreshold = value;
                }
            }

            private double reproductionInheritance;
            /// <summary>
            /// The percentage of its extra resources that an agent will bestow to its children.
            /// </summary>
            public double ReproductionInheritance
            {
                get { return reproductionInheritance; }
                set
                {
                    if (value < 0 || value > 1) throw new ArgumentOutOfRangeException("value", "ReproductionInheritance must be between zero and one.");
                    reproductionInheritance = value;
                }
            }

            private double migrationBaseChance;
            /// <summary>
            /// The basic chance that any agent has to move to a new location each generation.
            /// </summary>
            public double MigrationBaseChance
            {
                get { return migrationBaseChance; }
                set
                {
                    if (value < 0 || value > 1) throw new ArgumentOutOfRangeException("value", "MigrationBaseChance must be between zero and one.");
                    migrationBaseChance = value;
                }
            }

            private double maximumMigrationBonus;
            /// <summary>
            /// The maximum bonus chance that an agent can acquire to migrate based on its (lack of) fitness.
            /// </summary>
            public double MaximumMigrationBonus
            {
                get { return maximumMigrationBonus; }
                set
                {
                    if (value < 0 || value > 1) throw new ArgumentOutOfRangeException("value", "MaximumMigrationBonus must be between zero and one.");
                    maximumMigrationBonus = value;
                }
            }

            private double randomDeathChance;
            /// <summary>
            /// The percentage chance that an agent drops dead for no specific reason, per generation.
            /// </summary>
            public double RandomDeathChance
            {
                get { return randomDeathChance; }
                set
                {
                    if (value < 0 || value > 1) throw new ArgumentOutOfRangeException("value", "RandomDeathChance must be between zero and one.");
                    randomDeathChance = value;
                }
            }

            private int maximumAttemptsToFindSuitableTarget;
            /// <summary>
            /// The total number of times that an agent will try to find a target that matches
            /// its exchange tag during an interaction.
            /// </summary>
            public int MaximumAttemptsToFindSuitableTarget
            {
                get { return maximumAttemptsToFindSuitableTarget; }
                set
                {
                    if (value <= 0) throw new ArgumentOutOfRangeException("value", "MaximumAttemptsToFindSuitableTarget must be greater than zero.");
                    maximumAttemptsToFindSuitableTarget = value;
                }
            }

            private int startingTagComplexity;
            /// <summary>
            /// The maximum size of tags within an agent when they are created at the start 
            /// of the simulation.
            /// </summary>
            public int StartingTagComplexity
            {
                get { return startingTagComplexity; }
                set
                {
                    if (value <= 0) throw new ArgumentOutOfRangeException("value", "StartingTagComplexity must be greater than zero.");
                    startingTagComplexity = value;
                }
            }

            private double mutationChance;
            /// <summary>
            /// The chance that mutation will take place at any given point within an agent during reproduction.
            /// </summary>
            public double MutationChance
            {
                get { return mutationChance; }
                set
                {
                    if (value < 0 || value > 1) throw new ArgumentOutOfRangeException("value", "MutationChance must be between zero and one.");
                    mutationChance = value;
                }
            }
        }

        public class Tag
        {
            private int maxSize;
            /// <summary>
            /// An upper limit of the size that tags can grow to in the simulation.
            /// </summary>
            public int MaxSize
            {
                get { return maxSize; }
                set
                {
                    if (value <= 0) throw new ArgumentOutOfRangeException("value", "MaxSize must be greater than zero.");
                    maxSize = value;
                }
            }
        }
        
        public class Resource
        {
            /// <summary>
            /// Should the wildcard resource be allowed in simulation constructs?
            /// </summary>
            public bool AllowWildcards { get; set; }

            private int count;
            /// <summary>
            /// The total number of unique resources that are found in the simulation.
            /// </summary>
            public int Count
            {
                get { return count; }
                set
                {
                    if (value <= 0) throw new ArgumentOutOfRangeException("value", "Count must be greater than zero.");
                    count = value;
                }
            }

            private int normalToWildcardRatio;
            /// <summary>
            /// The relative frequency that wildcards will be selected with
            /// respect to "normal" resources when randomly generating a resource.
            /// </summary>
            public int NormalToWildcardRatio
            {
                get { return normalToWildcardRatio; }
                set
                {
                    if (value <= 0) throw new ArgumentOutOfRangeException("value", "NormalToWildcardRatio must be greater than zero.");
                    normalToWildcardRatio = value;
                }
            }
        }
    }
}
