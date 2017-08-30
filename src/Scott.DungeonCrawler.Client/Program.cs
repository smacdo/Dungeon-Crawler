/*
 * Copyright 2012-2017 Scott MacDonald
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
using Forge.Content;
using System.Collections.Generic;
using DungeonCrawler.WinDesktopClient.Content;
using System.Windows.Forms;

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
            // Install a global exception handler to catch pesky exceptions (but only if a debugger was not attached).
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                Application.ThreadException += ApplicationThreadException;
            }

            // Run the game and report any uncaught exceptions.
            try
            {
                RunGame();
            }
            catch (Exception ex) when (!System.Diagnostics.Debugger.IsAttached)
            {
                ReportUncaughtException(ex);
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
        private static void ReportUncaughtException(Exception ex)
        {
            var dialog = new ErrorDialog(ex);
            dialog.ShowDialog();
        }

        /// <summary>
        ///  Handles exceptions we forgot to catch.
        /// </summary>
        private static void ApplicationThreadException(object sender, ThreadExceptionEventArgs e)
        {
            ReportUncaughtException(e.Exception);
        }
    }
#endif
}

