using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cas.Core.Interfaces;

namespace Cas.Core
{
    public class Fossil : IFossil
    {
        public UniqueIdentifier Id { get; private set; }

        public Fossil(UniqueIdentifier speciesId)
        {
            if (speciesId == null) throw new ArgumentNullException("speciesId");

            this.Id = new UniqueIdentifier(IdentityType.Fossil, (Resource[])speciesId.Genome.Clone());
        }

        public string ToShortString()
        {
            return this.ToString();
        }

        public override string ToString()
        {
            return this.Id.ToString();
        }
    }
}
