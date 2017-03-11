/*
 * Copyright 2012-2014 Scott MacDonald
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
using Microsoft.Xna.Framework.Input;
using Scott.DungeonCrawler.Actions;
using Scott.DungeonCrawler.GameObjects;
using Scott.Forge;
using Scott.Forge.Engine;
using Scott.Forge.Engine.Actors;
using Scott.Forge.Engine.Ai;
using Scott.Forge.Engine.Content;
using Scott.Forge.Engine.Graphics;
using Scott.Forge.Engine.Input;
using Scott.Forge.Engine.Physics;
using Scott.Forge.Engine.Sprites;
using Scott.Forge.GameObjects;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Scott.DungeonCrawler
{
    /// <summary>
    ///  This is the main class for Dungeon Crawler. It is responsible for running the game loop,
    ///  responding to external events and managing content loading/unloading.
    /// </summary>
    public class DungeonCrawlerClient : Microsoft.Xna.Framework.Game
    {
        enum InputAction
        {
            ExitGame,
            PrintDebugInfo,
            Move,
            MeleeAttack,
            RangedAttack,
            CastSpell
        }

        private readonly GraphicsDeviceManager mGraphicsDevice;
        private GameObject mPlayer;
        private List<GameObject> mEnemies = new List<GameObject>();

        private bool mEnemySpawningEnabled = true;

        private SpriteComponentProcessor mSpriteProcessor = new SpriteComponentProcessor();
        private PhysicsComponentProcessor mPhysicsProcessor = new PhysicsComponentProcessor();
        private ActorProcessor mActorProcessor = new ActorProcessor();
        private AiProcessor mAiProcessor = new AiProcessor();

        private DungeonCrawlerGameObjectFactory mGameObjectFactory;

        private readonly InputManager<InputAction> mInputManager = new InputManager<InputAction>();
        private ForgeContentManager mContent;
        int mEnemyCount = 0;

        /// <summary>
        ///  Constructor.
        /// </summary>
        public DungeonCrawlerClient(string contentDirectory)
        {
            mGraphicsDevice = new GraphicsDeviceManager( this );
            Content.RootDirectory = contentDirectory ?? "Content";      // Settings.Default.ContentDir
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Configure desired settings.
            mGraphicsDevice.PreferredBackBufferWidth = 800;
            mGraphicsDevice.PreferredBackBufferHeight = 600;
            mGraphicsDevice.ApplyChanges();

            // Create our custom content manager.
            mContent = new ForgeContentManager( Services, "Content" );
            this.Content = mContent;

            // Initialize systems.
            GameRoot.Initialize( mGraphicsDevice.GraphicsDevice, Content );
            Screen.Initialize( mGraphicsDevice.GraphicsDevice );

            // Initialize default game level.
            mGameObjectFactory = new DungeonCrawlerGameObjectFactory(mContent)
            {
                SpriteProcessor = mSpriteProcessor,
                ActorProcessor = mActorProcessor,
                AiProcessor = mAiProcessor,
                PhysicsProcessor = mPhysicsProcessor
            };

            // Initialize input system with default settings.
            mInputManager.AddAction( InputAction.ExitGame, Keys.Escape );
            mInputManager.AddAction( InputAction.PrintDebugInfo, Keys.F2 );
            mInputManager.AddAction( InputAction.MeleeAttack, Keys.Space );
            mInputManager.AddAction( InputAction.RangedAttack, Keys.E );
            mInputManager.AddAction( InputAction.CastSpell, Keys.Q );
            mInputManager.AddDirectionalAction( InputAction.Move, Keys.W, DirectionName.North );
            mInputManager.AddDirectionalAction( InputAction.Move, Keys.D, DirectionName.East );
            mInputManager.AddDirectionalAction( InputAction.Move, Keys.S, DirectionName.South );
            mInputManager.AddDirectionalAction( InputAction.Move, Keys.A, DirectionName.West );

            // Let XNA engine initialize last.
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Instruct the content manager to search our content dir to find game assets that we
            // can load.
            mContent.SearchForContentItems( true );

            // Create the player blue print.
            mPlayer = mGameObjectFactory.Instantiate("Player");
            mPlayer.Transform.WorldPosition = new Forge.Vector2(200, 200);

            // Now that we have loaded the game's contents, we should force a garbage collection
            // before proceeding to play mode.
            GC.Collect();
        }

        /// <summary>
        /// TEMP HACK
        /// </summary>
        private void SpawnSkeleton()
        {
            // Spawn the skeleton enemy.
            GameObject skeleton = mGameObjectFactory.Instantiate("Skeleton");

            // Pick a spot to place the skeleton enemy.
            int width = Screen.Width - 64;
            int height = Screen.Height - 64;

            skeleton.Transform.WorldPosition = new Scott.Forge.Vector2(
                (int) ( GameRoot.Random.NextDouble() * width ),
                (int) ( GameRoot.Random.NextDouble() * height ) );

            // Add the newly spawned skeleton to our list of enemies.
            GameRoot.Enemies.Add( skeleton );
            mEnemyCount += 1;
            mEnemies.Add(skeleton);

            // temp hack
            var initialDirection = (DirectionName) GameRoot.Random.Next(0, 3);

            var actor = skeleton.Get<ActorComponent>();
            actor.Direction = initialDirection;

            skeleton.Get<SpriteComponent>().PlayAnimation("Walk", initialDirection, AnimationEndingAction.Loop);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            GameRoot.Unload();
        }

        bool firstSpawn = true;
        TimeSpan mNextSpawnTime = TimeSpan.Zero;

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update( GameTime gameTime )
        {
            // Allow game play systems to perform any pre-update logic that might be needed
            GameRoot.Debug.PreUpdate(gameTime);

            // Perform any requested actions based on user input.
            mInputManager.Update(gameTime.TotalGameTime.TotalSeconds, gameTime.ElapsedGameTime.TotalSeconds);

            if ( mInputManager.WasTriggered( InputAction.ExitGame ) )
            {
                Exit();
            }

            // Player movement.
            var playerActor = mPlayer.Get<ActorComponent>();
            var playerMovement = mInputManager.GetAxis(InputAction.Move);

            if (playerMovement.LengthSquared > 0.01)
            {
                playerActor.Move(playerMovement * 125.0f);
            }

            // Player actions.
            if ( mInputManager.WasTriggered( InputAction.MeleeAttack ) )
            {
                playerActor.Perform( new ActionSlashAttack() );
            }
            else if ( mInputManager.WasTriggered( InputAction.RangedAttack ) )
            {
                playerActor.Perform( new ActionRangedAttack() );
            }
            else if ( mInputManager.WasTriggered( InputAction.CastSpell ) )
            {
                playerActor.Perform(new ActionCastSpell());
            }

            // Spawn some stuff
            if ( mNextSpawnTime <= gameTime.TotalGameTime && GameRoot.Enemies.Count < 32 )
            {
                if ((GameRoot.Random.NextDouble() < 0.75 || firstSpawn) && mEnemySpawningEnabled)
                {
                    SpawnSkeleton();
                    firstSpawn = false;
                }

                mNextSpawnTime = gameTime.TotalGameTime.Add( TimeSpan.FromSeconds( 1.0 ) );
            }

            // Update the world //////////////////////////////
            // We resolve movement and collision first, before the player or AI gets chance
            // to do anything. Hence the current position of all objects (and collision)
            // that is displayed is actually one frame BEFORE this update
            mPhysicsProcessor.Update(gameTime.TotalGameTime.TotalSeconds, gameTime.ElapsedGameTime.TotalSeconds);

            // Update game ai and character actions
            mAiProcessor.Update(gameTime.TotalGameTime.TotalSeconds, gameTime.ElapsedGameTime.TotalSeconds);
            mActorProcessor.Update(gameTime.TotalGameTime.TotalSeconds, gameTime.ElapsedGameTime.TotalSeconds);

            // Make sure animations are primed and updated (we need to trigger the
            // correct animation events even if we are not drawwing)
            mSpriteProcessor.Update(gameTime.TotalGameTime.TotalSeconds, gameTime.ElapsedGameTime.TotalSeconds);

            base.Update( gameTime );

            // Post update
            mInputManager.ClearState();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw( GameTime gameTime )
        {
            // Render game sprites.
            GameRoot.Renderer.StartDrawing(clearScreen: true);
            mSpriteProcessor.Draw(
                GameRoot.Renderer,
                gameTime.TotalGameTime.TotalSeconds,
                gameTime.ElapsedGameTime.TotalSeconds);
            GameRoot.Renderer.FinishDrawing();

            // Debug drawing.
            GameRoot.Debug.Draw( gameTime );

            base.Draw( gameTime );
        }
    }
}