using Microsoft.Xna.Framework;
using Scott.Game.Content;
using Scott.Game.Entity;
using Scott.Game.Entity.Actor;
using Scott.Game.Entity.AI;
using Scott.Game.Entity.Graphics;
using Scott.Game.Entity.Movement;
using Scott.Game.Graphics;
using Scott.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Dungeon.Game
{
    /// <summary>
    ///  Temporary blueprint provider. This is in place until we create a real one that actually
    ///  reads blueprints from a file.
    /// </summary>
    public class TempBlueprintProvider : IBlueprintFactory
    {
        public GameObjectCollection Collection { get; set; }
        public ContentManagerX Content { get; set; }

        public TempBlueprintProvider( ContentManagerX content )
        {
            Content = content;
        }

        /// <summary>
        ///  Create a game object from the named blueprint.
        /// </summary>
        /// <param name="blueprintName"></param>
        /// <returns></returns>
        public GameObject Instantiate( string blueprintName )
        {
            if ( blueprintName == "Player" )
            {
                return InstantiatePlayer();
            }
            else if ( blueprintName == "Skeleton" )
            {
                return InstantiateSkeleton();
            }
            else
            {
                throw new ArgumentException(
                    "Could not locate blueprint {0}".With( blueprintName ) );
            }
        }

        private GameObject InstantiatePlayer()
        {
            // Create the player blue print.
            GameObject player = Collection.Create( "Player" );

            // Create the sprite and set it up.
            SpriteComponent sprite = Collection.Attach<SpriteComponent>( player );

            sprite.AssignRootSprite( Content.Load<SpriteData>( "sprites/Humanoid_Male" ) );

            sprite.AddLayer( "Torso", Content.Load<SpriteData>( "sprites/Torso_Armor_Leather" ) );
            sprite.AddLayer( "Legs", Content.Load<SpriteData>( "sprites/Legs_Pants_Green" ) );
            sprite.AddLayer( "Feet", Content.Load<SpriteData>( "sprites/Feet_Shoes_Brown" ) );
            sprite.AddLayer( "Head", Content.Load<SpriteData>( "sprites/Head_Helmet_Chain" ) );
            sprite.AddLayer( "Bracer", Content.Load<SpriteData>( "sprites/Bracer_Leather" ) );
            sprite.AddLayer( "Shoulder", Content.Load<SpriteData>( "sprites/Shoulder_Leather" ) );
            sprite.AddLayer( "Belt", Content.Load<SpriteData>( "sprites/Belt_Leather" ) );
            sprite.AddLayer( "Weapon", Content.Load<SpriteData>( "sprites/Weapon_Longsword" ), false );

            // Add movement component.
            MovementComponent movement =  Collection.Attach<MovementComponent>( player );
            movement.MoveBox = new RectangleF( new Vector2( 15, 32 ), new Vector2( 32, 32 ) );

            // Add actor component.
            Collection.Attach<ActorController>( player );

            return player;
        }

        private GameObject InstantiateSkeleton()
        {
            GameObject enemy = Collection.Create( "Skeleton" );
            SpriteComponent sprite = Collection.Attach<SpriteComponent>( enemy );

            sprite.AssignRootSprite( Content.Load<SpriteData>( "sprites/Humanoid_Skeleton" ) );

            Collection.Attach<ActorController>( enemy );
            Collection.Attach<AiController>( enemy );

            MovementComponent movement =  Collection.Attach<MovementComponent>( enemy );
            movement.MoveBox = new RectangleF( new Vector2( 15, 32 ), new Vector2( 32, 32 ) );

            return enemy;
        }
    }
}
