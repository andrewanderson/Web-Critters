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
