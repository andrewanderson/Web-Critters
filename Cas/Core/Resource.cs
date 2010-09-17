using System;
using System.Collections.Generic;

namespace Cas.Core
{
    /// <summary>
    /// Resources are the commodities that agents within the CAS simulation compete to obtain.
    /// Depending on the nature of the simulation, resources may be raw measures of fitness 
    /// (e.g prisoner's dilemma), or may be the building blocks that agents are constructed from (e.g. ECHO). 
    /// </summary>
    /// <remarks>
    /// A "resource" in the scope of the simulation is an atomic character like
    /// 'a', 'b', 'c', etc.  This class is meant to represent a single instance of consumable material within 
    /// a location.  For example, in ECHO it would be a single character, while in a web implementation it
    /// might be a word (or waste product), and in a twitter implementation it might be the entire message.
    /// </remarks>
    public class Resource : ICloneable
    {
        #region Instance Methods

        public Resource(char value)
        {
            this.Label = value;
        }

        /// <summary>
        /// The actual character (nutrient?) that this resource represents.
        /// </summary>
        public char Label { get; set; }

        public object Clone()
        {
            return new Resource(this.Label);
        }

        public override string ToString()
        {
            return this.Label.ToString();
        }

        public override bool Equals(object obj)
        {
            var other = obj as Resource;
            if (other == null) return false;

            return (other.Label == this.Label);
        }

        public override int GetHashCode()
        {
            return this.Label.GetHashCode();
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Each resource in the simulation will be this much more prominent than
        /// the wildcard resource when generating random resources.
        /// 
        /// i.e. NormalToWildcardRatio : 1 ratio.
        /// </summary>
        private static int NormalToWildcardRatio = 1;

        private const char WildcardResourceCharacter = '#';
        private const char FirstResourceCharacter = 'a';

        private static readonly Resource wildcardResource;
        private static readonly List<Resource> resourceList;
        private static readonly Dictionary<char, Resource> resourceMap;

        /// <summary>
        /// Retrieve the wildcard character within the simulation.
        /// </summary>
        public static Resource WildcardResource
        {
            get
            {
                return wildcardResource;
                // return (Resource)wildcardResource.Clone();  // TODO: Is cloning required?
            }
        }

        static Resource()
        {
            resourceList = new List<Resource>();
            resourceMap = new Dictionary<char, Resource>();
            wildcardResource = new Resource(WildcardResourceCharacter);
        }

        /// <summary>
        /// Populate the Resource Map with resource characters from 'a' to 'a'+numberOfResources.
        /// </summary>
        public static void Initialize(int numberOfResources, int normalToWildcardRatio)
        {
            NormalToWildcardRatio = normalToWildcardRatio;

            resourceList.Clear();
            resourceMap.Clear();
            for (int i = 0; i < numberOfResources; i++)
            {
                char c = Convert.ToChar(Convert.ToInt32(FirstResourceCharacter) + i);
                var r = new Resource(c);
                resourceList.Add(r);
                resourceMap.Add(c, r);
            }
        }

        /// <summary>
        /// Retrieve a resource from the simulation by index
        /// </summary>
        public static Resource Get(int index)
        {
            if (index < 0) throw new ArgumentException("Index cannot be less than zero", "index");
            if (index >= resourceList.Count) throw new ArgumentException("Index cannot exceed the number of resources.", "index");

            return resourceList[index];
            //return (Resource) resourceList[index].Clone(); // TODO: Is cloning required?
        }

        /// <summary>
        /// Retrieve a resource from the simulation by label
        /// </summary>
        public static Resource Resolve(char c)
        {
            if (c == WildcardResourceCharacter)
            {
                return WildcardResource;
            }
            else if (resourceMap.ContainsKey(c))
            {
                return resourceMap[c];
                //return (Resource)resourceMap[c].Clone();  // TODO: Is cloning required?
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Retrieve a random resource.
        /// </summary>
        /// <param name="allowWildcard">
        /// If true, the wildcard resource will be returned with equal probability to
        /// normal resources.
        /// </param>
        public static Resource Random(bool allowWildcard)
        {
            int range = (resourceList.Count * NormalToWildcardRatio) - 1;
            if (allowWildcard) range++;

            int rand = RandomProvider.Next(range+1);
            if (allowWildcard && rand == range)
            {
                return WildcardResource;
            }
            else
            {
                int indexToReturn = rand/NormalToWildcardRatio;
                return Get(indexToReturn);
            }
        }

        /// <summary>
        /// The number of unique resources that are defined in the Simulation.
        /// </summary>
        public static int Count
        {
            get
            {
                return resourceMap.Count;
            }
        }

        #endregion
    }
}
