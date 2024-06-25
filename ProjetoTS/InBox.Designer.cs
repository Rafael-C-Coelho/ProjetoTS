namespace ProjetoTS
{
    partial class InBox
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InBox));
			this.btnNewChat = new System.Windows.Forms.Button();
			this.listBoxMessages = new System.Windows.Forms.ListBox();
			this.txtBoxMessages = new System.Windows.Forms.RichTextBox();
			this.lblMessage = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.refreshMessagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.destruirUtilizadorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.logoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.timerFetchMessages = new System.Windows.Forms.Timer(this.components);
			this.lblUsername = new System.Windows.Forms.Label();
			this.txtBoxUser = new System.Windows.Forms.TextBox();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnNewChat
			// 
			this.btnNewChat.BackColor = System.Drawing.Color.SteelBlue;
			this.btnNewChat.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Bold);
			this.btnNewChat.ForeColor = System.Drawing.SystemColors.Window;
			this.btnNewChat.Location = new System.Drawing.Point(16, 62);
			this.btnNewChat.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.btnNewChat.Name = "btnNewChat";
			this.btnNewChat.Size = new System.Drawing.Size(324, 28);
			this.btnNewChat.TabIndex = 0;
			this.btnNewChat.Text = "NEW MESSAGE";
			this.btnNewChat.UseVisualStyleBackColor = false;
			this.btnNewChat.Click += new System.EventHandler(this.btnNewChat_Click);
			// 
			// listBoxMessages
			// 
			this.listBoxMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.listBoxMessages.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
			this.listBoxMessages.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.listBoxMessages.FormattingEnabled = true;
			this.listBoxMessages.Location = new System.Drawing.Point(9, 119);
			this.listBoxMessages.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.listBoxMessages.Name = "listBoxMessages";
			this.listBoxMessages.Size = new System.Drawing.Size(164, 182);
			this.listBoxMessages.TabIndex = 1;
			this.listBoxMessages.SelectedIndexChanged += new System.EventHandler(this.listBoxMessages_SelectedIndexChanged);
			// 
			// txtBoxMessages
			// 
			this.txtBoxMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.txtBoxMessages.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
			this.txtBoxMessages.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtBoxMessages.ForeColor = System.Drawing.SystemColors.MenuText;
			this.txtBoxMessages.Location = new System.Drawing.Point(195, 119);
			this.txtBoxMessages.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.txtBoxMessages.Name = "txtBoxMessages";
			this.txtBoxMessages.ReadOnly = true;
			this.txtBoxMessages.Size = new System.Drawing.Size(154, 182);
			this.txtBoxMessages.TabIndex = 2;
			this.txtBoxMessages.Text = "";
			// 
			// lblMessage
			// 
			this.lblMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.lblMessage.AutoSize = true;
			this.lblMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblMessage.ForeColor = System.Drawing.Color.SteelBlue;
			this.lblMessage.Location = new System.Drawing.Point(192, 101);
			this.lblMessage.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.lblMessage.Name = "lblMessage";
			this.lblMessage.Size = new System.Drawing.Size(72, 17);
			this.lblMessage.TabIndex = 3;
			this.lblMessage.Text = "Message";
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.ForeColor = System.Drawing.Color.SteelBlue;
			this.label1.Location = new System.Drawing.Point(6, 101);
			this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(103, 17);
			this.label1.TabIndex = 4;
			this.label1.Text = "Message List";
			// 
			// menuStrip1
			// 
			this.menuStrip1.BackColor = System.Drawing.Color.SteelBlue;
			this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
			this.menuStrip1.Size = new System.Drawing.Size(358, 25);
			this.menuStrip1.TabIndex = 5;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// settingsToolStripMenuItem
			// 
			this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshMessagesToolStripMenuItem,
            this.destruirUtilizadorToolStripMenuItem,
            this.logoutToolStripMenuItem});
			this.settingsToolStripMenuItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.settingsToolStripMenuItem.ForeColor = System.Drawing.SystemColors.Window;
			this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
			this.settingsToolStripMenuItem.Size = new System.Drawing.Size(89, 21);
			this.settingsToolStripMenuItem.Text = "OPTIONS";
			// 
			// refreshMessagesToolStripMenuItem
			// 
			this.refreshMessagesToolStripMenuItem.Name = "refreshMessagesToolStripMenuItem";
			this.refreshMessagesToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
			this.refreshMessagesToolStripMenuItem.Text = "Refresh messages";
			this.refreshMessagesToolStripMenuItem.Click += new System.EventHandler(this.refreshMessagesToolStripMenuItem_Click);
			// 
			// destruirUtilizadorToolStripMenuItem
			// 
			this.destruirUtilizadorToolStripMenuItem.Name = "destruirUtilizadorToolStripMenuItem";
			this.destruirUtilizadorToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
			this.destruirUtilizadorToolStripMenuItem.Text = "Destroy user";
			this.destruirUtilizadorToolStripMenuItem.Click += new System.EventHandler(this.destruirUtilizadorToolStripMenuItem_Click);
			// 
			// logoutToolStripMenuItem
			// 
			this.logoutToolStripMenuItem.Name = "logoutToolStripMenuItem";
			this.logoutToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
			this.logoutToolStripMenuItem.Text = "Logout";
			this.logoutToolStripMenuItem.Click += new System.EventHandler(this.logoutToolStripMenuItem_Click);
			// 
			// timerFetchMessages
			// 
			this.timerFetchMessages.Enabled = true;
			this.timerFetchMessages.Interval = 5000;
			this.timerFetchMessages.Tick += new System.EventHandler(this.timerFetchMessages_Tick);
			// 
			// lblUsername
			// 
			this.lblUsername.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblUsername.ForeColor = System.Drawing.Color.SteelBlue;
			this.lblUsername.Location = new System.Drawing.Point(14, 32);
			this.lblUsername.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.lblUsername.Name = "lblUsername";
			this.lblUsername.Size = new System.Drawing.Size(83, 19);
			this.lblUsername.TabIndex = 6;
			this.lblUsername.Text = "Username:";
			// 
			// txtBoxUser
			// 
			this.txtBoxUser.Location = new System.Drawing.Point(98, 32);
			this.txtBoxUser.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.txtBoxUser.Name = "txtBoxUser";
			this.txtBoxUser.ReadOnly = true;
			this.txtBoxUser.Size = new System.Drawing.Size(158, 20);
			this.txtBoxUser.TabIndex = 7;
			// 
			// InBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.ClientSize = new System.Drawing.Size(358, 310);
			this.Controls.Add(this.txtBoxUser);
			this.Controls.Add(this.lblUsername);
			this.Controls.Add(this.menuStrip1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.lblMessage);
			this.Controls.Add(this.txtBoxMessages);
			this.Controls.Add(this.listBoxMessages);
			this.Controls.Add(this.btnNewChat);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip1;
			this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.Name = "InBox";
			this.Text = "InBox";
			this.Load += new System.EventHandler(this.InBox_Load);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnNewChat;
        private System.Windows.Forms.ListBox listBoxMessages;
        private System.Windows.Forms.RichTextBox txtBoxMessages;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem destruirUtilizadorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logoutToolStripMenuItem;
        private System.Windows.Forms.Timer timerFetchMessages;
        private System.Windows.Forms.ToolStripMenuItem refreshMessagesToolStripMenuItem;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox txtBoxUser;
    }
}