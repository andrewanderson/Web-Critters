using System;
using System.Collections.Generic;
using System.Text;
using Cas.Core.Interfaces;

namespace Cas.Core
{
    /// <summary>
    /// Tags are the discrete functional sections of <see cref="ICell"/> objects that define
    /// the nature of each behaviour.
    /// 
    /// Examples: offense tag, defence tag, identity tag, etc.
    /// </summary>
    public class Tag
    {
        #region Instance members

        /// <summary>
        /// The series of resource identifiers that makes up the Tag.
        /// </summary>
        /// <example>
        /// Consider an <see cref="IEnvironment"/> with 4 distinct resources {a, b, c, d}. 
        /// Some examples are:
        /// b#cab
        /// aaaa
        /// a
        /// d###
        /// ###
        /// </example>
        public List<Resource> Data { get; private set; }

        // Deny default creation
        private Tag() {  }

        /// <summary>
        /// Create a new instance of this class from character array input.
        /// </summary>
        /// <param name="tagChars">
        /// The characters that will serve as the tag data.
        /// </param>
        private Tag(char[] tagChars)
        {
            if (tagChars == null) throw new ArgumentNullException("tagChars");
            if (tagChars.Length < 1 || tagChars.Length > MaxSize) throw new ArgumentOutOfRangeException("tagChars", tagChars.Length, string.Format("tagChars length must be between 1 and {0}", MaxSize));

            Data = new List<Resource>();
            for (int i = 0; i < tagChars.Length; i++)
            {
                char c = tagChars[i];
                Resource r = Resource.Resolve(c);

                if (r == null)
                {
                    throw new IndexOutOfRangeException(string.Format("Resource '{0}' is not valid in the simulation.", c));
                }
 
                Data.Add(r);
            }
        }

        /// <summary>
        /// Returns the string representation of this Tag.
        /// </summary>
        public override string ToString()
        {
            if (Data == null) return null;
            
            StringBuilder sb = new StringBuilder();
            foreach (var d in Data)
            {
                sb.Append(d.ToString());
            }
            return sb.ToString();
        }

        #endregion

        #region Static members

        /// <summary>
        /// The maximum size of a Tag in the system.  No tag can exceed this.
        /// </summary>
        public const int MaxSize = 10;

        /// <summary>
        /// Returns a new random tag with a size between 1 and 10.
        /// </summary>
        public static Tag New() 
        {
            return Tag.New(RandomProvider.Next(9) + 1, false);
        }

        /// <summary>
        /// Generates a new random tag of the desired length.
        /// </summary>
        public static Tag New(int length, bool allowWildCard)
        {
            if (length < 1 || length > MaxSize) throw new ArgumentOutOfRangeException("length");

            Tag newTag = new Tag();
            newTag.Data = new List<Resource>();

            for (int i = 0; i< length; i++)
            {
                newTag.Data.Add(Resource.Random(allowWildCard));
            }

            return newTag;
        }

        /// <summary>
        /// Generates a new tag with a specific set of characters.
        /// </summary>
        public static Tag New(string chars)
        {
            if (string.IsNullOrEmpty(chars)) throw new ArgumentNullException("chars");

            return Tag.New(chars.ToCharArray());
        }

        /// <summary>
        /// Generates a new tag with a specific set of characters.
        /// </summary>
        public static Tag New(char[] chars)
        {
            if (chars == null) throw new ArgumentNullException("chars");

            return new Tag(chars);
        }

        #endregion

    }
}
