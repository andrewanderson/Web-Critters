using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cas.Core.Extensions;
using Cas.Core.Interfaces;

namespace Cas.Core
{
    /// <summary>
    /// When an agent dies, a corpse object is created which contains the resources that
    /// were encoded in its genetic code.
    /// </summary>
    public class Corpse : IResourceNode
    {
        internal List<Resource> Reservoir { get; private set; }

        public UniqueIdentifier Id { get; private set; }

        public Tag Offense { get; set; }

        public Tag Defense { get; set; }

        public Tag Exchange { get; set; }


        public Corpse(IAgent deceased)
        {
            if (deceased == null) throw new ArgumentNullException("deceased");

            this.Id = new UniqueIdentifier(IdentityType.Corpse, deceased.Species.Id.Genome);
            this.Offense = Tag.New(deceased.Offense);
            this.Defense = Tag.New(deceased.Defense);
            this.Exchange = Tag.New(deceased.Exchange);

            // Build the reservoir from the genetic code of the deceased.
            this.Reservoir = deceased.GeneticMaterial;
        }

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
            throw new NotSupportedException("Resources cannot be added to a corpse.");
        }

        public string ShowResourcePool(string delimiter)
        {
            return string.Join(delimiter, this.Reservoir.Select(x => x.Label.ToString()));
        }
        
        public IResourceNode Source
        {
            get
            {
                return null;
            }
        }

        public List<Resource> RenewableResources
        {
            get 
            {
                return new List<Resource>();
            }
        }

        public void RefreshReservoir()
        {
            throw new NotSupportedException("Cannot refresh a corpse");
        }

        public string ToShortString()
        {
            return this.Id.ToString();
        }

        public override string ToString()
        {
            return string.Format("{0}: {1} resources", this.Id, this.Reservoir.Count);
        }

        public IResourceNode DeepCopy()
        {
            throw new NotImplementedException();
        }
    }
}
