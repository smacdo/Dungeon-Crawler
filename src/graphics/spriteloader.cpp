/**
 * Loads sprites and hands them to the sprite manager for caching and eventual
 * display
 */
SpriteLoader::SpriteLoader( SpriteManager& spriteManager )
    : mSpriteManager( spriteManager )
{

}

void SpriteLoader::loadSpritesFromXml( const std::string& filename )
{
    using namespace rapidxml;

    // Load the XML file into memory
    bool didWork        = false;
    std::string xmldata = Utils::loadFile( filename, &didWork );

    if (! didWork )
    {
        raiseError( "Failed to load the file '" + filename + "'" );
        return;
    }

    // Instantiate an XML parser to parse this loaded xml string data, and
    // then grab the topmost tag <spritepackage>
    xml_document<> doc;
    doc.parse<0>( xmldata );

    xml_node<> *pXmlNode = doc.first_node( "spritepackage" );

    if ( pXmlNode == NULL )
    {
        raiseError( "Failed to find a <spritepackage> element" );
        return;
    }

    // Iterate through every child element of the current <spritepackage>,
    // and process the commands appropriately
    xml_node<> *pCurrentNode = pXmlNode->first_node();

    while ( pCurrentNode != NULL && (! hasErrors() ) )
    {
        // What kind of node is this?
        std::string name = pCurrentNode->name();

        if ( name == "sheet" )
        {
            parseSheetTag( pCurrentNode );
        }
        else if ( name == "sprite" )
        {
            parseSpriteTag( pCurrentNode );
        }
        else if ( name == "image" )
        {
            parseImageTag( pCurrentNode );
        }
        else
        {
            raiseWarning( "Unknown sprite tag " + name );
        }

        // Move to the next xml node in <spritepackage>
        pCurrentNode = pXmlNode->next_sibling();
    }
}

/**
 * Parses a <sheet ...> XML tag
 */
void SpriteLoader::parseSheetTag( const XmlNode* pXmlNode )
{
    assert( pXmlNode != NULL );

    // Attributes that are required
    std::string filename;
    std::string sheetname;
    int spriteWidth = -1;
    int spriteHeight = -1;

    // Walk through all of the attributes that this <sheet ...> XML tag
    // contains
    XmlAttribute *pAttribute = pXmlNode->first_attribute();

    while ( pAttribute != NULL )
    {
        std::string key   = pAttribute->name();
        std::string value = pAttribute->value();

        if ( key == "file" )
        {
            filename = value;
        }
        else if ( key == "name" )
        {
            sheetname = value;
        }
        else if ( key == "sw" )
        {
            if (! Utils::parseInt( value, &spriteWidth ) )
            {
                raiseError( "Invalid spritesheet width: " + value );
            }
        }
        else if ( name == "sh" )
        {
            if (! Utils::parseInt( value, &spriteHeight ) )
            {
                raiseError( "Invalid spritesheet height: " + value );
            }
        }
        else
        {
            // warning...
        }
    }

    // Validate that all attributes were loaded and are semi-correct
    if ( filename.empty() || sheetname.empty() || spriteWidth < 1 ||
         spriteHeight < 1 )
    {
        raiseError( "Missing or invalid content for spritesheet" );
    }

    // Load the sprite sheet
}

void SpriteLoader::addSpriteSheet( const std::string& name,
                                   const std::string& imagepath,
                                   unsigned int spriteWidth,
                                   unsigned int spriteHeight )
{
}

bool SpriteLoader::addSpriteTile( const std::string& name,
                                  const std::string& sheetName,
                                  unsigned int row,
                                  unsigned int col )
{

}

bool SpriteLoader::addSprite( const std::string& name,
                              const std::string& filename,
                              unsigned int spriteWidth,
                              unsigned int spriteHeight )
{

}
