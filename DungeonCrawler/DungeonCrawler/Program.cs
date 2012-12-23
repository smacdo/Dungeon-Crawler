using System;

namespace Scott.Dungeon.Game
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using ( DungeonCrawler game = new DungeonCrawler() )
            {
                game.Run();
            }
        }
    }
#endif
}

