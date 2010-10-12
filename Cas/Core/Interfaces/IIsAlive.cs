using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cas.Core.Interfaces
{
    /// <summary>
    /// An interface specific to the animate things within the CAS.
    /// </summary>
    public interface IIsAlive
    {
        /// <summary>
        /// Is this thing fit enough to engage in some form of reproduction?
        /// </summary>
        bool CanReplicate(double reproductionThreshold);

        /// <summary>
        /// Is this thing a candidate for death?
        /// </summary>
        bool IsEligableForDeath { get; }

        /// <summary>
        /// Resources corresponding to all of the tags in the living object.
        /// </summary>
        List<Resource> GeneticMaterial { get; }
    }
}
