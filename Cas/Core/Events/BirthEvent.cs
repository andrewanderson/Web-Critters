using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cas.Core.Interfaces;

namespace Cas.Core.Events
{
    public class BirthEvent : EventBase
    {
        public Type ReproductionType { get; private set; }

        public List<ISpecies> Parents { get; private set; }

        public BirthEvent(Guid locationId, int generation, Type reproductionType, params ISpecies[] parents)
            : base(locationId, generation)
        {
            if (reproductionType == null) throw new ArgumentNullException("reproductionType");

            this.ReproductionType = reproductionType;
            this.Parents = new List<ISpecies>(parents);
        }

        public override string ToString()
        {
            string parents = string.Join(",", Parents.Select(x => x.ToString()));
            return string.Format("{0}: Birthed by {1} at location {2} by parent(s): {3}", this.Generation, ReproductionType.Name, LocationId, parents);
        }
        

    }
}
