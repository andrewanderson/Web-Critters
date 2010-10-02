using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cas.Core.Interfaces;

namespace Cas.Core
{
    public class Fossil : IFossil
    {
        public long Id { get; private set; }

        public Fossil(long id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException("id", "id must be positive");

            this.Id = id;
        }

        public override string ToString()
        {
            return string.Format("F.{0}: extinct", this.Id);
        }
    }
}
