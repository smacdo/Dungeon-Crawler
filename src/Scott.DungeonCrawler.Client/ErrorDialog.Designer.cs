namespace Scott.Dungeon
{
    partial class ErrorDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorDialog));
            this.mTopPanel = new System.Windows.Forms.Panel();
            this.mAppPictureBox = new System.Windows.Forms.PictureBox();
            this.mSorryMessage = new System.Windows.Forms.Label();
            this.mQuitButton = new System.Windows.Forms.Button();
            this.mImageList32 = new System.Windows.Forms.ImageList(this.components);
            this.mErrorText = new System.Windows.Forms.Label();
            this.mBottomPanel = new System.Windows.Forms.Panel();
            this.mSendErrorReport = new System.Windows.Forms.Button();
            this.mViewErrorDetails = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.mPleaseTell = new System.Windows.Forms.Label();
            this.mHelpUsImprove = new System.Windows.Forms.Label();
            this.mErrorTitle = new System.Windows.Forms.Label();
            this.mTopPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mAppPictureBox)).BeginInit();
            this.mBottomPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mTopPanel
            // 
            this.mTopPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mTopPanel.BackColor = System.Drawing.Color.Cornsilk;
            this.mTopPanel.Controls.Add(this.mAppPictureBox);
            this.mTopPanel.Controls.Add(this.mSorryMessage);
            this.mTopPanel.Location = new System.Drawing.Point(0, 0);
            this.mTopPanel.Name = "mTopPanel";
            this.mTopPanel.Size = new System.Drawing.Size(553, 44);
            this.mTopPanel.TabIndex = 0;
            // 
            // mAppPictureBox
            // 
            this.mAppPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mAppPictureBox.Location = new System.Drawing.Point(518, 3);
            this.mAppPictureBox.Name = "mAppPictureBox";
            this.mAppPictureBox.Size = new System.Drawing.Size(32, 32);
            this.mAppPictureBox.TabIndex = 1;
            this.mAppPictureBox.TabStop = false;
            // 
            // mSorryMessage
            // 
            this.mSorryMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mSorryMessage.Location = new System.Drawing.Point(12, 12);
            this.mSorryMessage.Name = "mSorryMessage";
            this.mSorryMessage.Size = new System.Drawing.Size(500, 28);
            this.mSorryMessage.TabIndex = 0;
            this.mSorryMessage.Text = "$APPNAME has encountered a problem and needs to close. We are sorry for the incon" +
    "vienience.";
            // 
            // mQuitButton
            // 
            this.mQuitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mQuitButton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.mQuitButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mQuitButton.Location = new System.Drawing.Point(467, 11);
            this.mQuitButton.Name = "mQuitButton";
            this.mQuitButton.Size = new System.Drawing.Size(75, 23);
            this.mQuitButton.TabIndex = 3;
            this.mQuitButton.Text = "Quit";
            this.mQuitButton.UseVisualStyleBackColor = false;
            // 
            // mImageList32
            // 
            this.mImageList32.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("mImageList32.ImageStream")));
            this.mImageList32.TransparentColor = System.Drawing.Color.Transparent;
            this.mImageList32.Images.SetKeyName(0, "GameThumbnail.png");
            // 
            // mErrorText
            // 
            this.mErrorText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mErrorText.AutoEllipsis = true;
            this.mErrorText.Location = new System.Drawing.Point(12, 80);
            this.mErrorText.Name = "mErrorText";
            this.mErrorText.Size = new System.Drawing.Size(538, 59);
            this.mErrorText.TabIndex = 2;
            this.mErrorText.Text = "(ERROR DETAILS)";
            // 
            // mBottomPanel
            // 
            this.mBottomPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mBottomPanel.BackColor = System.Drawing.Color.LightGray;
            this.mBottomPanel.Controls.Add(this.mSendErrorReport);
            this.mBottomPanel.Controls.Add(this.mViewErrorDetails);
            this.mBottomPanel.Controls.Add(this.label1);
            this.mBottomPanel.Controls.Add(this.mQuitButton);
            this.mBottomPanel.Location = new System.Drawing.Point(0, 207);
            this.mBottomPanel.Name = "mBottomPanel";
            this.mBottomPanel.Size = new System.Drawing.Size(553, 44);
            this.mBottomPanel.TabIndex = 3;
            // 
            // mSendErrorReport
            // 
            this.mSendErrorReport.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.mSendErrorReport.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.mSendErrorReport.Location = new System.Drawing.Point(340, 11);
            this.mSendErrorReport.Name = "mSendErrorReport";
            this.mSendErrorReport.Size = new System.Drawing.Size(121, 23);
            this.mSendErrorReport.TabIndex = 2;
            this.mSendErrorReport.Text = "Send Error Report";
            this.mSendErrorReport.UseVisualStyleBackColor = false;
            this.mSendErrorReport.Click += new System.EventHandler(this.ViewErrorDetails);
            // 
            // mViewErrorDetails
            // 
            this.mViewErrorDetails.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.mViewErrorDetails.Location = new System.Drawing.Point(215, 11);
            this.mViewErrorDetails.Name = "mViewErrorDetails";
            this.mViewErrorDetails.Size = new System.Drawing.Size(119, 23);
            this.mViewErrorDetails.TabIndex = 1;
            this.mViewErrorDetails.Text = "View Error Details";
            this.mViewErrorDetails.UseVisualStyleBackColor = false;
            this.mViewErrorDetails.Click += new System.EventHandler(this.ViewErrorDetails);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Forge Engine";
            this.label1.Visible = false;
            // 
            // mPleaseTell
            // 
            this.mPleaseTell.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mPleaseTell.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mPleaseTell.Location = new System.Drawing.Point(12, 139);
            this.mPleaseTell.Name = "mPleaseTell";
            this.mPleaseTell.Size = new System.Drawing.Size(541, 13);
            this.mPleaseTell.TabIndex = 4;
            this.mPleaseTell.Text = "Please tell $VENDOR about this problem";
            // 
            // mHelpUsImprove
            // 
            this.mHelpUsImprove.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mHelpUsImprove.Location = new System.Drawing.Point(12, 161);
            this.mHelpUsImprove.Name = "mHelpUsImprove";
            this.mHelpUsImprove.Size = new System.Drawing.Size(538, 34);
            this.mHelpUsImprove.TabIndex = 5;
            this.mHelpUsImprove.Text = "To help improve the software you use, $VENDOR is interested in learning more abou" +
    "t this error. We have created an error report for you to send to us.";
            // 
            // mErrorTitle
            // 
            this.mErrorTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mErrorTitle.AutoSize = true;
            this.mErrorTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mErrorTitle.Location = new System.Drawing.Point(12, 60);
            this.mErrorTitle.Name = "mErrorTitle";
            this.mErrorTitle.Size = new System.Drawing.Size(98, 13);
            this.mErrorTitle.TabIndex = 6;
            this.mErrorTitle.Text = "(ERROR TITLE)";
            // 
            // ErrorDialog
            // 
            this.AcceptButton = this.mViewErrorDetails;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.mQuitButton;
            this.ClientSize = new System.Drawing.Size(553, 249);
            this.Controls.Add(this.mErrorTitle);
            this.Controls.Add(this.mHelpUsImprove);
            this.Controls.Add(this.mPleaseTell);
            this.Controls.Add(this.mBottomPanel);
            this.Controls.Add(this.mErrorText);
            this.Controls.Add(this.mTopPanel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ErrorDialog";
            this.Text = "Error Dialog";
            this.Load += new System.EventHandler(this.OnLoad);
            this.mTopPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mAppPictureBox)).EndInit();
            this.mBottomPanel.ResumeLayout(false);
            this.mBottomPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel mTopPanel;
        private System.Windows.Forms.Button mQuitButton;
        private System.Windows.Forms.Label mSorryMessage;
        private System.Windows.Forms.PictureBox mAppPictureBox;
        private System.Windows.Forms.ImageList mImageList32;
        private System.Windows.Forms.Label mErrorText;
        private System.Windows.Forms.Panel mBottomPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label mPleaseTell;
        private System.Windows.Forms.Label mHelpUsImprove;
        private System.Windows.Forms.Button mSendErrorReport;
        private System.Windows.Forms.Button mViewErrorDetails;
        private System.Windows.Forms.Label mErrorTitle;
    }
}