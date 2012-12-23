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

using scott.dungeon;
using scott.dungeon.gameobject;
using System.Diagnostics;

namespace scott.dungeon
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class DungeonCrawler : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager mGraphics;
        SpriteBatch mSpriteBatch;
        GameObject mPlayer;
        GameObject mEnemy;

        public DungeonCrawler()
        {
            mGraphics = new GraphicsDeviceManager( this );
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
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            mSpriteBatch = new SpriteBatch( GraphicsDevice );
            Sprite playerSprite = new Sprite( Content.Load<SpriteData>( "maleplayer" ) );
            Sprite skeletonSprite = new Sprite( Content.Load<SpriteData>( "skeleton" ) );

            mPlayer = new GameObject( playerSprite );
            mEnemy = new GameObject( skeletonSprite );
            mEnemy.AI = new AiController( mEnemy );

            mEnemy.Position = new Vector2( 600, 250 );
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void BeginRun()
        {
            
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update( GameTime gameTime )
        {
            // Test user input
            KeyboardState keyboard = Keyboard.GetState();
            GamePadState gamepad = GamePad.GetState( PlayerIndex.One );

            if ( keyboard.IsKeyDown( Keys.Escape ) || gamepad.Buttons.Back == ButtonState.Pressed )
            {
                this.Exit();
            }
            else if ( keyboard.IsKeyDown( Keys.W ) )
            {
                mPlayer.Actor.Move( Direction.North, 50 );
            }
            else if ( keyboard.IsKeyDown( Keys.S ) )
            {
                mPlayer.Actor.Move( Direction.South, 50 );
            }
            else if ( keyboard.IsKeyDown( Keys.A ) )
            {
                mPlayer.Actor.Move( Direction.West, 50 );
            }
            else if ( keyboard.IsKeyDown( Keys.D ) )
            {
                mPlayer.Actor.Move( Direction.East, 50 );
            }

            // Update logic...
            mPlayer.Actor.Update( gameTime );

            mEnemy.AI.Update( gameTime );
            mEnemy.Actor.Update( gameTime );

            mPlayer.Movement.Update( this, gameTime );
            mEnemy.Movement.Update( this, gameTime );

            base.Update( gameTime );
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw( GameTime gameTime )
        {
            GraphicsDevice.Clear( Color.CornflowerBlue );

            // Update animations on all of our sprites and render them
            mSpriteBatch.Begin();

            mPlayer.Sprite.Update( gameTime );
            DrawGameObject( mSpriteBatch, mPlayer );

            mEnemy.Sprite.Update( gameTime );
            DrawGameObject( mSpriteBatch, mEnemy );

            mSpriteBatch.End();

            base.Draw( gameTime );
        }

        private void DrawGameObject( SpriteBatch spriteBatch, GameObject gameObject )
        {
            spriteBatch.Draw( gameObject.Sprite.CurrentAtlasTexture,
                              gameObject.Position,
                              gameObject.Sprite.CurrentAtlasOffset,
                              Color.White );
        }
    }
}
