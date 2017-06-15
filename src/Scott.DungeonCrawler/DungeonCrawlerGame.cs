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
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Scott.DungeonCrawler.Actions;
using Scott.DungeonCrawler.GameObjects;
using Scott.Forge;
using Scott.Forge.Actors;
using Scott.Forge.Engine.Content;
using Scott.Forge.Input;
using Scott.Forge.Sprites;
using Scott.Forge.GameObjects;
using Scott.Forge.Graphics;
using Scott.Forge.Content;
using Scott.Forge.Spatial;
using Scott.Forge.Tilemaps;
using Scott.DungeonCrawler.WorldGeneration;

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
            MoveCamera,
            MeleeAttack,
            RangedAttack,
            CastSpell
        }

        private readonly GraphicsDeviceManager mGraphicsDevice;
        private GameObject mPlayer;

        private bool mEnemySpawningEnabled = true;

        private DungeonCrawlerGameObjectFactory mGameObjectFactory;
        private GameScene mLevelScene;

        private readonly InputManager<InputAction> mInputManager = new InputManager<InputAction>();
        private IContentManager mContent;
        int mEnemyCount = 0;


        bool firstSpawn = true;
        TimeSpan mNextSpawnTime = TimeSpan.Zero;

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
            mContent = new CachedContentManager(new ForgeContentManager(Services, "Content"));
            this.Content = new XnaProxyContentManager(Services, "Content", mContent);

            // Initialize systems.
            var renderer = new GameRenderer(mGraphicsDevice.GraphicsDevice);
            var debugFont = Content.Load<SpriteFont>(Path.Combine("fonts", "System10.xnb"));
            var debugOverlay = new StandardDebugOverlay(debugFont);

            GameRoot.Initialize(
                renderer,
                debugOverlay,
                new Forge.Settings.ForgeSettings());
            Screen.Initialize( mGraphicsDevice.GraphicsDevice );

            // Initialize default game level.
            mGameObjectFactory = new DungeonCrawlerGameObjectFactory(mContent);

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

            mInputManager.AddDirectionalAction(InputAction.MoveCamera, Keys.I, DirectionName.North);
            mInputManager.AddDirectionalAction(InputAction.MoveCamera, Keys.L, DirectionName.East);
            mInputManager.AddDirectionalAction(InputAction.MoveCamera, Keys.K, DirectionName.South);
            mInputManager.AddDirectionalAction(InputAction.MoveCamera, Keys.J, DirectionName.West);

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
            mLevelScene = new GameScene(GenerateMap(64, 64));

            // Create the player blue print.
            mPlayer = mGameObjectFactory.Instantiate(mLevelScene, "Player");
            mPlayer.Transform.WorldPosition = new Forge.Vector2(200, 200);

            mLevelScene.Add(mPlayer);

            // Create a follow camera to follow the player.
            mLevelScene.MainCamera = new FollowCamera(new SizeF(Screen.Width, Screen.Height), mPlayer);

            // Now that we have loaded the game's contents, we should force a garbage collection
            // before proceeding to play mode.
            GC.Collect();
        }

        /// <summary>
        ///  TEMP HACK
        /// </summary>
        private TileMap GenerateMap(int cols, int rows)
        {
            // Load tileset.
            // TODO: Load this as a content item.
            var tileAtlas = Content.Load<Texture2D>("tiles/dg_dungeon32.png");
            var tileset = new TileSet(tileAtlas, 32, 32);

            tileset.Add(new TileDefinition(0, "void", 160, 192));
            tileset.Add(new TileDefinition(1, "wall", 0, 96));       // or 0, 0
            tileset.Add(new TileDefinition(2, "floor", 192, 160));  // or 192, 160
            tileset.Add(new TileDefinition(3, "door", 96, 0));

            tileset[0].SetIsVoid(true);
            tileset[1].SetIsWall(true);
            tileset[2].SetIsFloor(true);

            // Random generate a map.
            var generator = new DungeonGenerator(tileset);

            generator.Void = tileset[0];
            generator.Wall = tileset[1];
            generator.Floor = tileset[2];

            generator.RoomGenerators.Add(new RoomGenerator()
            {
                FloorTile = generator.Floor,
                WallTile = generator.Wall,
                MinWidth = 4,
                MaxWidth = 16,
                MinHeight = 4,
                MaxHeight = 16
            });

            return generator.Generate(100, 100);
        }

        /// <summary>
        /// TEMP HACK
        /// </summary>
        private void SpawnSkeleton()
        {
            // Spawn the skeleton enemy.
            var skeleton = mGameObjectFactory.Instantiate(mLevelScene, "Skeleton");

            // Pick a spot to place the skeleton enemy.
            var mapWidth = mLevelScene.Tilemap.Width - 128;
            var mapHeight = mLevelScene.Tilemap.Height - 128;

            skeleton.Transform.WorldPosition = new Scott.Forge.Vector2(
                (float) ( GameRoot.Random.NextDouble() * mapWidth + 64),
                (float) ( GameRoot.Random.NextDouble() * mapHeight + 64));

            // Add the newly spawned skeleton to our list of enemies.
            // TODO: Check if location is clear before spawning!
            mLevelScene.Add( skeleton );
            mEnemyCount += 1;

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

            // Camera movement.
            var cameraMovement = mInputManager.GetAxis(InputAction.MoveCamera);

            if (cameraMovement.LengthSquared > 0.01)
            {
                mLevelScene.MainCamera.Translate(cameraMovement * 8.0f);
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
            if ( mNextSpawnTime <= gameTime.TotalGameTime && mEnemyCount < 32 )
            {
                if ((GameRoot.Random.NextDouble() < 0.75 || firstSpawn) && mEnemySpawningEnabled)
                {
                    SpawnSkeleton();
                    firstSpawn = false;
                }

                mNextSpawnTime = gameTime.TotalGameTime.Add( TimeSpan.FromSeconds( 1.0 ) );
            }

            mLevelScene.Update(gameTime);
           
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
            GameRoot.Renderer.StartDrawing(clearScreen: true);
            mLevelScene.Draw(gameTime);
            GameRoot.Renderer.FinishDrawing();

            GameRoot.Debug.Draw( gameTime );

            base.Draw( gameTime );
        }
    }
}