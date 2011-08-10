using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cas.Core;

namespace TestCore
{
    [TestClass]
    public class UniqueIdentifierTest
    {
        private static Resource A;
        private static Resource B;
        private static Resource C;
        private static Resource D;

        [TestInitialize()]
        public void MyTestInitialize()
        {
            var config = new Configuration();
            config.ResourceSettings.AllowWildcards = false;
            config.ResourceSettings.Count = 4;
            config.ResourceSettings.NormalToWildcardRatio = 1;

            Resource.Initialize(config);

            A = Resource.Get(0);
            B = Resource.Get(1);
            C = Resource.Get(2);
            D = Resource.Get(3);
        }

        [TestMethod]
        public void ResourceConstructorTest()
        {
            var expectedType = IdentityType.Species;
            var expectedGenome = new[] { A, B, A, C, D };

            var id = new UniqueIdentifier(expectedType, expectedGenome);

            Assert.AreEqual(expectedType, id.Type);
            Assert.AreEqual(expectedGenome.Length, id.Genome.Length);

            for (int i = 0; i < expectedGenome.Length; i++)
            {
                Assert.AreEqual(expectedGenome[i], id.Genome[i]);
            }
        }

        [TestMethod]
        public void TagConstructorTest()
        {
            var expectedType = IdentityType.Corpse;
            var tagOne = Tag.New(new[] { A, B, A, C, D });
            var tagTwo = Tag.New(new[] { C, C, D });

            var id = new UniqueIdentifier(expectedType, new[] { tagOne, tagTwo });

            Assert.AreEqual(expectedType, id.Type);
            Assert.AreEqual(tagOne.Data.Count + tagTwo.Data.Count + 1, id.Genome.Length);

            int actualIndex = 0;
            for (int i = 0; i < tagOne.Data.Count; i++)
            {
                Assert.AreEqual(tagOne.Data[i], id.Genome[actualIndex++]);
            }

            Assert.AreEqual(null, id.Genome[actualIndex++]);

            for (int i = 0; i < tagTwo.Data.Count; i++)
            {
                Assert.AreEqual(tagTwo.Data[i], id.Genome[actualIndex++]);
            }
        }

        [TestMethod]
        public void MultiAgentTagConstructorTest()
        {
            var expectedType = IdentityType.Corpse;
            var tagOne = Tag.New(new[] { A, B, A, C, D });
            var tagTwo = Tag.New(new[] { C, C, D });
            var tagThree = Tag.New(new[] { B });
            var tagFour = Tag.New(new[] { B, A, D, C });

            var id = new UniqueIdentifier(expectedType, new[] { tagOne, tagTwo, null, tagThree, tagFour });

            Assert.AreEqual(expectedType, id.Type);
            Assert.AreEqual(tagOne.Data.Count + tagTwo.Data.Count + tagThree.Data.Count + tagFour.Data.Count + 3 + 1, id.Genome.Length);

            int actualIndex = 0;
            for (int i = 0; i < tagOne.Data.Count; i++)
            {
                Assert.AreEqual(tagOne.Data[i], id.Genome[actualIndex++]);
            }

            Assert.AreEqual(null, id.Genome[actualIndex++]);

            for (int i = 0; i < tagTwo.Data.Count; i++)
            {
                Assert.AreEqual(tagTwo.Data[i], id.Genome[actualIndex++]);
            }

            Assert.AreEqual(null, id.Genome[actualIndex++]);
            Assert.AreEqual(null, id.Genome[actualIndex++]);

            for (int i = 0; i < tagThree.Data.Count; i++)
            {
                Assert.AreEqual(tagThree.Data[i], id.Genome[actualIndex++]);
            }

            Assert.AreEqual(null, id.Genome[actualIndex++]);

            for (int i = 0; i < tagFour.Data.Count; i++)
            {
                Assert.AreEqual(tagFour.Data[i], id.Genome[actualIndex++]);
            }
        }

        [TestMethod]
        public void NullFirstMultiAgentTagConstructorTest()
        {
            var expectedType = IdentityType.Corpse;
            var tagOne = Tag.New(new[] { A, B, A, C, D });
            var tagTwo = Tag.New(new[] { C, C, D });
            var tagThree = Tag.New(new[] { B });
            var tagFour = Tag.New(new[] { B, A, D, C });

            var id = new UniqueIdentifier(expectedType, new[] { null, tagOne, tagTwo, null, tagThree, tagFour });

            Assert.AreEqual(expectedType, id.Type);
            Assert.AreEqual(tagOne.Data.Count + tagTwo.Data.Count + tagThree.Data.Count + tagFour.Data.Count + 3 + 2, id.Genome.Length);

            int actualIndex = 0;
            Assert.AreEqual(null, id.Genome[actualIndex++]);
            
            for (int i = 0; i < tagOne.Data.Count; i++)
            {
                Assert.AreEqual(tagOne.Data[i], id.Genome[actualIndex++]);
            }

            Assert.AreEqual(null, id.Genome[actualIndex++]);

            for (int i = 0; i < tagTwo.Data.Count; i++)
            {
                Assert.AreEqual(tagTwo.Data[i], id.Genome[actualIndex++]);
            }

            Assert.AreEqual(null, id.Genome[actualIndex++]);
            Assert.AreEqual(null, id.Genome[actualIndex++]);

            for (int i = 0; i < tagThree.Data.Count; i++)
            {
                Assert.AreEqual(tagThree.Data[i], id.Genome[actualIndex++]);
            }

            Assert.AreEqual(null, id.Genome[actualIndex++]);

            for (int i = 0; i < tagFour.Data.Count; i++)
            {
                Assert.AreEqual(tagFour.Data[i], id.Genome[actualIndex++]);
            }
        }

        [TestMethod]
        public void EqualityTrueTest()
        {
            var tagOne = Tag.New(new[] { A, B, A, C, D });
            var tagTwo = Tag.New(new[] { C, C, D });

            var id1 = new UniqueIdentifier(IdentityType.Species, new[] { tagOne, tagTwo });
            var id2 = new UniqueIdentifier(IdentityType.Species, new[] { tagOne, tagTwo });

            Assert.IsTrue(id1 == id2);
            Assert.IsTrue(id2 == id1);
        }

        [TestMethod]
        public void EqualityFalseTest()
        {
            var tagOne = Tag.New(new[] { A, B, A, C, D });
            var tagTwo = Tag.New(new[] { C, C, D });
            var tagThree = Tag.New(new[] { B });
            var tagFour = Tag.New(new[] { B, A, D, C });

            var id1 = new UniqueIdentifier(IdentityType.Species, new[] { tagOne, tagTwo });
            var id2 = new UniqueIdentifier(IdentityType.Species, new[] { tagThree, tagFour });

            Assert.IsFalse(id1 == id2);
            Assert.IsFalse(id2 == id1);
        }

        [TestMethod]
        public void EqualityFalseTypeOnlyTest()
        {
            var tagOne = Tag.New(new[] { A, B, A, C, D });
            var tagTwo = Tag.New(new[] { C, C, D });

            var id1 = new UniqueIdentifier(IdentityType.Species, new[] { tagOne, tagTwo });
            var id2 = new UniqueIdentifier(IdentityType.ResourceNode, new[] { tagOne, tagTwo });

            Assert.IsFalse(id1 == id2);
            Assert.IsFalse(id2 == id1);
        }

    }
}
