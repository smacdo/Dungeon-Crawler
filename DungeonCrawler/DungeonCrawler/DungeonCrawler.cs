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
        GameObject mEnemy;

        /// <summary>
        /// Constructar
        /// </summary>
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
            Sprite playerBodySprite = new Sprite( Content.Load<SpriteData>( "sprites/Humanoid_Male" ) );
            Sprite playerWeaponSprite = new Sprite( Content.Load<SpriteData>( "sprites/Weapon_Longsword" ), false );
            CharacterSprite playerSprite = new CharacterSprite( playerBodySprite, playerWeaponSprite );

            mPlayer = new GameObject( playerSprite );

            // Create the enemey character
            Sprite skeletonBodySprite = new Sprite( Content.Load<SpriteData>( "sprites/Humanoid_Skeleton" ) );
            Sprite skeletonWeaponSprite = new Sprite( Content.Load<SpriteData>( "sprites/Weapon_Longsword" ), false );

            playerWeaponSprite.Visible = false;

            CharacterSprite skeletonSprite = new CharacterSprite( skeletonBodySprite, skeletonWeaponSprite );

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

            // Actor movement
            if ( keyboard.IsKeyDown( Keys.W ) )
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
            
            // Actor actions
            if ( keyboard.IsKeyDown( Keys.Space ) )
            {
                mPlayer.Actor.SlashAttack();
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

            mPlayer.CharacterSprite.Update( gameTime );
            DrawGameObject( mSpriteBatch, mPlayer );

            mEnemy.CharacterSprite.Update( gameTime );
            DrawGameObject( mSpriteBatch, mEnemy );

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
            DrawSprite( spriteBatch, position, sprite.BodySprite );
            DrawSprite( spriteBatch, position, sprite.WeaponSprite );
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
