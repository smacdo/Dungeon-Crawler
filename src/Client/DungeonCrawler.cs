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

namespace Scott.Dungeon.Game
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class DungeonCrawler : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager mGraphicsDevice;
        GameObject mPlayer;
        GameObjectCollection mGameObjects;
        int mEnemyCount = 0;

        /// <summary>
        /// Constructar
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
            GameRoot.Initialize( mGraphicsDevice.GraphicsDevice, Content );
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create the player character
            mPlayer = mGameObjects.Create( "Player" );
            mPlayer.Bounds = new BoundingArea( mPlayer.Position, new Vector2( 30, 50 ), new Vector2( 16, 12 ) );

            SpriteComponent sprite = mGameObjects.Sprites.Create( mPlayer );

            sprite.AssignRootSprite( Content.Load<SpriteData>( "sprites/Humanoid_Male" ) );

            sprite.AddSprite( "Torso",    Content.Load<SpriteData>( "sprites/Torso_Armor_Leather" ) );
            sprite.AddSprite( "Legs",     Content.Load<SpriteData>( "sprites/Legs_Pants_Green" ) );
            sprite.AddSprite( "Feet",     Content.Load<SpriteData>( "sprites/Feet_Shoes_Brown" ) );
            sprite.AddSprite( "Head",     Content.Load<SpriteData>( "sprites/Head_Helmet_Chain" ) );
            sprite.AddSprite( "Bracer",   Content.Load<SpriteData>( "sprites/Bracer_Leather" ) );
            sprite.AddSprite( "Shoulder", Content.Load<SpriteData>( "sprites/Shoulder_Leather" ) );
            sprite.AddSprite( "Belt",     Content.Load<SpriteData>( "sprites/Belt_Leather" ) );
 //           mPlayer.AddChild( CreateBodyPart( "Weapon",    "sprites/Weapon_Longsword", false ) );

            mGameObjects.Movements.Create( mPlayer );
            mGameObjects.ActorControllers.Create( mPlayer );
            mGameObjects.Colliders.Create( mPlayer );
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

            Vector2 position = new Vector2( (int) ( GameRoot.Random.NextDouble() * 600.0 ),
                                            (int) ( GameRoot.Random.NextDouble() * 400.0 ) );

            GameObject enemy = mGameObjects.Create( "Skeleton" + mEnemyCount,
                                                    position,
                                                    Direction.South );

            enemy.Bounds = new BoundingArea( enemy.Position, new Vector2( 30, 50 ), new Vector2( 16, 12 ) );

            SpriteComponent sprite = mGameObjects.Sprites.Create( enemy );

            sprite.AssignRootSprite( Content.Load<SpriteData>( "sprites/Humanoid_Skeleton" ) );

            mGameObjects.ActorControllers.Create( enemy );
            mGameObjects.AiControllers.Create( enemy );
            mGameObjects.Movements.Create( enemy );

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

            // Test user input
            KeyboardState keyboard = Keyboard.GetState();
            GamePadState gamepad = GamePad.GetState( PlayerIndex.One );

            if ( keyboard.IsKeyDown( Keys.Escape ) || gamepad.Buttons.Back == ButtonState.Pressed )
            {
                this.Exit();
            }

            if ( keyboard.IsKeyDown( Keys.F2 ) )
            {
                Console.WriteLine( "Game Object Debugging Information" );
                Console.WriteLine( "=================================" );
                Console.WriteLine( mGameObjects.DumpDebugInfoToString() );
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

            // Player movement
            MovementComponent movement = mPlayer.GetComponent<MovementComponent>();
            ActorController playerActor = mPlayer.GetComponent<ActorController>();

            if ( keyboard.IsKeyDown( Keys.W ) )
            {
                movement.Move( Direction.North, 125 );
            }
            else if ( keyboard.IsKeyDown( Keys.S ) )
            {
                movement.Move( Direction.South, 125 );
            }
            else if ( keyboard.IsKeyDown( Keys.A ) )
            {
                movement.Move( Direction.West, 125 );
            }
            else if ( keyboard.IsKeyDown( Keys.D ) )
            {
                movement.Move( Direction.East, 125 );
            }
            
            // Actor actions
            if ( keyboard.IsKeyDown( Keys.Space ) )
            {
                playerActor.SlashAttack();
            }

            // Update the world
            mGameObjects.Update( gameTime );

            base.Update( gameTime );
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw( GameTime gameTime )
        {
            // Walk through the game scene and collect all sprites for drawing
            mGameObjects.Sprites.Update( gameTime );

            foreach ( SpriteComponent sprite in mGameObjects.Sprites )
            {
                sprite.Draw( gameTime );
            }

            // Draw all requested game sprites
            GameRoot.Renderer.DrawScreen( gameTime );
            GameRoot.Debug.Draw( gameTime );

            base.Draw( gameTime );
        }
    }
}
