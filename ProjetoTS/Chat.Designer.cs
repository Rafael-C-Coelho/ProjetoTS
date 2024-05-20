namespace ProjetoTS
{
    partial class Chat
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Chat));
            this.lblTo = new System.Windows.Forms.Label();
            this.cBxTo = new System.Windows.Forms.ComboBox();
            this.textBox = new System.Windows.Forms.TextBox();
            this.lblMessage = new System.Windows.Forms.Label();
            this.btnSend = new System.Windows.Forms.Button();
            this.lblMaxChars = new System.Windows.Forms.Label();
            this.lblUsername = new System.Windows.Forms.Label();
            this.txtBoxUser = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblTo
            // 
            this.lblTo.AutoSize = true;
            this.lblTo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTo.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblTo.Location = new System.Drawing.Point(8, 48);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(36, 20);
            this.lblTo.TabIndex = 0;
            this.lblTo.Text = "To:";
            // 
            // cBxTo
            // 
            this.cBxTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cBxTo.FormattingEnabled = true;
            this.cBxTo.Location = new System.Drawing.Point(47, 48);
            this.cBxTo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cBxTo.Name = "cBxTo";
            this.cBxTo.Size = new System.Drawing.Size(336, 24);
            this.cBxTo.TabIndex = 1;
            // 
            // textBox
            // 
            this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox.Location = new System.Drawing.Point(11, 111);
            this.textBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox.MaxLength = 100;
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(372, 187);
            this.textBox.TabIndex = 2;
            // 
            // lblMessage
            // 
            this.lblMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblMessage.AutoSize = true;
            this.lblMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessage.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblMessage.Location = new System.Drawing.Point(8, 88);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(84, 20);
            this.lblMessage.TabIndex = 3;
            this.lblMessage.Text = "Message";
            // 
            // btnSend
            // 
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.BackColor = System.Drawing.Color.SteelBlue;
            this.btnSend.FlatAppearance.BorderColor = System.Drawing.Color.SteelBlue;
            this.btnSend.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F);
            this.btnSend.ForeColor = System.Drawing.SystemColors.Window;
            this.btnSend.Location = new System.Drawing.Point(12, 306);
            this.btnSend.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(371, 37);
            this.btnSend.TabIndex = 4;
            this.btnSend.Text = "SEND";
            this.btnSend.UseVisualStyleBackColor = false;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // lblMaxChars
            // 
            this.lblMaxChars.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblMaxChars.AutoSize = true;
            this.lblMaxChars.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMaxChars.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblMaxChars.Location = new System.Drawing.Point(263, 94);
            this.lblMaxChars.Name = "lblMaxChars";
            this.lblMaxChars.Size = new System.Drawing.Size(121, 13);
            this.lblMaxChars.TabIndex = 5;
            this.lblMaxChars.Text = "Max. 100 characters";
            // 
            // lblUsername
            // 
            this.lblUsername.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUsername.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblUsername.Location = new System.Drawing.Point(8, 9);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(111, 23);
            this.lblUsername.TabIndex = 7;
            this.lblUsername.Text = "Username:";
            // 
            // txtBoxUser
            // 
            this.txtBoxUser.Location = new System.Drawing.Point(109, 10);
            this.txtBoxUser.Name = "txtBoxUser";
            this.txtBoxUser.ReadOnly = true;
            this.txtBoxUser.Size = new System.Drawing.Size(115, 22);
            this.txtBoxUser.TabIndex = 8;
            // 
            // Chat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(395, 353);
            this.Controls.Add(this.txtBoxUser);
            this.Controls.Add(this.lblUsername);
            this.Controls.Add(this.lblMaxChars);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.cBxTo);
            this.Controls.Add(this.lblTo);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Chat";
            this.Text = "Chat";
            this.Load += new System.EventHandler(this.Chat_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.ComboBox cBxTo;
        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Label lblMaxChars;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox txtBoxUser;
    }
}