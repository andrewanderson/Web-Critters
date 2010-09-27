using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cas.Core.Interfaces;
using Cas.Core;
using Cas.Core.Extensions;

namespace Cas.TestImplementation
{
    public class GridEnvironment : IEnvironment
    {
        /// <summary>
        /// The length of the grid
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// The width of the grid
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// The minimum number of resource nodes that will be created in each location
        /// </summary>
        public int MinResourceNodesPerLocation { get; private set; }

        /// <summary>
        /// The maximum number of resource nodes that will be created in each location
        /// </summary>
        public int MaxResourceNodesPerLocation { get; private set; }

        /// <summary>
        /// The minimum number of resources that each resource node will contain
        /// </summary>
        public int MinResourcesPerNodePerLocation { get; private set; }

        /// <summary>
        /// The maximum number of resources that each resource node will contain
        /// </summary>
        public int MaxResourcesPerNodePerLocation { get; private set; }

        /// <summary>
        /// The maximum length of new tags (cells/resources) within the simulation
        /// </summary>
        public int StartingTagComplexity { get; private set; }
        
        /// <summary>
        /// The number of unique resource nodes in the simulation.  
        /// </summary>
        /// <remarks>
        /// This sort of implies fauna diversity.
        /// </remarks>
        public int GlobalResourcePoolSize { get; private set; }

        #region IEnvironment Members

        /// <summary>
        /// The parent simulation that this environment belongs to
        /// </summary>
        public ISimulation Simulation { get; private set; }

        /// <summary>
        /// The locations that the environment currently encompasses.  
        /// </summary>
        public List<ILocation> Locations { get; private set; }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            Locations = null;
        }

        #endregion

        /// <summary>
        /// Create a new length x width sized grid.
        /// </summary>
        public GridEnvironment(int length, int width, int minResourceNodes, int maxResourceNodes, int minResourcesPerNode, int maxResourcesPerNode, 
            int tagComplexity, int uniqueResourceCount, GridSimulation simulation)
        {
            if (length <= 0) throw  new ArgumentOutOfRangeException("length", length, "Length must be greater than zero.");
            if (width <= 0) throw new ArgumentOutOfRangeException("width", width, "Width must be greater than zero.");
            if (minResourceNodes <= 0) throw new ArgumentOutOfRangeException("minResourceNodes", minResourceNodes, "minResourceNodes must be greater than zero.");
            if (maxResourceNodes <= 0) throw new ArgumentOutOfRangeException("maxResourceNodes", maxResourceNodes, "maxResourceNodes must be greater than zero.");
            if (minResourceNodes > maxResourceNodes) throw new ArgumentException("minResourceNodes cannot exceed maxResourceNodes");
            if (minResourcesPerNode <= 0) throw new ArgumentOutOfRangeException("minResourcesPerNode", minResourcesPerNode, "minResourcesPerNode must be greater than zero.");
            if (maxResourcesPerNode <= 0) throw new ArgumentOutOfRangeException("maxResourcesPerNode", maxResourcesPerNode, "maxResourcesPerNode must be greater than zero.");
            if (minResourcesPerNode > maxResourcesPerNode) throw new ArgumentException("minResourcesPerNode cannot exceed maxResourcesPerNode");
            if (tagComplexity <= 0) throw new ArgumentOutOfRangeException("tagComplexity", tagComplexity, "tagComplexity must be greater than zero.");
            if (uniqueResourceCount <= 0) throw new ArgumentOutOfRangeException("uniqueResourceCount", uniqueResourceCount, "uniqueResourceCount must be greater than zero.");
            if (simulation == null) throw new ArgumentNullException("simulation");

            Length = length;
            Width = width;
            Simulation = simulation;
            MinResourceNodesPerLocation = minResourceNodes;
            MaxResourceNodesPerLocation = maxResourceNodes;
            MinResourcesPerNodePerLocation = minResourcesPerNode;
            MaxResourcesPerNodePerLocation = maxResourcesPerNode;
            StartingTagComplexity = tagComplexity;
            GlobalResourcePoolSize = uniqueResourceCount;
        }

        /// <summary>
        /// Create and link up the grid <see cref="ILocation"/> objects.
        /// </summary>
        public void Initialize()
        {
            Locations = new List<ILocation>();

            GridLocation[,] grid = new GridLocation[Length, Width];
            for (int x = 0; x < Length; x++)
            {
                for (int y = 0; y < Width; y++)
                {
                    // Upkeep between 1 and MaximumUpkeepCostPerLocation
                    GridLocation gl = new GridLocation(x, y, AllocateRandomResources(), RandomProvider.Next(Simulation.MaximumUpkeepCostPerLocation) + 1); 
                    grid[x, y] = gl; 
                    LinkToPreviousLocations(gl, grid);
                    Locations.Add(gl);
                }
            }
        }

        /// <summary>
        /// Allocates a random assortment of ResourceNodes from a global pool of 
        /// nodes that are common for the whole environment.
        /// </summary>
        internal List<IResourceNode> AllocateRandomResources()
        {
            // TODO: It would be nice if the resources were normally distributed
            
            List<IResourceNode> nodes = new List<IResourceNode>();
            int num = RandomProvider.Next(MinResourceNodesPerLocation, MaxResourceNodesPerLocation);
            for (int i = 0; i < num; i++)
            {
                var node = GlobalResources.GetRandom().Clone() as IResourceNode;
                nodes.Add(node);
            }

            return nodes;
        }

        private List<IResourceNode> globalResources = null;
        internal List<IResourceNode> GlobalResources
        {
            get
            {
                if (globalResources == null)
                {
                    globalResources = new List<IResourceNode>();
                    for (int i = 0; i < GlobalResourcePoolSize; i++)
                    {
                        int nodeSize = RandomProvider.Next(MinResourcesPerNodePerLocation, MaxResourcesPerNodePerLocation);
                        var resources = new List<Resource>();
                        for (int j = 0; j < nodeSize; j++)
                        {
                            resources.Add(Resource.Random(false));
                        }

                        globalResources.Add(GridResourceNode.New(resources, StartingTagComplexity));
                    }

                }
                return globalResources;
            }
        }

        /// <summary>
        /// Link up the specified Grid location to any grid locations of lesser X/Y values
        /// </summary>
        private void LinkToPreviousLocations(GridLocation gl, GridLocation[,] grid)
        {
            if (gl == null) throw new ArgumentNullException("gl");
            if (grid == null) throw new ArgumentNullException("grid");

            if (gl.X > 0) WireUp(grid, gl.X - 1, gl.Y, gl);
            if (gl.X < Length - 1) WireUp(grid, gl.X + 1, gl.Y, gl);
            if (gl.Y > 0) WireUp(grid, gl.X, gl.Y - 1, gl);
            if (gl.Y > Width - 1) WireUp(grid, gl.X, gl.Y + 1, gl);
        }

        private static void WireUp(GridLocation[,] grid, int locX, int locY, GridLocation targetLocation)
        {
            if (targetLocation == null) throw new ArgumentNullException("targetLocation");
            if (grid == null) throw new ArgumentNullException("grid");

            GridLocation sourceLocation = grid[locX, locY];
            if (sourceLocation != null)
            {
                sourceLocation.ConnectTo(targetLocation);
            }
        }

        public override string ToString()
        {
            return string.Format("{0}x{1} grid", Length, Width);
        }
    }
}
