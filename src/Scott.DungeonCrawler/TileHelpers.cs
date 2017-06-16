/*
 * Copyright 2012-2014 Scott MacDonald
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
using Scott.Forge.Tilemaps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scott.DungeonCrawler
{
    [Flags]
    public enum TileDataFlags : ushort
    {
        Placed = 0x1,           // Tile is considered meaningful (not void space).
        Room = 0x2,             // Tile is considered part of a room.
        // Note: You can do AB = A | B !!
    }

    public static class TileHelper
    {
        /*public static bool IsPlaced(Tile tile)
        {
            return (tile.Data & (ushort)TileDataFlags.Placed) != 0;
        }

        public static bool IsInRoom(Tile tile)
        {
            return (tile.Data & (ushort)TileDataFlags.Room) != 0;
        }

        public static void SetIsPlaced(Tile tile, bool isPlaced)
        {
            if (isPlaced)
            {
                tile.Data |= (ushort)TileDataFlags.Placed;
            }
            else
            {
                tile.Data &= (ushort)~TileDataFlags.Placed;
            }
        }

        public static void SetIsInRoom(Tile tile, bool isRoom)
        {
            if (isRoom)
            {
                tile.Data |= (ushort)TileDataFlags.Room;
            }
            else
            {
                tile.Data &= (ushort)~TileDataFlags.Room;
            }
        }*/
    }

    [Flags]
    public enum TileDefinitionFlags : ulong
    {
        Void = 0x1,             // Tile is nothingness.
        Wall = 0x2,             // Tile is a room wall.
        Floor = 0x4,            // Tile is a room floor.
    }

    public static class TileDefinitionHelpers
    {
        public static bool IsVoid(this TileDefinition tile)
        {
            return (tile.ExtraFlags & (ulong)TileDefinitionFlags.Void) != 0;
        }

        public static bool IsWall(this TileDefinition tile)
        {
            return (tile.ExtraFlags & (ulong)TileDefinitionFlags.Wall) != 0;
        }

        public static bool IsFloor(this TileDefinition tile)
        {
            return (tile.ExtraFlags & (ulong)TileDefinitionFlags.Floor) != 0;
        }

        public static void SetIsVoid(this TileDefinition tile, bool isVoid)
        {
            if (isVoid)
            {
                tile.ExtraFlags |= (ulong)TileDefinitionFlags.Void;
            }
            else
            {
                tile.ExtraFlags &= (ulong)~TileDefinitionFlags.Void;
            }
        }

        public static void SetIsWall(this TileDefinition tile, bool isWall)
        {
            if (isWall)
            {
                tile.ExtraFlags |= (ulong)TileDefinitionFlags.Wall;
            }
            else
            {
                tile.ExtraFlags &= (ulong)~TileDefinitionFlags.Wall;
            }
        }

        public static void SetIsFloor(this TileDefinition tile, bool isFloor)
        {
            if (isFloor)
            {
                tile.ExtraFlags |= (ulong)TileDefinitionFlags.Floor;
            }
            else
            {
                tile.ExtraFlags &= (ulong)~TileDefinitionFlags.Floor;
            }
        }
    }
}
