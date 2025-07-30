namespace InvAddIn.Forms
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.button1 = new System.Windows.Forms.Button();
            this.autoArrangeCheckBox = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.controlBox = new System.Windows.Forms.GroupBox();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configureAPIKeyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.switchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.modeToggleButton = new System.Windows.Forms.Button();
            this.chatWindow = new InvAddIn.Forms.ChatWindow();
            this.controlBox.SuspendLayout();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button1.Location = new System.Drawing.Point(61, 529);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(130, 58);
            this.button1.TabIndex = 0;
            this.button1.Text = "Apply";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // autoArrangeCheckBox
            // 
            this.autoArrangeCheckBox.AutoSize = true;
            this.autoArrangeCheckBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.autoArrangeCheckBox.Location = new System.Drawing.Point(18, 63);
            this.autoArrangeCheckBox.Name = "autoArrangeCheckBox";
            this.autoArrangeCheckBox.Size = new System.Drawing.Size(186, 28);
            this.autoArrangeCheckBox.TabIndex = 1;
            this.autoArrangeCheckBox.Text = "Auto Arrange";
            this.autoArrangeCheckBox.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.checkBox2.Location = new System.Drawing.Point(18, 110);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(210, 28);
            this.checkBox2.TabIndex = 2;
            this.checkBox2.Text = "Add Hole Table";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.checkBox3.Location = new System.Drawing.Point(18, 157);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(222, 28);
            this.checkBox3.TabIndex = 3;
            this.checkBox3.Text = "Add Centermarks";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
            // 
            // controlBox
            // 
            this.controlBox.Controls.Add(this.checkBox3);
            this.controlBox.Controls.Add(this.checkBox2);
            this.controlBox.Controls.Add(this.autoArrangeCheckBox);
            this.controlBox.Location = new System.Drawing.Point(61, 189);
            this.controlBox.Name = "controlBox";
            this.controlBox.Size = new System.Drawing.Size(834, 251);
            this.controlBox.TabIndex = 5;
            this.controlBox.TabStop = false;
            this.controlBox.Text = "Manual Controls";
            // 
            // menuStrip
            // 
            this.menuStrip.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.switchToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(970, 39);
            this.menuStrip.TabIndex = 7;
            this.menuStrip.Text = "menuStrip1";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configureAPIKeyToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(127, 35);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // configureAPIKeyToolStripMenuItem
            // 
            this.configureAPIKeyToolStripMenuItem.Name = "configureAPIKeyToolStripMenuItem";
            this.configureAPIKeyToolStripMenuItem.Size = new System.Drawing.Size(355, 44);
            this.configureAPIKeyToolStripMenuItem.Text = "Configure API Key";
            this.configureAPIKeyToolStripMenuItem.Click += new System.EventHandler(this.configureAPIKeyToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(105, 35);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // switchToolStripMenuItem
            // 
            this.switchToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.switchToolStripMenuItem.Margin = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.switchToolStripMenuItem.Name = "switchToolStripMenuItem";
            this.switchToolStripMenuItem.Size = new System.Drawing.Size(245, 35);
            this.switchToolStripMenuItem.Text = "Switch to AI Mode";
            this.switchToolStripMenuItem.Click += new System.EventHandler(this.switchToolStripMenuItem_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::InvAddIn.Properties.Resources.logo;
            this.pictureBox1.Location = new System.Drawing.Point(22, 75);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(389, 59);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 8;
            this.pictureBox1.TabStop = false;
            // 
            // modeToggleButton
            // 
            this.modeToggleButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.modeToggleButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.modeToggleButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.modeToggleButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.modeToggleButton.ForeColor = System.Drawing.Color.White;
            this.modeToggleButton.Location = new System.Drawing.Point(593, 407);
            this.modeToggleButton.Name = "modeToggleButton";
            this.modeToggleButton.Size = new System.Drawing.Size(192, 49);
            this.modeToggleButton.TabIndex = 9;
            this.modeToggleButton.Text = "Switch to AI Mode";
            this.modeToggleButton.UseVisualStyleBackColor = false;
            this.modeToggleButton.Visible = false;
            this.modeToggleButton.Click += new System.EventHandler(this.modeToggleButton_Click);
            // 
            // chatWindow
            // 
            this.chatWindow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.chatWindow.Location = new System.Drawing.Point(417, 84);
            this.chatWindow.Name = "chatWindow";
            this.chatWindow.Size = new System.Drawing.Size(541, 565);
            this.chatWindow.TabIndex = 6;
            this.chatWindow.Visible = false;
            this.chatWindow.Load += new System.EventHandler(this.chatWindow_Load);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(970, 648);
            this.Controls.Add(this.modeToggleButton);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.chatWindow);
            this.Controls.Add(this.controlBox);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.menuStrip);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.controlBox.ResumeLayout(false);
            this.controlBox.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox autoArrangeCheckBox;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.GroupBox controlBox;
        private InvAddIn.Forms.ChatWindow chatWindow;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configureAPIKeyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button modeToggleButton;
        private System.Windows.Forms.ToolStripMenuItem switchToolStripMenuItem;
    }
}