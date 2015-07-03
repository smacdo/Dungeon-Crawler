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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Scott.Forge;

namespace Scott.Dungeon
{
    public partial class ErrorDialog : Form
    {
        private System.Exception mException;

        /// <summary>
        ///  Constructor.
        /// </summary>
        public ErrorDialog( System.Exception exception )
        {
            mException = exception;

            // Set the platform font correctly before initializing components.
//            this.Font = SystemFonts.DialogFont;

            // Now initialize our components.
            InitializeComponent();
        }

        /// <summary>
        ///  Set up the form display before presenting it to the user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoad( object sender, EventArgs e )
        {
            // Get assembly information.
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo( assembly.Location );

            string product = fvi.ProductName;
            string vendor  = fvi.CompanyName;

            // Replace dialog placeholder text with actual text.
            ReplaceText( mSorryMessage, product, vendor );
            ReplaceText( mPleaseTell, product, vendor );
            ReplaceText( mHelpUsImprove, product, vendor );

            // Make our dialog have the product name in it.
            Text = "{0} Fatal Error".With( product );

            // Format the exception error message.
            if ( mException != null )
            {
                mErrorTitle.Text = mException.GetType().FullName;
                mErrorText.Text = mException.Message;
            }
            else
            {
                mErrorTitle.Text = "Error is missing";
                mErrorText.Text = "The exception that caused the error was null! What on earth?";
            }

            // Load the application icon.
            mAppPictureBox.Image = mImageList32.Images[0];

            // Make sure the user hears the standard dialog box alert.
            System.Media.SystemSounds.Exclamation.Play();
        }

        /// <summary>
        ///  Replace placeholders in a label with their actual values.
        /// </summary>
        /// <param name="label"></param>
        /// <param name="product"></param>
        /// <param name="vendor"></param>
        private void ReplaceText( Label label, string product, string vendor )
        {
            label.Text = label.Text.Replace( "$APPNAME", product );
            label.Text = label.Text.Replace( "$VENDOR", vendor );
        }

        /// <summary>
        ///  View the exception report.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewErrorDetails( object sender, EventArgs e )
        {
            RunExceptionReporter();
        }

        /// <summary>
        ///  Show the exception report.
        /// </summary>
        private void RunExceptionReporter()
        {
            ExceptionReporting.ExceptionReporter reporter =
                new ExceptionReporting.ExceptionReporter();

            reporter.ReadConfig();
            reporter.Show( mException );
        }
    }
}
