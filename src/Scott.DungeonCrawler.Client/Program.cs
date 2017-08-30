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

using System;
using System.Threading;
using Scott.Dungeon;
using DungeonCrawler;
using Forge.Content;
using System.Collections.Generic;
using DungeonCrawler.WinDesktopClient.Content;

namespace DungeonCrawler.WinDesktopClient
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // Run the game.
            if ( System.Diagnostics.Debugger.IsAttached )
            {
                RunGame();
            }
            else
            {
                RunWithExceptionGuard();
            }
        }

        /// <summary>
        ///  Runs the game with a exception guard that catches uncaught exceptions and reports
        ///  them to the user.
        /// </summary>
        private static void RunWithExceptionGuard()
        {
            // Install a global exception handler to catch any pesky exceptions.
            //MediaTypeNames.Application.ThreadException += ApplicationThreadException;

            // Run the game in an exception guard.
            try
            {
                RunGame();
            }
            catch ( System.Exception ex )
            {
                if ( System.Diagnostics.Debugger.IsAttached )
                {
                    throw;
                }
                else
                {
                    ReportUncaughtException( ex );
                }
            }
        }

        /// <summary>
        ///  Runs the game.
        /// </summary>
        private static void RunGame()
        {
            // Configure content manager.
            var rootContentContainer = new DirectoryContentContainer(Settings.Default.ContentDir);
            var contentHandlerDirectory = new ReflectionContentHandlerDirectory();
            
            var contentManager = new ForgeContentManager(
                new List<IContentContainer>() { rootContentContainer },
                new List<IContentHandlerDirectory> { contentHandlerDirectory });

            // Create and run game.
            using (var game = new DungeonCrawlerClient(contentManager))
            {
                game.Run();
            }
        }

        /// <summary>
        ///  Traps an uncaught exception, and presents it to the player.
        /// </summary>
        /// <param name="ex"></param>
        private static void ReportUncaughtException( System.Exception ex )
        {
            var dialog = new ErrorDialog( ex );
            dialog.ShowDialog();
        }

        /// <summary>
        ///  Handles exceptions we forgot to catch (If for some reason it falls through our general
        ///  exception catcher).
        /// </summary>
        internal static void ApplicationThreadException( object sender, ThreadExceptionEventArgs e )
        {
            ReportUncaughtException( e.Exception );
        }
    }
#endif
}

