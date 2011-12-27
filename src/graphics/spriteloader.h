#ifndef DUNGEON_GRAPHICS_SPRITELOADER_H
#define DUNGEON_GRAPHICS_SPRITELOADER_H

// Forward declarations
template<class Ch> class xml_node;
template<class Ch> class xml_attribute;

typedef xml_node<char> XmlNode;
typedef xml_attribute<Char> XmlAttribute;

class SpriteLoader
{
public:
    SpriteLoader( SpriteManager& manager );
    ~SpriteLoader();

    void loadSpritesFromXml( const std::string& filepath );

    bool hasErrors() const;
    std::string errorText() const;

protected:
    void parseSheetTag( const XmlNode* pXmlNode );
    void parseSpriteTag( const XmlNode* pXmlNode );
    void parseImageTag( const XmlNode* pXmlNode );
    
    void raiseError( const std::string& message );
    void raiseWarning( const std::string& message );

private:
    SpriteManager& mSpriteManager;
    std::string mErrorText;
};

#endif
