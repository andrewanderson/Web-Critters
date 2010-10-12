using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cas.Core;
using Cas.Core.Interfaces;

namespace Cas.TestImplementation
{
    public class GridLocation : LocationBase
    {
        /// <summary>
        /// The X-coordinate of this location in a grid
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// The Y-coordinate of this location in a grid
        /// </summary>
        public int Y { get; set; }

        public GridLocation(int x, int y, ISimulation simulation, List<IResourceNode> resourceAllocation, int upkeep, int capacity) : base(simulation)
        {
            if (resourceAllocation == null) throw new ArgumentNullException("resourceAllocation");
            if (x < 0) throw new ArgumentOutOfRangeException("x", x, "x must be zero or greater");
            if (y < 0) throw new ArgumentOutOfRangeException("y", y, "y must be zero or greater");
            if (upkeep < 0) throw new ArgumentOutOfRangeException("upkeep", upkeep, "upkeep must be zero or greater");
            if (capacity < 0) throw new ArgumentOutOfRangeException("capacity", upkeep, "capacity must be zero or greater");

            Connections = new List<ILocation>();
            Agents = new List<IAgent>();
            CurrentResources = new List<IResourceNode>();

            X = x;
            Y = y;
            UpkeepCost = upkeep;
            ResourceCapacity = capacity;

            ResourceAllocation = resourceAllocation;
        }

        public override string ToShortString()
        {
            return string.Format("X={0}, Y={1}", X, Y);
        }

        public override string ToString()
        {
            return string.Format("{0} : [{1} upkeep, {2} capacity] : {3} agents", this.ToShortString(), UpkeepCost, ResourceCapacity, this.Agents.Count);
        }
    }
}
