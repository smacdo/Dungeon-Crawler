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
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Scott.DungeonCrawler.Actions;
using Forge;
using Forge.Actors;
using Forge.Input;
using Forge.GameObjects;
using Forge.Graphics;
using Forge.Content;
using Forge.Tilemaps;
using Scott.DungeonCrawler.WorldGeneration;
using Scott.DungeonCrawler.Levels;
using Scott.DungeonCrawler.Blueprints;

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
        private Random mWorldRandom = new Random();

        private IGameRenderer mRenderer;
        private DungeonCrawlerBlueprintFactory mGameObjectFactory;
        private GameScene mCurrentScene;
        private DungeonLevel mCurrentLevel;

        private readonly InputManager<InputAction> mInputManager = new InputManager<InputAction>();
        int mEnemyCount = 0;

        bool mFirstSpawn = true;
        TimeSpan mNextSpawnTime = TimeSpan.Zero;

        public new IContentManager Content { get; private set; }

        /// <summary>
        ///  Constructor.
        /// </summary>
        public DungeonCrawlerClient(IContentManager contentManager)
        {
            if (contentManager == null)
            {
                throw new ArgumentNullException(nameof(contentManager));
            }

            Content = contentManager;

            // TODO: Provide game renderer as input to constructor.
            mGraphicsDevice = new GraphicsDeviceManager( this );
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Set display size.
            mRenderer = new GameRenderer(mGraphicsDevice);
            mRenderer.Resize(800, 600);

            // Configure legacy XNA content loader.
            base.Content = new Microsoft.Xna.Framework.Content.ContentManager(Services);
            base.Content.RootDirectory = "Content";     // TODO: Make configurable via content container.

            Content.XnaContentManager = base.Content;
            
            // Initialize systems.
            var debugFont = Content.Load<SpriteFont>(Path.Combine("fonts", "System10.xnb")).Result;
            var debugOverlay = new StandardDebugOverlay(debugFont);

            Globals.Initialize(
                debugOverlay,
                new Forge.Settings.ForgeSettings());

            Screen.Initialize( mGraphicsDevice.GraphicsDevice );

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
            // Create a new level.
            mCurrentLevel = GenerateMap(100, 100);
            mCurrentScene = new GameScene(mCurrentLevel.TileMap);

            mGameObjectFactory = new DungeonCrawlerBlueprintFactory(Content, mCurrentScene);

            // Spawn player into level.
            var position = mCurrentLevel.TileMap.GetWorldPositionForTile(mCurrentLevel.StairsUpPoint);
            mPlayer = mGameObjectFactory.Spawn(BlueprintNames.Player, "Player", position).Result;    // TODO: async support.
                
            // Spawn a bunch of skeletons to get started.
            for (int i = 0; i < 100; i++)
            {
                SpawnSkeleton();
            }

            // Create a follow camera to follow the player.
            mCurrentScene.MainCamera = new FollowCamera(new SizeF(Screen.Width, Screen.Height), mPlayer);

            // Now that we have loaded the game's contents, we should force a garbage collection
            // before proceeding to play mode.
            GC.Collect();
        }

        /// <summary>
        ///  TEMP HACK
        /// </summary>
        private DungeonLevel GenerateMap(int cols, int rows)
        {
            const int TileSize = 32;

            // Load tileset.
            // TODO: Load this as a content item.
            var tileAtlas = Content.Load<Texture2D>("tiles/dg_dungeon32.png").Result;
            var tileset = new TileSet(tileAtlas, TileSize, TileSize); 

            tileset.Add(new TileDefinition(0, "void",  5 * TileSize, 6 * TileSize));
            tileset.Add(new TileDefinition(1, "wall",  0 * TileSize, 3 * TileSize));
            tileset.Add(new TileDefinition(2, "floor", 6 * TileSize, 5 * TileSize));
            tileset.Add(new TileDefinition(3, "door",  3 * TileSize, 0 * TileSize));

            tileset[0].SetIsVoid(true);
            tileset[1].SetIsWall(true);
            tileset[2].SetIsFloor(true);

            // Random generate a map.
            var generator = new DungeonGenerator(tileset);

            generator.Void = tileset[0];
            generator.Wall = tileset[1];
            generator.Floor = tileset[2];
            generator.Doorway = tileset[3];

            generator.RoomGenerators.Add(new RoomGenerator()
            {
                FloorTile = generator.Floor,
                WallTile = generator.Wall,
                MinWidth = 4,
                MaxWidth = 16,
                MinHeight = 4,
                MaxHeight = 16
            });

            return generator.Generate(cols, rows);
        }

        private void SpawnSkeleton()
        {
            // Pick a spot to place the skeleton enemy.
            // TODO: Check if location is clear before spawning!
            var mapWidth = mCurrentScene.Tilemap.Width - 128;
            var mapHeight = mCurrentScene.Tilemap.Height - 128;

            var spawnIndex = mEnemyCount % mCurrentLevel.SpawnPoints.Count;
            var position = mCurrentLevel.TileMap.GetWorldPositionForTile(mCurrentLevel.SpawnPoints[spawnIndex]);

            // Spawn the skeleton enemy.
            var skeleton = mGameObjectFactory.Spawn(BlueprintNames.Skeleton, "Skeleton", position);

            // Add the newly spawned skeleton to our list of enemies.
            //  TODO: Temp hack, should not be tracked this way.
            mEnemyCount += 1;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update( GameTime gameTime )
        {
            // Allow game play systems to perform any pre-update logic that might be needed
            Globals.Debug.PreUpdate(gameTime);

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
                mCurrentScene.MainCamera.Translate(cameraMovement * 8.0f);
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
                if ((mWorldRandom.NextDouble() < 0.75 || mFirstSpawn) && mEnemySpawningEnabled)
                {
                    //SpawnSkeleton();
                    mFirstSpawn = false;
                }

                mNextSpawnTime = gameTime.TotalGameTime.Add( TimeSpan.FromSeconds( 1.0 ) );
            }

            mCurrentScene.Update(gameTime);
           
            base.Update( gameTime );

            // Post update
            mInputManager.ClearState();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            mRenderer.StartDrawing(clearScreen: true);
            mCurrentScene.Draw(gameTime, mRenderer);
            mRenderer.FinishDrawing();

            Globals.Debug.Draw(gameTime, mRenderer);

            base.Draw(gameTime);
        }
    }
}