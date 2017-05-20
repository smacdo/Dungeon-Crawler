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
using Microsoft.Xna.Framework;
using Scott.Forge.Actors;
using Scott.Forge.Ai;
using Scott.Forge.Physics;
using Scott.Forge.Sprites;
using Scott.Forge.GameObjects;
using Scott.Forge.Spatial;
using Scott.Forge.Tilemaps;
using Scott.Forge.Graphics;

namespace Scott.Forge
{
    /// <summary>
    ///  Represents a game scene that has a tilemap, game objects, physics and everything simulatable.
    ///  [TODO: Better description].
    ///  
    /// TODO: When spawning a game object, the scene should set itself as the owner on GameObject.Scene.
    ///       Adding components should check they are from the same scene, etc. Very useful for debugging.
    /// </summary>
    public class GameScene : IGameScene
    {
        private List<GameObject> mRootGameObjects = new List<GameObject>();

        public SpriteComponentProcessor Sprites { get; internal set; }
        public PhysicsComponentProcessor Physics { get; internal set; }

        /// <summary>
        ///  Get the scene tilemap.
        /// </summary>
        public TileMap Tilemap { get; private set; }

        /// <summary>
        ///  Get or set the main camera used for rendering.
        /// </summary>
        /// <remarks>
        ///  The camera can be set to null which will disable rendering.
        /// </remarks>
        public Camera MainCamera { get; set; }

        // TODO: Change this PlayerController, and move other logic into separate gameplay components or AI/Actor brian.
        public ActorProcessor Actors { get; internal set; }
        public AiProcessor AI { get; internal set; }

        /// <summary>
        ///  Get the width of the scene.
        /// </summary>
        /// <remarks>
        ///  Make it so this value can be set independently of tile map.
        /// </remarks>
        public float Width { get { return Tilemap.Width; } }

        /// <summary>
        ///  Get the height of the scene.
        /// </summary>
        /// <remarks>
        ///  Make it so this value can be set indendently of the tile map.
        /// </remarks>
        public float Height { get { return Tilemap.Height; } }

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="tilemap">Tile map to use in the scene.</param>
        public GameScene(TileMap tilemap)
        {
            Sprites = new SpriteComponentProcessor(this);
            Physics = new PhysicsComponentProcessor(this);
            Actors = new ActorProcessor(this);
            AI = new AiProcessor(this);

            if (tilemap == null)
            {
                throw new ArgumentNullException(nameof(tilemap));
            }

            Tilemap = tilemap;
        }

        /// <summary>
        ///  Add a game object to the scene.
        /// </summary>
        /// <param name="go"></param>
        public void Add(GameObject go)
        {
            // Check that game object was provided.
            if (go == null)
            {
                throw new ArgumentNullException(nameof(go));
            }

            // Check that game object is not already in another scene.
            if (go.Scene != null)
            {
                throw new InvalidOperationException("Cannot add game object to multiple scenes");
            }

            go.Scene = this;

            // Register game object.
            mRootGameObjects.Add(go);
        }

        /// <summary>
        ///  Update the scene.
        /// </summary>
        /// <param name="gameTime">Current game simulation time.</param>
        public virtual void Update(GameTime gameTime)
        {
            // Update camera logic.
            if (MainCamera != null)
            {
                MainCamera.Update(gameTime);
            }

            // Resolve movement and collision first, before the player or AI gets chance to do anything. Hence the
            // current position of all objects (and collision) that is displayed is actually one frame BEFORE this
            // nupdate
            Physics.Update(gameTime.TotalGameTime.TotalSeconds, gameTime.ElapsedGameTime.TotalSeconds);

            // Make sure animations are primed and updated.
            Sprites.Update(gameTime.TotalGameTime.TotalSeconds, gameTime.ElapsedGameTime.TotalSeconds);

            // Update game ai and character actions
            AI.Update(gameTime.TotalGameTime.TotalSeconds, gameTime.ElapsedGameTime.TotalSeconds);
            Actors.Update(gameTime.TotalGameTime.TotalSeconds, gameTime.ElapsedGameTime.TotalSeconds);
        }

        /// <summary>
        ///  Draw the scene.
        /// </summary>
        /// <param name="gameTime">Current game simulation time.</param>
        public virtual void Draw(GameTime gameTime)
        {
            // Don't bother with drawing if no camera is available.
            if (MainCamera == null)
            {
                return;
            }

            // Draw tilemap.
            GameRoot.Renderer.DrawTilemap(MainCamera, Tilemap);

            // Draw sprites.
            Sprites.Draw(
                GameRoot.Renderer,
                MainCamera,
                gameTime.TotalGameTime.TotalSeconds,
                gameTime.ElapsedGameTime.TotalSeconds);
        }
    }
}
