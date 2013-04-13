using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Game.Entity.Actor
{
    /// <summary>
    ///  Interface for an actor action.
    /// </summary>
    public interface IActorAction
    {
        /// <summary>
        ///  Check if the action is completed.
        /// </summary>
        bool IsFinished { get; }

        /// <summary>
        ///  Check if the player can move while the action is in progress.
        /// </summary>
        bool CanMove { get; }

        /// <summary>
        ///  Updates the action towards completion.
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="gameTime"></param>
        void Update( ActorController actor, GameTime gameTime );
    }

    /// <summary>
    ///  An action that an actor can perform.
    /// </summary>
    public abstract class ActorAction : IActorAction
    {
        /// <summary>
        ///  Check if the action is completed.
        /// </summary>
        public abstract bool IsFinished { get; }

        public abstract bool CanMove { get; }

        /// <summary>
        ///  Updates the action towards completion.
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="gameTime"></param>
        public abstract void Update( ActorController actor, GameTime gameTime );
    }
}
