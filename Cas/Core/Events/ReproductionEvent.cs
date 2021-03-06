﻿using System;
using System.Collections.Generic;
using System.Linq;
using Cas.Core.Interfaces;

namespace Cas.Core.Events
{
    public class ReproductionEvent : EventBase
    {
        public ISpecies Mate { get; private set; }

        public List<ISpecies> Offspring { get; private set; }

        public Type ReproductionType { get; private set; }

        public ReproductionEvent(ILocation location, int generation, Type reproductionType, ISpecies mate, params ISpecies[] children)
            : base(location, generation)
        {
            this.Mate = mate;
            this.Offspring = new List<ISpecies>(children);
            this.ReproductionType = reproductionType;
        }

        public override string ToString()
        {
            string offspring = string.Join(",", Offspring.Select(x => x.ToString()));
            if (Mate == null)
            {

                return string.Format("{0}: {1} at {2} produced offspring: {3}", this.Generation, ReproductionType.Name, Location.ToShortString(), offspring);
            }
            else
            {
                return string.Format("{0}: {1} with {2} at {3} produced offspring: {4}", this.Generation, ReproductionType.Name, Mate, Location.ToShortString(), offspring);
            }
        }
    }
}
