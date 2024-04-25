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
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnNewChat
            // 
            this.btnNewChat.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNewChat.Location = new System.Drawing.Point(17, 24);
            this.btnNewChat.Margin = new System.Windows.Forms.Padding(2);
            this.btnNewChat.Name = "btnNewChat";
            this.btnNewChat.Size = new System.Drawing.Size(324, 28);
            this.btnNewChat.TabIndex = 0;
            this.btnNewChat.Text = "Nova mensagem";
            this.btnNewChat.UseVisualStyleBackColor = true;
            this.btnNewChat.Click += new System.EventHandler(this.btnNewChat_Click);
            // 
            // listBoxMessages
            // 
            this.listBoxMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.listBoxMessages.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listBoxMessages.FormattingEnabled = true;
            this.listBoxMessages.Location = new System.Drawing.Point(17, 72);
            this.listBoxMessages.Margin = new System.Windows.Forms.Padding(2);
            this.listBoxMessages.Name = "listBoxMessages";
            this.listBoxMessages.Size = new System.Drawing.Size(154, 182);
            this.listBoxMessages.TabIndex = 1;
            this.listBoxMessages.SelectedIndexChanged += new System.EventHandler(this.listBoxMessages_SelectedIndexChanged);
            // 
            // txtBoxMessages
            // 
            this.txtBoxMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBoxMessages.BackColor = System.Drawing.SystemColors.ControlDark;
            this.txtBoxMessages.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtBoxMessages.ForeColor = System.Drawing.SystemColors.MenuText;
            this.txtBoxMessages.Location = new System.Drawing.Point(187, 72);
            this.txtBoxMessages.Margin = new System.Windows.Forms.Padding(2);
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
            this.lblMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessage.Location = new System.Drawing.Point(184, 54);
            this.lblMessage.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(77, 17);
            this.lblMessage.TabIndex = 3;
            this.lblMessage.Text = "Mensagem";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(14, 54);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "Lista de mensagens";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(358, 24);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshMessagesToolStripMenuItem,
            this.destruirUtilizadorToolStripMenuItem,
            this.logoutToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Options";
            // 
            // refreshMessagesToolStripMenuItem
            // 
            this.refreshMessagesToolStripMenuItem.Name = "refreshMessagesToolStripMenuItem";
            this.refreshMessagesToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.refreshMessagesToolStripMenuItem.Text = "Refresh messages";
            this.refreshMessagesToolStripMenuItem.Click += new System.EventHandler(this.refreshMessagesToolStripMenuItem_Click);
            // 
            // destruirUtilizadorToolStripMenuItem
            // 
            this.destruirUtilizadorToolStripMenuItem.Name = "destruirUtilizadorToolStripMenuItem";
            this.destruirUtilizadorToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.destruirUtilizadorToolStripMenuItem.Text = "Destroy user";
            this.destruirUtilizadorToolStripMenuItem.Click += new System.EventHandler(this.destruirUtilizadorToolStripMenuItem_Click);
            // 
            // logoutToolStripMenuItem
            // 
            this.logoutToolStripMenuItem.Name = "logoutToolStripMenuItem";
            this.logoutToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.logoutToolStripMenuItem.Text = "Logout";
            // 
            // timerFetchMessages
            // 
            this.timerFetchMessages.Enabled = true;
            this.timerFetchMessages.Interval = 5000;
            this.timerFetchMessages.Tick += new System.EventHandler(this.timerFetchMessages_Tick);
            // 
            // InBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(358, 280);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.txtBoxMessages);
            this.Controls.Add(this.listBoxMessages);
            this.Controls.Add(this.btnNewChat);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "InBox";
            this.Text = "InBox";
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
    }
}