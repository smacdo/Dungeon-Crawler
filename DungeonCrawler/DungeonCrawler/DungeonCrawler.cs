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
        GraphicsDeviceManager mGraphics;
        SpriteBatch mSpriteBatch;
        GameObject mPlayer;
        Random mRandom = new Random();
        GameObjectCollection mGameObjects;

        /// <summary>
        /// A simple 1x1 texture that can be arbitrarily colored and stretched. Perfect for little boxes
        /// </summary>
        private Texture2D mSinglePIxel;

        /// <summary>
        /// Constructar
        /// </summary>
        public DungeonCrawler()
        {
            mGraphics = new GraphicsDeviceManager( this );
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
            mSinglePIxel = new Texture2D( GraphicsDevice, 1, 1 );
            mSinglePIxel.SetData( new[] { Color.White } );

            GameRoot.Initialize( mGraphics.GraphicsDevice, Content );

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            mSpriteBatch = new SpriteBatch( GraphicsDevice );

            // Create the player character
            mPlayer = mGameObjects.Create();

            CharacterSprite playerSprite = mGameObjects.CharacterSprites.Create( mPlayer );
            mPlayer.CharacterSprite = playerSprite; // XXX TEMP HACK

            playerSprite.Body = new Sprite( Content.Load<SpriteData>( "sprites/Humanoid_Male" ) );
            playerSprite.Torso = new Sprite( Content.Load<SpriteData>( "sprites/Torso_Armor_Leather" ) );
            playerSprite.Legs = new Sprite( Content.Load<SpriteData>( "sprites/Legs_Pants_Green" ) );
            playerSprite.Feet = new Sprite( Content.Load<SpriteData>( "sprites/Feet_Shoes_Brown" ) );
            playerSprite.Head = new Sprite( Content.Load<SpriteData>( "sprites/Head_Helmet_Chain" ) );
            playerSprite.Hands = new Sprite( Content.Load<SpriteData>( "sprites/Bracer_Leather" ) );
            playerSprite.Shoulder = new Sprite( Content.Load<SpriteData>( "sprites/Shoulder_Leather" ) );
            playerSprite.Belt = new Sprite( Content.Load<SpriteData>( "sprites/Belt_Leather" ) );
            playerSprite.Weapon = new Sprite( Content.Load<SpriteData>( "sprites/Weapon_Longsword" ), false );
        }

        /// <summary>
        /// TEMP HACK
        /// </summary>
        private void SpawnSkeleton()
        {
            Vector2 position = new Vector2( (int) ( mRandom.NextDouble() * 600.0 ),
                                            (int) ( mRandom.NextDouble() * 400.0 ) );
            GameObject enemy = mGameObjects.Create( position, Direction.South, 32, 32 );

            CharacterSprite skeletonSprite = mGameObjects.CharacterSprites.Create( enemy );
            enemy.CharacterSprite = skeletonSprite; // XXX TEMP HACK

            skeletonSprite.Body = new Sprite( Content.Load<SpriteData>( "sprites/Humanoid_Skeleton" ) );

            AiController ai = mGameObjects.AiControllers.Create( enemy );

            GameRoot.Enemies.Add( enemy );
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

            // Actor movement
            if ( keyboard.IsKeyDown( Keys.W ) )
            {
                mPlayer.Actor.Move( Direction.North, 125 );
            }
            else if ( keyboard.IsKeyDown( Keys.S ) )
            {
                mPlayer.Actor.Move( Direction.South, 125 );
            }
            else if ( keyboard.IsKeyDown( Keys.A ) )
            {
                mPlayer.Actor.Move( Direction.West, 125 );
            }
            else if ( keyboard.IsKeyDown( Keys.D ) )
            {
                mPlayer.Actor.Move( Direction.East, 125 );
            }
            
            // Actor actions
            if ( keyboard.IsKeyDown( Keys.Space ) )
            {
                mPlayer.Actor.SlashAttack();
            }

            // Update game ai and character actions
            mGameObjects.AiControllers.Update( gameTime );
            mPlayer.Actor.Update( gameTime );
            
            foreach ( GameObject obj in GameRoot.Enemies )
            {
                obj.Actor.Update( gameTime );
            }

            // Now update movement
            mPlayer.Movement.Update( this, gameTime );

            foreach ( GameObject obj in GameRoot.Enemies )
            {
                obj.Movement.Update( this, gameTime );
            }

            base.Update( gameTime );
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw( GameTime gameTime )
        {
            GraphicsDevice.Clear( Color.CornflowerBlue );
            mSpriteBatch.Begin();

            // Update animations on all of our sprites and render them
//            mPlayer.CharacterSprite.Update( gameTime );
            mGameObjects.CharacterSprites.Update( gameTime );

            DrawGameObject( mSpriteBatch, mPlayer );

            foreach ( GameObject obj in GameRoot.Enemies )
            {
//                obj.CharacterSprite.Update( gameTime );
                DrawGameObject( mSpriteBatch, obj );
            }


            mSpriteBatch.End();

            GameRoot.Debug.Draw( gameTime );

            // Detect if the game is running slowly, and if so draw an indicator
            mSpriteBatch.Begin();
            if ( gameTime.IsRunningSlowly )
            {
                mSpriteBatch.Draw( mSinglePIxel, new Rectangle( 0, 0, 20, 20 ), Color.Red );
            }

            mSpriteBatch.End();
            base.Draw( gameTime );
        }


        // ------------ vvvv should be put into a renderer vvvv ----------------------------- //
        private void DrawGameObject( SpriteBatch spriteBatch, GameObject gameObject )
        {
//            if ( )
            {
                DrawCharacterSprite( spriteBatch, gameObject.Position,  gameObject.CharacterSprite );
            }
//            else
//            {
//                DrawSprite( spriteBatch, gameObject.Position, gameObject.Sprite );
//            }
        }

        private void DrawCharacterSprite( SpriteBatch spriteBatch, Vector2 position, CharacterSprite sprite )
        {
            // TODO: Get an ordered list of sprites to draw
            //  (for the moment this will work)
            int spriteIndex = (int) CharacterSprite.SubSpriteIndex.Max;

            for ( int i = 0; i < spriteIndex; ++i )
            {
                Sprite s = sprite.SubSprites[i];

                if ( s != null )
                {
                    DrawSprite( spriteBatch, position, s );
                }
            }
        }

        private void DrawSprite( SpriteBatch spriteBatch, Vector2 position, Sprite sprite )
        {
            if ( sprite.Visible )
            {
                spriteBatch.Draw( sprite.CurrentAtlasTexture,
                                  position + sprite.DrawOffset,
                                  sprite.CurrentAtlasOffset,
                                  Color.White );
            }
        }
    }
}
