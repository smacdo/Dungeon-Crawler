using Microsoft.Xna.Framework;
using Scott.GameContent;
using Scott.Geometry;
using System;

namespace Scott.Game.Entity
{
    /// <summary>
    ///  Interface for game objects.
    /// </summary>
    public interface IGameObject
    {
        void AddComponent<T>( T instance ) where T : IComponent;
        void DeleteComponent<T>() where T : Component;
        string DumpDebugInfoToString();
        T GetComponent<T>() where T : IComponent;
        Guid Id { get; }
        string Name { get; }
        bool Enabled { get; set; }
        TransformComponent Transform { get; }
    }
}
