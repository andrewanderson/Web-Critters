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
        /// Allows an agent to have a resource conversion. (i.e. a => b) which
        /// may or may not have an associated cost.
        /// </summary>
        Tag Transformation { get; set; } // pg. 113

        /// <summary>
        /// Controls whether or not an agent/cell will enter into the boundry of other agent/cells.
        /// </summary>
        Tag Adhesion { get; set; } // pg. 115

        /// <summary>
        /// Constrains the potential mates that an agent/cell may have.
        /// </summary>
        Tag Mating { get; set; } // pg. 122

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
