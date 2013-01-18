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

        private GameObject CreateBodyPart( string name, string spriteName, bool visible = true )
        {
            GameObject go = mGameObjects.Create( name );
            AddSprite( go, spriteName, visible );

            return go;
        }

        private void AddSprite( GameObject go, string spriteName, bool visible = true )
        {
            SpriteComponent sprite       = mGameObjects.Sprites.Create( go );
            AnimationComponent animation = mGameObjects.Animations.Create( go );

            animation.AssignAnimationData( Content.Load<SpriteData>( spriteName ) );

            if ( !visible )
            {
                go.Enabled = false;
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create the player character
            mPlayer = mGameObjects.Create( "Player" );
            mPlayer.Bounds = new BoundingArea( new Rectangle( 16, 8, 32, 52 ) );

            AddSprite( mPlayer, "sprites/Humanoid_Male" );

            mPlayer.AddChild( CreateBodyPart( "Body",      "sprites/Torso_Armor_Leather" ) );
            mPlayer.AddChild( CreateBodyPart( "Legs",      "sprites/Legs_Pants_Green" ) );
            mPlayer.AddChild( CreateBodyPart( "Feet",      "sprites/Feet_Shoes_Brown" ) );
            mPlayer.AddChild( CreateBodyPart( "Head",      "sprites/Head_Helmet_Chain" ) );
            mPlayer.AddChild( CreateBodyPart( "Hands",     "sprites/Bracer_Leather" ) );
            mPlayer.AddChild( CreateBodyPart( "Shoulders", "sprites/Shoulder_Leather" ) );
            mPlayer.AddChild( CreateBodyPart( "Belt",      "sprites/Belt_Leather" ) );
            mPlayer.AddChild( CreateBodyPart( "Weapon",    "sprites/Weapon_Longsword", false ) );

            mGameObjects.Movements.Create( mPlayer );
            mGameObjects.ActorControllers.Create( mPlayer );
        }

        /// <summary>
        /// TEMP HACK
        /// </summary>
        private void SpawnSkeleton()
        {
            if ( mEnemyCount > 128 )
            {
                return;
            }

            Vector2 position = new Vector2( (int) ( mRandom.NextDouble() * 600.0 ),
                                            (int) ( mRandom.NextDouble() * 400.0 ) );

            GameObject enemy = mGameObjects.Create( "Skeleton" + mEnemyCount,
                                                    position,
                                                    Direction.South );

            enemy.Bounds = new BoundingArea( new Rectangle( 16, 8, 32, 52 ) );

            AddSprite( enemy, "sprites/Humanoid_Skeleton" );

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

            // Make sure animations are primed and updated (we need to trigger the
            // correct animation events even if we are not drawwing)
            mGameObjects.Animations.Update( gameTime );

            base.Update( gameTime );
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw( GameTime gameTime )
        {
            // Walk through the game scene and collect all sprites
            mGameObjects.Sprites.Update( gameTime );

            // Draw all requested game sprites
            GameRoot.Renderer.DrawScreen( gameTime );
            GameRoot.Debug.Draw( gameTime );

            base.Draw( gameTime );
        }
    }
}
