using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cas.Core.Interfaces
{
    /// <summary>
    /// A record of an extinct species.
    /// </summary>
    public interface IFossil : IIsUnique
    {

        // TODO:  Flesh this out; class only exists to circumvent predator/prey
        //        memory leak dilemma. 

    }
}
