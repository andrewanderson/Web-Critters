using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cas.Core.Interfaces
{
    /// <summary>
    /// An interface that serves to flag implementors as singletons within
    /// the simulation.
    /// 
    /// Examples:  Species, ResourceNodes
    /// </summary>
    public interface IIsUnique
    {
        /// <summary>
        /// Unique identifier for the object.
        /// </summary>
        long Id { get; }

        /// <summary>
        /// A terse description of the object.
        /// </summary>
        string ToShortString();
    }

    public class IIsUnqiueEqualityComparer : IEqualityComparer<IIsUnique>
    {
        public bool Equals(IIsUnique x, IIsUnique y)
        {
            if (Object.ReferenceEquals(x, y)) return true;
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null)) return false;
            return x.Id == y.Id;
        }

        public int GetHashCode(IIsUnique obj)
        {
            return obj.Id.GetHashCode();
        }

        public static IIsUnqiueEqualityComparer Instance = new IIsUnqiueEqualityComparer();
    }

}
