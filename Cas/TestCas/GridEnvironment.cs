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

        #region IEnvironment Members

        /// <summary>
        /// The parent simulation that this environment belongs to
        /// </summary>
        public ISimulation Simulation { get; private set; }

        /// <summary>
        /// The locations that the environment currently encompasses.  
        /// </summary>
        public List<ILocation> Locations { get; private set; }

        public IResourceNode FindResourceNodeById(long id)
        {
            return this.GlobalResources.Where(rn => rn.Id == id).FirstOrDefault();
        }   

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
        public GridEnvironment(int length, int width, GridSimulation simulation)
        {
            if (length <= 0) throw  new ArgumentOutOfRangeException("length", length, "Length must be greater than zero.");
            if (width <= 0) throw new ArgumentOutOfRangeException("width", width, "Width must be greater than zero.");
            if (simulation == null) throw new ArgumentNullException("simulation");

            Length = length;
            Width = width;
            Simulation = simulation;
        }

        /// <summary>
        /// Create and link up the grid <see cref="ILocation"/> objects.
        /// </summary>
        public void Initialize()
        {
            Locations = new List<ILocation>();

            GridLocation[,] grid = new GridLocation[Length, Width];
            var settings = Simulation.Configuration.EnvironmentSettings;
            for (int x = 0; x < Length; x++)
            {
                for (int y = 0; y < Width; y++)
                {
                    int upkeep = RandomProvider.Next(Simulation.Configuration.EnvironmentSettings.MaximumUpkeepCostPerLocation) + 1;
                    int capacity =
                        RandomProvider.Next(settings.MaximumLocationCapacity - settings.MinimumLocationCapacity) + settings.MinimumLocationCapacity;

                    GridLocation gl = new GridLocation(x, y, this.Simulation, AllocateRandomResources(), upkeep, capacity);
                    gl.RefreshResourcePool(); // start with a full resource pool
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
            int num = RandomProvider.Next(
                Simulation.Configuration.EnvironmentSettings.MinimumRenewableResourceNodes, 
                Simulation.Configuration.EnvironmentSettings.MinimumRenewableResourceNodes);

            for (int i = 0; i < num; i++)
            {
                var node = GlobalResources.GetRandom();
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
                    var settings = Simulation.Configuration.EnvironmentSettings;
                    globalResources = new List<IResourceNode>();
                    for (int i = 0; i < settings.GlobalResourcePoolSize; i++)
                    {
                        globalResources.Add(GridResourceNode.New(Simulation.Configuration.TagSettings.MaxSize, settings.MinimumResourcesPerNode, settings.MaximumResourcesPerNode));
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
