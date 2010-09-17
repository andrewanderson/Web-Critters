using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cas.Core.Interfaces
{
    /// <summary>
    /// Defines the manner in which simulation participants behave with one another.
    /// </summary>
    public interface IInteraction<TActor, TTarget, TResult>
    {
        /// <summary>
        /// Performs some form of interaction between two members of a simulation.
        /// </summary>
        /// <param name="actor">
        /// The object initiating the action.
        /// </param>
        /// <param name="target">
        /// The object that the action is directed towards
        /// </param>
        /// <returns>
        /// Some data that can be used to assess the results of the interaction.
        /// </returns>
        TResult Interact(TActor actor, TTarget target);
    }
}
