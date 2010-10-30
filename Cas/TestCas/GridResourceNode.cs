﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cas.Core.Interfaces;
using Cas.Core;
using Cas.Core.Extensions;

namespace Cas.TestImplementation
{
    /// <summary>
    /// In the grid implementation (for now), a resource node
    /// contains only a single resource.
    /// </summary>
    public class GridResourceNode : IResourceNode
    {
        private static long NextAvailableId = -1;

        /// <summary>
        /// The global resource node instance that spawned this node, if any.
        /// 
        /// Global resource nodes are identifiable by checking that this field is null
        /// </summary>
        public IResourceNode Source { get; private set; }

        internal List<Resource> Reservoir { get; private set; }

        private GridResourceNode()
        {
            RenewableResources = new List<Resource>();
            Reservoir = new List<Resource>();

            Id = NextAvailableId--;
            Source = null;
        }

        private GridResourceNode(int maximumTagSize) : this()
        {
            Offense = Tag.New(maximumTagSize, false);
            Defense = Tag.New(maximumTagSize, false);
            Exchange = Tag.New(maximumTagSize, false);
        }

        private GridResourceNode(IResourceNode node)
        {
            if (node == null) throw new ArgumentNullException("node");

            this.Id = node.Id;
            this.Source = node;
            this.Offense = node.Offense;
            this.Defense = node.Defense;
            this.Exchange = node.Exchange;

            this.RenewableResources = new List<Resource>();
            this.RenewableResources.AddRange(node.RenewableResources);

            this.Reservoir = new List<Resource>();
            this.RefreshReservoir();
        }

        public static GridResourceNode New(List<Resource> resources, int maximumTagSize)
        {
            var grn = new GridResourceNode(maximumTagSize);

            grn.RenewableResources.AddRange(resources);
            grn.RefreshReservoir();

            return grn;
        }

        /// <summary>
        /// Creates a new GridResourceNode with resources that are scaled to the complexity of the
        /// node's tags.
        /// </summary>
        public static GridResourceNode New(int maximumTagSize, int minResources, int maxResources)
        {
            if (maximumTagSize <= 0) throw new ArgumentOutOfRangeException("maximumTagSize");
            if (minResources < 0) throw new ArgumentOutOfRangeException("minResources");
            if (maxResources < minResources) throw new ArgumentOutOfRangeException("maxResources cannot be less than minResources");

            var grn = new GridResourceNode();
            grn.Offense = Tag.New(maximumTagSize, false);
            grn.Defense = Tag.New(maximumTagSize, false);
            grn.Exchange = Tag.New(maximumTagSize, false);

            int nodeSize = 0;
            int resourceRange = maxResources - minResources;
            double maxSize = maximumTagSize * 3;
            double sizeRange = maxSize - 3;
            nodeSize = (int)(((double)(maxSize - grn.Size) / sizeRange) * resourceRange) + minResources;
            
            var resources = new List<Resource>();
            for (int j = 0; j < nodeSize; j++)
            {
                resources.Add(Resource.Random(false));
            }

            grn.RenewableResources.AddRange(resources);
            grn.RefreshReservoir();

            return grn;
        }

        public int Size
        {
            get
            {
                return this.Offense.Data.Count + this.Defense.Data.Count + this.Exchange.Data.Count;
            }
        }

        public override string ToString()
        {
            return string.Format("RN.{0}: {1} {2}, {3} resources", Math.Abs(this.Id), this.Offense, this.Defense, this.Reservoir.Count);
        }

        #region IResourceNode Members

        /// <summary>
        /// Return the list of resources in this node, which in the case of 
        /// a GridEnvironment is always exactly one.
        /// </summary>
        public List<Resource> RenewableResources { get; private set; }

        /// <summary>
        /// Instruct the resource node to populate the reservoir with new 
        /// copies of all renewable resources.
        /// </summary>
        public void RefreshReservoir()
        {
            Reservoir.Clear();
            Reservoir.AddRange(RenewableResources);
        }

        public IResourceNode DeepCopy()
        {
            return new GridResourceNode(this);
        }

        public string ToShortString()
        {
            return string.Format("RN.{0}: {1} {2}", Math.Abs(this.Id), this.Offense, this.Defense);
        }

        #endregion

        #region IInteractable Members

        public Tag Offense { get; set; }

        public Tag Defense { get; set; }

        public Tag Exchange { get; set; }

        #endregion

        #region IContainsResources

        public int CurrentResourceCount
        {
            get
            {
                return this.Reservoir.Count;
            }
        }

        public List<Resource> RemoveResources(int count)
        {
            if (count < 0) throw new ArgumentOutOfRangeException("count");

            var resources = new List<Resource>();
            for (int i = 0; i < count; i++)
            {
                if (this.Reservoir.Count == 0) break;
                resources.Add(this.Reservoir.RemoveRandom());
            }
            return resources;
        }

        public void AddResources(List<Resource> resources)
        {
            if (resources == null) throw new ArgumentNullException("resources");

            this.Reservoir.AddRange(resources);
        }

        public string ShowResourcePool(string delimiter)
        {
            return string.Join(delimiter, this.Reservoir.Select(x => x.Label.ToString()));
        }

        #endregion

        #region IIsUnique members
        
        /// <summary>
        /// The unique identifier for this ResourceNode, which is always a negative number.
        /// </summary>
        public long Id { get; private set; }

        #endregion

    }
}
