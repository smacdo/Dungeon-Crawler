/*
 * Copyright 2012-2017 Scott MacDonald
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Scott.Forge.GameObjects
{
    /// <summary>
    ///  GameObject represents a basic, physical game object that can exist in the game world.
    /// </summary>
    public class GameObject : IGameObject
    {
        private const int InitialComponentCapacity = 7;

        private bool mDisposed = false;
        private GameObject mParent = null;
        
        private Dictionary<System.Type, IComponent> mComponents =
            new Dictionary<Type, IComponent>(InitialComponentCapacity);

        /// <summary>
        ///  Creates a new empty game object that is not associated with any collection.
        /// </summary>
        public GameObject()
            : this(null)
        {
        }

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="name">Name of the game object, or null for none.</param>
        public GameObject(string name)
        {
            Name = name ?? string.Empty;
            Id = Guid.NewGuid();

            // Pre-create a transform component, and make sure it is also in the component bag.
            Transform = new TransformComponent();
            Transform.Owner = this;

            mComponents.Add(Transform.GetType(), Transform);
        }

        /// <summary>
        ///  Destructor ensures that all components are disposed of properly.
        /// </summary>
        ~GameObject()
        {
            Dispose(false);
        }

        /// <summary>
        ///  Get or ste if this game object is enabled.
        /// </summary>
        /// <remarks>
        ///  Changing the value of this property will propagate this change to all children.
        /// </remarks>
        public bool Active
        {
            get { return ActiveInHierarchy && ActiveSelf; }
            set
            {
                if (ActiveSelf != value)
                {
                    ActiveSelf = value;
                    OnActiveChanged(!value);
                }
            }
        }

        /// <summary>
        ///  Get if the GameObject is active in the current scene.
        /// </summary>
        public bool ActiveInHierarchy { get; private set; } = true;

        /// <summary>
        ///  Get the local active state of the GameObject.
        /// </summary>
        public bool ActiveSelf { get; private set; } = true;

        /// <summary>
        ///  Get the first child of this game object.
        /// </summary>
        public GameObject FirstChild { get; private set; }

        /// <summary>
        ///  Get the first child of this game object.
        /// </summary>
        IGameObject IGameObject.FirstChild { get { return FirstChild; } }

        /// <summary>
        ///  Get a unique identifier for this game object.
        /// </summary>
        /// <remarks>
        ///  The id should be globally unique, however there are no code checks to enforce this. It will cause problems
        ///  with any system that expects a unique id, especially the serialization and network systems.
        /// </remarks>
        public Guid Id { get; private set; }

        /// <summary>
        ///  Get a string identifier for this game object.
        /// </summary>
        /// <remarks>
        ///  The name value does not have to globally unique.
        /// </remarks>
        public string Name { get; private set; }

        /// <summary>
        ///  Get the next sibling of this game object.
        /// </summary>
        public GameObject NextSibling { get; private set; }

        /// <summary>
        ///  Get the next sibling of this game object.
        /// </summary>
        IGameObject IGameObject.NextSibling { get { return NextSibling; } }

        /// <summary>
        ///  Get or set the parent of a game object.
        /// </summary>
        /// <remarks>
        ///  Changing the parent can trigger a cascade of updates which is a moderately slow operation.
        /// </remarks>
        public GameObject Parent
        {
            get { return mParent; }
            set
            {
                // Skip update logic if parent is the same instance.
                if (mParent == value)
                {
                    return;
                }

                // Unparent the game object.
                var oldParent = mParent;

                if (oldParent != null)
                {
                    if (!oldParent.RemoveChild(this))
                    {
                        throw new InvalidOperationException("Parent does not have this object has a child");
                    }
                }

                // Change parent to new game object and notify components that the parent has changed.
                if (value != null)
                {
                    value.AddChild(this);
                }
                else
                {
                    mParent = null;
                }
                
                Transform.OnParentChanged(value, oldParent);
            }
        }

        /// <summary>
        ///  Get or set the parent of a game object.
        /// </summary>
        /// <remarks>
        ///  Changing the parent can trigger a cascade of updates which is a moderately slow operation.
        /// </remarks>
        IGameObject IGameObject.Parent
        {
            get { return Parent; }
            set { Parent = (GameObject) value; }
        }

        /// <summary>
        ///  Get component containing information on this game object's physical location in the game world.
        /// </summary>
        public TransformComponent Transform { get; private set; }

        /// <summary>
        ///  Add a component to this game object.
        /// </summary>
        /// <remarks>
        ///  Only one component of each type can be added to the game object.
        /// </remarks>
        /// <typeparam name="TComponent">Component type to add.</typeparam>
        /// <param name="component">Component component to add.</param>
        public void Add<TComponent>(TComponent component) where TComponent : IComponent
        {
            if (ReferenceEquals(component, null))
            {
                throw new ArgumentNullException("component");
            }

            var key = component.GetType();

            if (!mComponents.ContainsKey(key))
            {
                mComponents.Add(key, component);
            }
            else
            {
                throw new ComponentAlreadyAddedException(this, component);
            }
        }

        /// <summary>
        ///  Check if this game object has a component of the specified type.
        /// </summary>
        /// <typeparam name="TComponent">Type of component to check for.</typeparam>
        /// <returns>True if the game object has the component type, false otherwise.</returns>
        public bool Contains<T>() where T : IComponent
        {
            return mComponents.ContainsKey(typeof (T));
        }

        /// <summary>
        ///  Find the first child that matches the given name.
        /// </summary>
        /// <param name="name">Name of the child game object.</param>
        /// <returns>The child game object if located otherwise null.</returns>
        public IGameObject FindChildByName(string name)
        {
            var next = FirstChild;

            while (next != null)
            {
                if (next.Name == name)
                {
                    return next;
                }
                else if (next.FirstChild != null)
                {
                    var result = next.FindChildByName(name);

                    if (result != null)
                    {
                        return result;
                    }
                }

                next = next.NextSibling;
            }

            return null;
        }

        /// <summary>
        /// Returns the game object's component. Use the non-generic Get for a (slightly)
        /// faster look up.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>() where T : IComponent
        {
            IComponent component;
            if (!mComponents.TryGetValue(typeof(T), out component))
            {
                throw new ComponentDoesNotExistException(this, typeof(T));
            }

            return (T) component;
        }

        /// <summary>
        ///  Remove a component from this game object.
        /// </summary>
        /// <remarks>
        ///  Removing the transform component will also set the .Transform property to null. Generally, if the calling
        ///  code is not destroying the game object, removing the transform component will have unpredicable
        ///  consequences.
        /// 
        ///  Additionally the component is disposed after being removed from this object. Since the game object has
        ///  exclusive ownership over its components, you should not interact with the component once Remove`T` is
        ///  invoked.
        /// </remarks>
        /// <typeparam name="TComponent">Component type to remove.</typeparam>
        public bool Remove<TComponent>() where TComponent : IComponent
        {
            return Remove(typeof (TComponent));
        }

        /// <summary>
        ///  Remove a component from this game object.
        /// </summary>
        /// <remarks>
        ///  Removing the transform component will also set the .Transform property to null. Generally, if the calling
        ///  code is not destroying the game object, removing the transform component will have unpredicable
        ///  consequences.
        /// 
        ///  Additionally the component is disposed after being removed from this object. Since the game object has
        ///  exclusive ownership over its components, you should not interact with the component once Remove`T` is
        ///  invoked.
        /// </remarks>
        /// <typeparam name="TComponent">Component type to remove.</typeparam>
        public bool Remove(System.Type componentType)
        {
            IComponent component;

            if (mComponents.TryGetValue(componentType, out component))
            {
                if (componentType == typeof(TransformComponent))
                {
                    Transform = null;
                }

                mComponents.Remove(componentType);
                ((IDisposable) component).Dispose();

                return true;
            }

            return false;
        }

        /// <summary>
        ///  Get a component from the game object. This will return null if the component was not added.
        /// </summary>
        /// <typeparam name="TComponent">Type of game object to get.</typeparam>
        /// <returns>Component that was stored in this game object.</returns>
        public TComponent TryGet<TComponent>() where TComponent : class, IComponent
        {
            IComponent component;

            if (mComponents.TryGetValue(typeof(TComponent), out component))
            {
                return component as TComponent;
            }

            return null;
        }

        /// <summary>
        ///  Display debugging information about this game object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(
                "GameObject id = {0}, Name = {1}, ComponentCount = {2}",
                Id,
                Name,
                mComponents.Count);
        }

        /// <summary>
        ///  Dispose this game object, and all components contained within.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///  Dispose this game object, and all components contained within.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!mDisposed)
            {
                foreach (var pair in mComponents)
                {
                    ((IDisposable) pair.Value).Dispose();
                }

                mComponents = null;
                Transform = null;
                Id = Guid.Empty;
                Name = null;
                mDisposed = true;
            }
        }

        /// <summary>
        ///  Add a child game object.
        /// </summary>
        /// <remarks>
        ///  Should only be called by the Parent property when it is changed.
        /// </remarks>
        /// <param name="child">The child game object to add.</param>
        private void AddChild(GameObject child)
        {
            if (child == null)
            {
                throw new ArgumentNullException("child");
            }

            if (child == this)
            {
                throw new ArgumentException("Cannot add self as child", "child");
            }

            if (child.Parent != null)
            {
                throw new ArgumentException("Cannot add child with existing parent", "child");
            }

            if (child.NextSibling != null)
            {
                throw new ArgumentException("Child has dangling sibling reference", "child");
            }

            // Add to the start of the children list (For performance).
            // TODO: Consider debug option to validate game object not added twice.
            if (FirstChild == null)
            {
                FirstChild = child;
            }
            else
            {
                child.NextSibling = FirstChild;
                FirstChild = child;
            }

            // Change child parent to this game object. Make sure to update the mParent backing field to avoid circular
            // updates.
            child.mParent = this;
        }

        /// <summary>
        ///  Remove a child game object.
        /// </summary>
        /// <remarks>
        ///  Should only be called by the Parent property when it is changed.
        /// </remarks>
        /// <param name="child">The child object to remove.</param>
        private bool RemoveChild(GameObject child)
        {
            if (child == null)
            {
                throw new ArgumentNullException("child");
            }

            if (child == this)
            {
                throw new ArgumentException("Cannot remove self as child", "child");
            }

            if (child.Parent != this)
            {
                throw new ArgumentException("Cannot remove game object that is not child of self", "child");
            }

            // Find and remove the child.
            bool didFindAndRemove = false;

            if (child == FirstChild)
            {
                // First child is the one to remove.
                FirstChild = child.NextSibling;
                didFindAndRemove = true;
            }
            else
            {
                // Search children list until child is found.
                var previousNode = FirstChild;
                var current = FirstChild.NextSibling;

                while (current != null && !didFindAndRemove)
                {
                    if (current == child)
                    {
                        previousNode.NextSibling = current.NextSibling;
                        didFindAndRemove = true;
                    }

                    previousNode = current;
                    current = current.NextSibling;
                }
            }

            // Remove the child's parent reference before returning. (Do not assign to public Parent field).
            child.mParent = null;
            child.NextSibling = null;

            return didFindAndRemove;
        }
        
        /// <summary>
        ///  Called when the GameObject Active property is changed.
        /// </summary>
        /// <param name="oldActiveSelf">Previous ActiveSelf value.</param>
        private void OnActiveChanged(bool oldActiveSelf)
        {
            if (FirstChild != null)
            {
                SetActiveInHierarchy(this);
            }
        }

        /// <summary>
        ///  Update active in hierarchy value and propagate changes to children.
        /// </summary>
        /// <param name="parent"></param>
        private void SetActiveInHierarchy(GameObject parent)
        {
            var next = FirstChild;

            while (next != null)
            {
                next.ActiveInHierarchy = parent.ActiveSelf && parent.ActiveInHierarchy;

                if (next.FirstChild != null)
                {
                    next.FirstChild.SetActiveInHierarchy(next);
                }

                next = next.NextSibling;
            }
        }
    }
}
