using System;
using System.Collections.Generic;

namespace Cas.Core.Interfaces
{
    /// <summary>
    /// A cell is a single component of an agent (or, to be more explicit, multi-agent.  In John Holland's 
    /// terms, an ICell is an agent, and it lives in a boundary, which is an IAgent.
    /// </summary>
    public interface ICell : IInteractable, IIsAlive
    {
        /// <summary>
        /// Allows an agent to have a resource conversion. (i.e. a => b) which
        /// may or may not have an associated cost.
        /// </summary>
        Tag Transformation { get; set; } // pg. 113

        /// <summary>
        /// Controls whether or not an ICell will enter into the Agents of other ICells.
        /// </summary>
        Tag Adhesion { get; set; } // pg. 115

        /// <summary>
        /// Constrains the potential mates that an Agent may have.
        /// </summary>
        Tag Mating { get; set; } // pg. 122

        /// <summary>
        /// The string representation of the ICell's genetic material.
        /// </summary>
        /// <remarks>
        /// In most implementations this should be derived (by combining all
        /// tags) rather than stored.
        /// </remarks>
        string Chromosome { get; }

        /// <summary>
        /// How many tags are we using in the model?
        /// </summary>
        int ActiveTagsInModel { get; }

        /// <summary>
        /// Retrieve a tag by index.  The CellTagIndex enum should be
        /// used to determine the indecies.
        /// </summary>
        Tag GetTagByIndex(int index);

        /// <summary>
        /// Set the value of a tag by index.  The CellTagIndex enum should be
        /// used to determine the indecies.
        /// </summary>
        void SetTagByIndex(int index, Tag value);

        /// <summary>
        /// Return a blank cell that has the same class as this object.
        /// </summary>
        ICell CreateEmptyCell();

        /// <summary>
        /// Calculates the size of a cell, which is equivalent to the
        /// number of resources defined within its tags.
        /// </summary>
        int Size { get; }

        /// <summary>
        /// Add a fixed number of resources to the Reservoir.
        /// </summary>
        void AddRandomResources(int count);

        /// <summary>
        /// Copies the structure, but not the state, of a cell.
        /// </summary>
        ICell DeepCopy();
    }
}
