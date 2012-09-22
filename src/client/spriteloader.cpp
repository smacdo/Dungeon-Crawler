/*
 * Copyright 2012 Scott MacDonald
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
#include "client/spriteloader.h"
#include "client/sprite.h"
#include "client/spritemanager.h"

#include "common/logging.h"

#include <tinyxml/tinyxml.h>
#include <string>
#include <sstream>

/**
 * Loads sprites and hands them to the sprite manager for caching and eventual
 * display
 */
SpriteLoader::SpriteLoader( SpriteManager& spriteManager )
    : mSpriteManager( spriteManager ),
      mErrorText( "" ),
      mExactWidth( 0 ),
      mExactHeight( 0 )
{

}

/**
 * Sprite loader destructor
 */
SpriteLoader::~SpriteLoader()
{
}

/**
 * Loads sprite images from the requested xml file
 */
void SpriteLoader::loadSpritesFromXml( const std::string& filename )
{
    TiXmlDocument doc( filename );
    
    // Load the XML file into memory
    LOG_INFO("Loader") << "Loading spritesheet from " << filename;

    if (! doc.LoadFile() )
    {
        raiseError( NULL, "Failed to load XML file: " + filename );
        return;
    }

    // Instantiate an XML parser to parse this loaded xml string data, and
    // then grab the topmost tag <spritesheet>
    TiXmlElement * pRoot = doc.FirstChildElement( "spritesheet" );

    if ( pRoot == NULL )
    {
        raiseError( NULL, "Failed to find a <spritesheet> element" );
    }
    else
    {
        readSpriteSheetNode( pRoot );
    }

}

/**
 * Parses a <spritesheet ..> element, and for will create a sprite template
 * (in the sprite manager) for every <sprite ...> element it finds
 *
 * \param  pNode  The <spritesheet ...> XML node
 */
void SpriteLoader::readSpriteSheetNode( const TiXmlElement *pNode )
{
    assert( pNode != NULL );

    // Load the sprite sheet file name
    std::string spriteSheetFile;

    if ( pNode->QueryStringAttribute( "file", &spriteSheetFile ) != TIXML_SUCCESS )
    {
        raiseError( pNode,
                    "Failed to find 'file' attribute for sprite sheet tag" );
        return;
    }

    // Iterate through every child element of the current <spritesheet>
    // container and read every <sprite>
    const TiXmlElement *pCurrentNode = pNode->FirstChildElement( "sprite" );

    while ( pCurrentNode != NULL && (! hasErrors() ) )
    {
        // First read the node
        readSpriteNode( spriteSheetFile, pCurrentNode );

        // Then move to the next node
        pCurrentNode = pCurrentNode->NextSiblingElement( "sprite" );
    }
}

/**
 * Parses a <sprite ...> element and proceeds to load it into this sprite
 * loader's sprite manager
 */
void SpriteLoader::readSpriteNode( const std::string& spriteSheetFile,
                                   const TiXmlElement * pNode )
{
    assert( pNode != NULL );

    // Read sprite name
    std::string name;

    if ( pNode->QueryStringAttribute( "name", &name ) )
    {
        raiseError( pNode, "Failed reading sprite name" );
        return;
    }

    // Read X and Y attributes
    int x = 0, y = 0;

    if ( pNode->QueryIntAttribute( "x", &x ) != TIXML_SUCCESS ||
         pNode->QueryIntAttribute( "y", &y ) != TIXML_SUCCESS )
    {
        raiseError( pNode, "Failed reading x/y values. Missing or invalid" );
        return;
    }
    

    // Read the sprite width and height
    int w = 0, h = 0;

    if ( pNode->QueryIntAttribute( "w", &w ) != TIXML_SUCCESS ||
         pNode->QueryIntAttribute( "h", &h ) != TIXML_SUCCESS )
    {
        raiseError( pNode, "Failed reading w/h values. Missing or invalid" );
        return;
    }

    // Check if there are exact width or heights. If so, then verify that this
    // sprite's width and height match
    if ( ( mExactWidth  > 0 && w != mExactWidth  ) ||
         ( mExactHeight > 0 && h != mExactHeight ) )
    {
        raiseError( pNode,
                    "Sprite's width or height does not match what is "
                    "expected" );
        return;
    }

    // Now that we've read all of the required sprite attributes, lets create
    // a new sprite template in our sprite manager
    mSpriteManager.addSpriteData( name,
                                  spriteSheetFile,
                                  x,
                                  y,
                                  w,
                                  h );
}

/**
 * Internal method that raises an error, and can therefore be reported back
 * to the user.
 *
 * This method keeps track of the position of the xml node being read which
 * makes error reporting a little nicer
 *
 * \param  pNode    The XML node that triggered an error condition (NULL for none)
 * \param  message  The error message to report to the user
 */
void SpriteLoader::raiseError( const TiXmlElement * pNode,
                               const std::string& message )
{
    std::ostringstream ss;

    // Either append the message to an already existing error text, or just
    // add it for the first time
    if ( hasErrors() )
    {
        ss << mErrorText << "\n" << message;
    }
    else
    {
        ss << message;
    }

    // If we were provided an xml node that triggered the error condition,
    // print it's information out
    if ( pNode != NULL )
    {
        ss << " ( line " << pNode->Row() 
           << ", col "  << pNode->Column()
           << " ) ";
    }

    // Store the new error information
    mErrorText = ss.str();
}

/**
 * Raises an error inside of the sprite loader, so that it can be queried
 * and reported back to the user
 *
 * \return  True if there were errors while loading, false otherwise
 */
bool SpriteLoader::hasErrors() const
{
    return (! mErrorText.empty() );
}

/**
 * Returns the text of any errors that were raised while loading a sprite
 * sheet data file
 */
std::string SpriteLoader::errorText() const
{
    return mErrorText;
}
