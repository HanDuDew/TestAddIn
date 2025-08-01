namespace AutoBeau.Forms
{
    partial class ChooseInventorMethodsForm
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
            this.controlBox = new System.Windows.Forms.GroupBox();
            this.retrieveDimsCheckBox = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.autoArrangeCheckBox = new System.Windows.Forms.CheckBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.instructionLabel = new System.Windows.Forms.Label();
            this.controlBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // controlBox
            // 
            this.controlBox.Controls.Add(this.retrieveDimsCheckBox);
            this.controlBox.Controls.Add(this.checkBox3);
            this.controlBox.Controls.Add(this.checkBox2);
            this.controlBox.Controls.Add(this.autoArrangeCheckBox);
            this.controlBox.Location = new System.Drawing.Point(30, 41);
            this.controlBox.Name = "controlBox";
            this.controlBox.Size = new System.Drawing.Size(380, 279);
            this.controlBox.TabIndex = 1;
            this.controlBox.TabStop = false;
            this.controlBox.Text = "Select Inventor Methods";
            // 
            // retrieveDimsCheckBox
            // 
            this.retrieveDimsCheckBox.AutoSize = true;
            this.retrieveDimsCheckBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.retrieveDimsCheckBox.Location = new System.Drawing.Point(15, 192);
            this.retrieveDimsCheckBox.Name = "retrieveDimsCheckBox";
            this.retrieveDimsCheckBox.Size = new System.Drawing.Size(270, 28);
            this.retrieveDimsCheckBox.TabIndex = 3;
            this.retrieveDimsCheckBox.Text = "Retrieve Dimensions";
            this.retrieveDimsCheckBox.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.checkBox3.Location = new System.Drawing.Point(15, 144);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(222, 28);
            this.checkBox3.TabIndex = 2;
            this.checkBox3.Text = "Add Centermarks";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.checkBox2.Location = new System.Drawing.Point(15, 96);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(210, 28);
            this.checkBox2.TabIndex = 1;
            this.checkBox2.Text = "Add Hole Table";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // autoArrangeCheckBox
            // 
            this.autoArrangeCheckBox.AutoSize = true;
            this.autoArrangeCheckBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.autoArrangeCheckBox.Location = new System.Drawing.Point(15, 46);
            this.autoArrangeCheckBox.Name = "autoArrangeCheckBox";
            this.autoArrangeCheckBox.Size = new System.Drawing.Size(186, 28);
            this.autoArrangeCheckBox.TabIndex = 0;
            this.autoArrangeCheckBox.Text = "Auto Arrange";
            this.autoArrangeCheckBox.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.okButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.okButton.ForeColor = System.Drawing.Color.White;
            this.okButton.Location = new System.Drawing.Point(210, 350);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(100, 35);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = false;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.BackColor = System.Drawing.Color.Gray;
            this.cancelButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelButton.ForeColor = System.Drawing.Color.White;
            this.cancelButton.Location = new System.Drawing.Point(320, 350);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(100, 35);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = false;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // instructionLabel
            // 
            this.instructionLabel.AutoSize = true;
            this.instructionLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.instructionLabel.Location = new System.Drawing.Point(30, 30);
            this.instructionLabel.Name = "instructionLabel";
            this.instructionLabel.Size = new System.Drawing.Size(657, 32);
            this.instructionLabel.TabIndex = 0;
            this.instructionLabel.Text = "Choose which Inventor methods to enable in Manual Mode:";
            this.instructionLabel.Visible = false;
            // 
            // ChooseInventorMethodsForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(450, 420);
            this.Controls.Add(this.instructionLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.controlBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChooseInventorMethodsForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Choose Inventor Methods";
            this.Load += new System.EventHandler(this.ChooseInventorMethodsForm_Load);
            this.controlBox.ResumeLayout(false);
            this.controlBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox controlBox;
        private System.Windows.Forms.CheckBox retrieveDimsCheckBox;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox autoArrangeCheckBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label instructionLabel;
    }
}