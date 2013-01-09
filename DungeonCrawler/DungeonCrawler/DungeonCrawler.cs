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
using Scott.Dungeon.ComponentModel;
using Scott.Dungeon.Data;
using Scott.Dungeon.Graphics;
using Scott.Dungeon.Actor;
using Scott.Dungeon.AI;

using System.Diagnostics;

namespace Scott.Dungeon.Game
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class DungeonCrawler : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager mGraphicsDevice;
        GameObject mPlayer;
        Random mRandom = new Random();
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

            CharacterSprite playerSprite = mGameObjects.CharacterSprites.Create( mPlayer );

            playerSprite.Body = new Sprite( Content.Load<SpriteData>( "sprites/Humanoid_Male" ) );
            playerSprite.Torso = new Sprite( Content.Load<SpriteData>( "sprites/Torso_Armor_Leather" ) );
            playerSprite.Legs = new Sprite( Content.Load<SpriteData>( "sprites/Legs_Pants_Green" ) );
            playerSprite.Feet = new Sprite( Content.Load<SpriteData>( "sprites/Feet_Shoes_Brown" ) );
            playerSprite.Head = new Sprite( Content.Load<SpriteData>( "sprites/Head_Helmet_Chain" ) );
            playerSprite.Hands = new Sprite( Content.Load<SpriteData>( "sprites/Bracer_Leather" ) );
            playerSprite.Shoulder = new Sprite( Content.Load<SpriteData>( "sprites/Shoulder_Leather" ) );
            playerSprite.Belt = new Sprite( Content.Load<SpriteData>( "sprites/Belt_Leather" ) );
            playerSprite.Weapon = new Sprite( Content.Load<SpriteData>( "sprites/Weapon_Longsword" ), false );

            Collider collider = mGameObjects.Colliders.Create( mPlayer );
            collider.Set( new Rectangle( 16, 8, 32, 52 ) );

            mGameObjects.Movements.Create( mPlayer );
            mGameObjects.ActorControllers.Create( mPlayer );
        }

        /// <summary>
        /// TEMP HACK
        /// </summary>
        private void SpawnSkeleton()
        {
            Vector2 position = new Vector2( (int) ( mRandom.NextDouble() * 600.0 ),
                                            (int) ( mRandom.NextDouble() * 400.0 ) );
            GameObject enemy = mGameObjects.Create( "Skeleton" + mEnemyCount,
                                                    position,
                                                    Direction.South );

            CharacterSprite skeletonSprite = mGameObjects.CharacterSprites.Create( enemy );

            skeletonSprite.Body = new Sprite( Content.Load<SpriteData>( "sprites/Humanoid_Skeleton" ) );

            Collider collider = mGameObjects.Colliders.Create( enemy );
            collider.Set( new Rectangle( 16, 8, 32, 52 ) );

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

        protected override void BeginRun()
        {
            
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
                if ( mRandom.NextDouble() < 0.75 || firstSpawn )
                {
                    SpawnSkeleton();
                    firstSpawn = false;
                }

                mNextSpawnTime = gameTime.TotalGameTime.Add( TimeSpan.FromSeconds( 1.0 ) );
            }

            // Player movement
            ActorController playerActor = mPlayer.GetComponent<ActorController>();

            if ( keyboard.IsKeyDown( Keys.W ) )
            {
                playerActor.Move( Direction.North, 125 );
            }
            else if ( keyboard.IsKeyDown( Keys.S ) )
            {
                playerActor.Move( Direction.South, 125 );
            }
            else if ( keyboard.IsKeyDown( Keys.A ) )
            {
                playerActor.Move( Direction.West, 125 );
            }
            else if ( keyboard.IsKeyDown( Keys.D ) )
            {
                playerActor.Move( Direction.East, 125 );
            }
            
            // Actor actions
            if ( keyboard.IsKeyDown( Keys.Space ) )
            {
                playerActor.SlashAttack();
            }

            // Update game ai and character actions
            mGameObjects.AiControllers.Update( gameTime );
            mGameObjects.ActorControllers.Update( gameTime );

            // Now update movement
            mGameObjects.Movements.Update( gameTime );

            // Resolve collisions
            mGameObjects.Colliders.Update( gameTime );

            base.Update( gameTime );
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw( GameTime gameTime )
        {
            // Update animations on all of our sprites and then have them sent to the
            // sprite renderer for drawing
            mGameObjects.CharacterSprites.Update( gameTime );

            // Draw all requested game sprites
            GameRoot.Renderer.DrawScreen( gameTime );
            GameRoot.Debug.Draw( gameTime );

            base.Draw( gameTime );
        }
    }
}
