using System.Windows.Forms;
using System;
using System.Threading;

namespace Scott.Dungeon.Game
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
            Application.ThreadException += ApplicationThreadException;

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
            using ( DungeonCrawler game = new DungeonCrawler() )
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
            ErrorDialog dialog = new ErrorDialog( ex );
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

