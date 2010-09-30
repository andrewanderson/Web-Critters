using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cas.Core.Interfaces;
using Cas.Core.Extensions;

namespace Cas.Core
{
    /// <summary>
    /// Functionality that is common to all ICell implementations
    /// </summary>
    public abstract class CellBase : ICell
    {
        private const int CurrentlyImplementedTagCount = 3;

        internal List<Resource> Reservoir { get; private set; }

        protected CellBase()
        {
            Reservoir = new List<Resource>();
            ActiveTagsInModel = GetActiveTagCount();
        }

        /// <summary>
        /// Return the number of tags implemented by the particular model.
        /// </summary>
        /// <returns></returns>
        protected int GetActiveTagCount()
        {
            return CurrentlyImplementedTagCount;
        }

        #region ICell Members

        // Not yet implemented

        public Tag Transformation { get; set; }

        public Tag Adhesion { get; set; }

        public Tag Mating { get; set; }

        public string Chromosome
        {
            get { throw new NotImplementedException(); }
        }

        public int ActiveTagsInModel { get; private set; }

        public Tag GetTagByIndex(int index)
        {
            switch(index)
            {
                case (int)CellTagIndex.Offense:
                    return Offense;
                case (int) CellTagIndex.Defense:
                    return Defense;
                case (int)CellTagIndex.Exchange:
                    return Exchange;
                default:
                    throw new ArgumentOutOfRangeException("index", index, "No tag with specified index.");
            }
        }

        public void SetTagByIndex(int index, Tag value)
        {
            switch (index)
            {
                case (int)CellTagIndex.Offense:
                    Offense = value;
                    break;
                case (int)CellTagIndex.Defense:
                    Defense = value;
                    break;
                case (int)CellTagIndex.Exchange:
                    Exchange = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("index", index, "No tag with specified index.");
            }
        }

        public abstract ICell CreateEmptyCell();

        public int Size
        {
            get
            {
                int size = 0;
                for (int i = 0; i < this.ActiveTagsInModel; i++)
                {
                    size += this.GetTagByIndex(i).Data.Count;
                }
                return size;
            }
        }

        public void AddRandomResources(int count)
        {
            for (int i = 0; i < count; i++)
            {
                this.Reservoir.Add(Resource.Random(false));
            }
        }

        public abstract ICell DeepCopy();

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

        #region IIsAlive Members

        public bool CanReplicate(double reproductionThreshold)
        {
            return this.Reservoir.Count >= (int)this.Size * reproductionThreshold;
        }

        public bool IsEligableForDeath {
            get
            {
                return this.Reservoir.Count == 0;
            }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", Offense, Defense, Exchange);
        }

    }
}
