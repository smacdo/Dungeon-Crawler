using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Scott.Dungeon;
using Scott.Game;
using Scott.Game.Entity;
using Scott.GameContent;
using Scott.Game.Graphics;
using Scott.Game.Entity.Actor;

using System.Diagnostics;
using Scott.Geometry;
using Scott.Game.Entity.Graphics;
using Scott.Game.Entity.Movement;
using Scott.Game.Entity.AI;
using Scott.Game.Input;

namespace Scott.Dungeon.Game
{
    /// <summary>
    ///  This is the main class for Dungeon Crawler. It is responsible for running the game loop,
    ///  responding to external events and managing content loading/unloading.
    /// </summary>
    public class DungeonCrawler : Microsoft.Xna.Framework.Game
    {
        enum InputAction
        {
            ExitGame,
            PrintDebugInfo,
            Move,
            Attack
        }

        private GraphicsDeviceManager mGraphicsDevice;
        private GameObject mPlayer;
        private GameObjectCollection mGameObjects;
        private InputManager<InputAction> mInputManager = new InputManager<InputAction>();
        int mEnemyCount = 0;

        /// <summary>
        ///  Constructor.
        /// </summary>
        public DungeonCrawler()
        {
            mGraphicsDevice = new GraphicsDeviceManager( this );
            mGameObjects = new GameObjectCollection();
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Let XNA engine initialize first.
            base.Initialize();
            
            // Initialize systems.
            GameRoot.Initialize( mGraphicsDevice.GraphicsDevice, Content );
            Screen.Initialize( mGraphicsDevice.GraphicsDevice );

            // Initialize input system with default settings.
            mInputManager.AddAction( InputAction.ExitGame, Keys.Escape );
            mInputManager.AddAction( InputAction.PrintDebugInfo, Keys.F2 );
            mInputManager.AddAction( InputAction.Attack, Keys.Space );
            mInputManager.AddDirectionalAction( InputAction.Move, Keys.W, Direction.North );
            mInputManager.AddDirectionalAction( InputAction.Move, Keys.D, Direction.East );
            mInputManager.AddDirectionalAction( InputAction.Move, Keys.S, Direction.South );
            mInputManager.AddDirectionalAction( InputAction.Move, Keys.A, Direction.West );
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create the player blue print.
            mPlayer = mGameObjects.Create( "Player" );
            mPlayer.Bounds = new BoundingArea( mPlayer.Position, new Vector2( 30, 50 ), new Vector2( 16, 12 ) );

            SpriteComponent sprite = mGameObjects.Attach<SpriteComponent>( mPlayer ); // mGameObjects.Sprites.Create( mPlayer );

            sprite.AssignRootSprite( Content.Load<SpriteData>( "sprites/Humanoid_Male" ) );

            sprite.AddSprite( "Torso",    Content.Load<SpriteData>( "sprites/Torso_Armor_Leather" ) );
            sprite.AddSprite( "Legs",     Content.Load<SpriteData>( "sprites/Legs_Pants_Green" ) );
            sprite.AddSprite( "Feet",     Content.Load<SpriteData>( "sprites/Feet_Shoes_Brown" ) );
            sprite.AddSprite( "Head",     Content.Load<SpriteData>( "sprites/Head_Helmet_Chain" ) );
            sprite.AddSprite( "Bracer",   Content.Load<SpriteData>( "sprites/Bracer_Leather" ) );
            sprite.AddSprite( "Shoulder", Content.Load<SpriteData>( "sprites/Shoulder_Leather" ) );
            sprite.AddSprite( "Belt",     Content.Load<SpriteData>( "sprites/Belt_Leather" ) );

            mGameObjects.Attach<MovementComponent>( mPlayer );
            mGameObjects.Attach<ActorController>( mPlayer );
            mGameObjects.Attach<ColliderComponent>( mPlayer );
        }

        /// <summary>
        /// TEMP HACK
        /// </summary>
        private void SpawnSkeleton()
        {
            if ( mEnemyCount > 0 )
            {
                return;
            }

            Vector2 position = new Vector2( (int) ( GameRoot.Random.NextDouble() * Screen.Width ),
                                            (int) ( GameRoot.Random.NextDouble() * Screen.Height ) );

            GameObject enemy = mGameObjects.Create( "Skeleton" + mEnemyCount,
                                                    position,
                                                    Direction.South );

            enemy.Bounds = new BoundingArea( enemy.Position, new Vector2( 30, 50 ), new Vector2( 16, 12 ) );

            SpriteComponent sprite = mGameObjects.Attach<SpriteComponent>( enemy );

            sprite.AssignRootSprite( Content.Load<SpriteData>( "sprites/Humanoid_Skeleton" ) );

            mGameObjects.Attach<ActorController>( enemy );
            mGameObjects.Attach<AiController>( enemy );
            mGameObjects.Attach<MovementComponent>( enemy );

            GameRoot.Enemies.Add( enemy );
            mEnemyCount += 1;
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
            GameRoot.Renderer.ClearQueuedItems();
            GameRoot.Debug.PreUpdate( gameTime );

            // Perform any requested actions based on user input.
            mInputManager.Update();

            if ( mInputManager.WasTriggered( InputAction.ExitGame ) )
            {
                this.Exit();
            }

            if ( mInputManager.WasTriggered( InputAction.PrintDebugInfo ) )
            {
                Console.WriteLine( "Game Object Debugging Information" );
                Console.WriteLine( "=================================" );
                Console.WriteLine( mGameObjects.DumpDebugInfoToString() );
            }

            // Player movement.
            MovementComponent movement = mPlayer.GetComponent<MovementComponent>();
            ActorController playerActor = mPlayer.GetComponent<ActorController>();
            Direction playerDirection;

            if ( mInputManager.WasTriggered( InputAction.Move, out playerDirection ) )
            {
                movement.Move( playerDirection, 125 );
            }

            if ( mInputManager.WasTriggered( InputAction.Attack ) )
            {
                playerActor.SlashAttack();
            }

            // Spawn some stuff
            if ( mNextSpawnTime <= gameTime.TotalGameTime )
            {
                if ( GameRoot.Random.NextDouble() < 0.75 || firstSpawn )
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
            mGameObjects.Update<MovementComponent>( gameTime );
            mGameObjects.Update<ColliderComponent>( gameTime );

            // Update game ai and character actions
            mGameObjects.Update<AiController>( gameTime );
            mGameObjects.Update<ActorController>( gameTime );

            // Make sure animations are primed and updated (we need to trigger the
            // correct animation events even if we are not drawwing)
            mGameObjects.Update<SpriteComponent>( gameTime );   // TODO: Remove this and have an animation component

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
            // Walk through the game scene and collect all sprites for drawing
            mGameObjects.Draw<SpriteComponent>( gameTime );

            // Draw all requested game sprites
            GameRoot.Renderer.DrawScreen( gameTime );
            GameRoot.Debug.Draw( gameTime );

            base.Draw( gameTime );
        }
    }
}