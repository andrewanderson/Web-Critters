using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cas.Core
{
    public class UniqueIdentifier
    {
        private readonly IdentityType type;
        private readonly Resource[] genome;

        /// <summary>
        /// The type of CAS object that the identifier is for.
        /// </summary>
        /// <remarks>
        /// Objects with the same Genome but different Types are not equal.
        /// </remarks>
        public IdentityType Type
        {
            get
            {
                return this.type;
            }
        }
        
        /// <summary>
        /// The sequence of resources that make up the object that is being identified.
        /// Nulls may be used as spacers to distinguish between objects with the same resources
        /// but different tag structures.
        /// </summary>
        public Resource[] Genome
        {
            get
            {
                return this.genome;
            }
        }

        public UniqueIdentifier(IdentityType type, Resource[] genome)
        {
            this.type = type;
            this.genome = genome;
        }

        /// <summary>
        /// Create a new unique identifier from a list of tags.
        /// </summary>
        /// <param name="type">
        /// The type of object that the identifier is for.
        /// </param>
        /// <param name="tags">
        /// A list of Tag objects and nulls, where null specifies the delimiter
        /// between agent tags in the case of a multiagent.
        /// </param>
        public UniqueIdentifier(IdentityType type, Tag[] tags)
        {
            this.type = type;

            int totalSize = tags.Sum(x => (x==null) ? 0 : x.Data.Count) + tags.Length - 1;
            this.genome = new Resource[totalSize];
            int i = 0;
            bool isFirst = true;
            foreach (var tag in tags)
            {
                // Pad with a null for every new tag after the first
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    this.genome[i++] = null;
                }

                if (tag != null)
                {
                    Array.Copy(tag.Data.ToArray(), 0, this.genome, i, tag.Data.Count);
                    i += tag.Data.Count;
                }
            }
        }

        public override bool Equals(object obj)
        {
            var other = obj as UniqueIdentifier;
            if (other == null) return false;

            if (this.Type != other.Type) return false;
            if (this.Genome.Length != other.Genome.Length) return false;

            for (int i = 0; i < this.Genome.Length; i++)
            {
                if (this.Genome[i] != other.Genome[i]) return false;
            }

            return true;
        }

        public static bool operator ==(UniqueIdentifier o1, UniqueIdentifier o2)
        {
            if (object.ReferenceEquals(o1, o2)) return true;
            if (object.ReferenceEquals(o1, null)) return false;
            if (object.ReferenceEquals(o2, null)) return false;

            return o1.Equals(o2);
        }

        public static bool operator !=(UniqueIdentifier o1, UniqueIdentifier o2)
        {
            return !(o1 == o2);
        }

        // See here for some explanation on the technique used:
        //http://stackoverflow.com/questions/638761/c-gethashcode-override-of-object-containing-generic-array#639098
        public override int GetHashCode()
        {
            int hash = this.Type.GetHashCode();
            if (this.Genome != null)
            {
                hash = (hash * 17) + this.Genome.Length;
                foreach (Resource r in this.Genome)
                {
                    hash *= 17;
                    hash = (r != null) ? hash + r.GetHashCode() : hash + 47;
                }
            }
            return hash;
        }

        public override string ToString()
        {
            StringBuilder idString = new StringBuilder();

            switch (this.Type)
            {
                case IdentityType.Corpse:
                    idString.Append("C");
                    break;
                case IdentityType.Fossil:
                    idString.Append("F");
                    break;
                case IdentityType.ResourceNode:
                    idString.Append("R");
                    break;
                case IdentityType.Species:
                    idString.Append("S");
                    break;
                default:
                    throw new InvalidOperationException("Unknown identifier type.");
            }
            idString.Append(".");

            foreach (var r in this.Genome)
            {
                if (r == null)
                {
                    idString.Append("_");
                }
                else
                {
                    idString.Append(r.Label);
                }
            }

            return idString.ToString();
        }
    }
}
