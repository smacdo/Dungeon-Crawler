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
#ifndef DUNGEON_GRAPHICS_SPRITELOADER_H
#define DUNGEON_GRAPHICS_SPRITELOADER_H

#include <boost/utility.hpp>

class SpriteManager;
class TiXmlElement;

/**
 * The sprite loader class is responsible for taking an XML spritesheet
 * description and then loading all of the sprites it contains into the
 * sprite manager
 *
 * TODO: Add support for unloading a loaded sprite file
 */
class SpriteLoader : boost::noncopyable
{
public:
    SpriteLoader( SpriteManager& manager );
    ~SpriteLoader();

    void loadSpritesFromXml( const std::string& filepath );

    bool hasErrors() const;
    std::string errorText() const;

protected:
    void readSpriteSheetNode( const TiXmlElement * );
    void readSpriteNode( const std::string& spriteSheetFile,
                         const TiXmlElement * );
    
    void raiseError( const TiXmlElement * pNode,
                     const std::string& message );

private:
    SpriteManager& mSpriteManager;
    std::string mErrorText;

    int mExactWidth;
    int mExactHeight;
};

#endif
