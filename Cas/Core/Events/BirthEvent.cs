using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cas.Core.Events
{
    public class BirthEvent : EventBase
    {
        public Type ReproductionType { get; private set; }

        public List<Guid> ParentIds { get; private set; }

        public BirthEvent(Guid locationId, int generation, Type reproductionType, params Guid[] parentIds)
            : base(locationId, generation)
        {
            if (reproductionType == null) throw new ArgumentNullException("reproductionType");

            this.ReproductionType = reproductionType;
            this.ParentIds = new List<Guid>(parentIds);
        }

        public override string ToString()
        {
            string parents = string.Join(",", ParentIds.Select(x => x.ToString()));
            return string.Format("{0}: Birthed by {1} at location {2} by parent(s): {3}", this.Generation, ReproductionType.Name, LocationId, parents);
        }
        

    }
}
