using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cas.Core
{
    /// <summary>
    /// The type of object that the identity is for.  Serves as the first discriminator when testing for equality.
    /// </summary>
    public enum IdentityType
    {
        Species,
        ResourceNode,
        Corpse,
        Fossil,
    }
}
