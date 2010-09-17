using System;
using System.Collections.Generic;
using System.Linq;

namespace Cas.Core.Events
{
    public class ReproductionEvent : EventBase
    {
        public Guid MateId { get; private set; }

        public List<Guid> OffspringIds { get; private set; }

        public Type ReproductionType { get; private set; }

        public ReproductionEvent(Guid agentId, Guid locationId, int generation, Type reproductionType, Guid mateId, params Guid[] childIds)
            : base(agentId, locationId, "reproduction", generation)
        {
            this.MateId = mateId;
            this.OffspringIds = new List<Guid>(childIds);
            this.ReproductionType = reproductionType;
        }

        public override string ToString()
        {
            string offspring = string.Join(",", OffspringIds.Select(x => x.ToString()));
            if (MateId == Guid.Empty)
            {

                return string.Format("{0}: {1} at location {2} produced offspring: {3}", this.Generation, ReproductionType.Name, LocationId, offspring);
            }
            else
            {
                return string.Format("{0}: {1} with {2} at location {3} produced offspring: {4}", this.Generation, ReproductionType.Name, MateId, LocationId, offspring);
            }
        }
    }
}
