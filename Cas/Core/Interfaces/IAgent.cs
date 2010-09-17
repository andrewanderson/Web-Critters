using System;
using System.Collections.Generic;
using Cas.Core.Events;

namespace Cas.Core.Interfaces
{
    /// <summary>
    /// Agents are the top-level entity that populate every CAS.  It is their survival and evolution
    /// that we are running the simulations to observe.
    /// 
    /// It might be more proper to think of every agent as a multi-agent.  Every agent can contain 1-N
    /// ICell object, each of which acts as a miniature agent.  In fact, in the simple case, an agent contains
    /// all a single cell, and has no more or less functionality than specified by that single cell.
    /// </summary>
    /// <remarks>
    /// An Agent is actually a specialized boundary.  Other agents and cells can live within it.
    /// </remarks>
    public interface IAgent : IBoundary, IInteractable, IIsAlive
    {
        /// <summary>
        /// The free-floating cells that make up the agent.
        /// </summary>
        /// <remarks>
        /// The simplest case is a 1:1 relationship between an IAgent and ICell, however this model
        /// easily extends to support multi-levelled organisms comprised of many cells and/or many sub-agents.
        /// </remarks>
        List<ICell> Cells { get; }

        /// <summary>
        /// Returns true if this agent is more than a single cell, or false otherwise.
        /// </summary>
        bool IsMultiAgent();

        /// <summary>
        /// Randomly nominates one cell in this agent to be representative of the entire agent.
        /// </summary>
        void SetInteractionContactPoint();

        /// <summary>
        /// The number of generations that an agent has existed for.
        /// </summary>
        long Age { get; }

        /// <summary>
        /// Adds a tick to the agent's age.
        /// </summary>
        void IncrementAge();

        /// <summary>
        /// A list of events that have occurred to this agent.
        /// </summary>
        List<IEvent> History { get; }
    }
}
