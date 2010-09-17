using System;
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
        internal List<Resource> Reservoir { get; private set; }

        public GridResourceNode(List<Resource> resources)
        {
            RenewableResources = new List<Resource>();
            RenewableResources.AddRange(resources);

            Reservoir = new List<Resource>();
            RefreshReservoir();

            Id = Guid.NewGuid();
            Offense = Tag.New(GridCell.MaxOffenseSize, false);
            Defense = Tag.New(GridCell.MaxDefenseSize, false);
            Exchange = Tag.New(GridCell.MaxExchangeSize, false);
        }

        public GridResourceNode(IResourceNode node)
        {
            if (node == null) throw new ArgumentNullException("node");

            this.Id = node.Id;
            this.Offense = node.Offense;
            this.Defense = node.Defense;
            this.Exchange = node.Exchange;

            this.RenewableResources = new List<Resource>();
            this.RenewableResources.AddRange(node.RenewableResources.Select(x=>x.Clone() as Resource));

            this.Reservoir = new List<Resource>();
            this.RefreshReservoir();
        }

        public override string ToString()
        {
            string renewableResources = string.Concat(this.RenewableResources.Select(x => x.ToString()));
            return string.Format("{1} {2} {3} => {0}", renewableResources, this.Offense, this.Defense, this.Exchange);
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
            Reservoir = new List<Resource>(
                RenewableResources.Select(resource => (Resource)resource.Clone()).ToList()
            );
        }

        #endregion

        #region IInteractable Members

        public Guid Id { get; private set; }

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

        #region ICloneable Members

        public object Clone()
        {
            return new GridResourceNode(this);
        }

        #endregion

    }
}
