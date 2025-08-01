using System;
using System.Drawing;
using System.Windows.Forms;
using AutoBeau.Services;
using AutoBeau.Common;

namespace AutoBeau.Forms
{
    public partial class ApiKeyConfigForm : Form
    {
        private TextBox apiKeyTextBox;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button clearButton;
        private Label instructionLabel;
        private Label statusLabel;
        private GroupBox configGroupBox;

        public ApiKeyConfigForm()
        {
            InitializeComponent();
            LoadCurrentStatus();
        }

        private void InitializeComponent()
        {
            this.apiKeyTextBox = new TextBox();
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.clearButton = new System.Windows.Forms.Button();
            this.instructionLabel = new Label();
            this.statusLabel = new Label();
            this.configGroupBox = new GroupBox();
            this.configGroupBox.SuspendLayout();
            this.SuspendLayout();

            // 
            // instructionLabel
            // 
            this.instructionLabel.AutoSize = true;
            this.instructionLabel.Location = new Point(20, 20);
            this.instructionLabel.Name = "instructionLabel";
            this.instructionLabel.Size = new Size(450, 48);
            this.instructionLabel.TabIndex = 0;
            this.instructionLabel.Text = "Enter your OpenAI API key to enable AI chat functionality.\r\nYou can get an API key from: https://platform.openai.com/api-keys";

            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            this.statusLabel.ForeColor = Color.Gray;
            this.statusLabel.Location = new Point(20, 85);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new Size(150, 15);
            this.statusLabel.TabIndex = 1;
            this.statusLabel.Text = "Current Status: Checking...";

            // 
            // configGroupBox
            // 
            this.configGroupBox.Controls.Add(this.clearButton);
            this.configGroupBox.Controls.Add(this.cancelButton);
            this.configGroupBox.Controls.Add(this.saveButton);
            this.configGroupBox.Controls.Add(this.apiKeyTextBox);
            this.configGroupBox.Location = new Point(20, 120);
            this.configGroupBox.Name = "configGroupBox";
            this.configGroupBox.Size = new Size(450, 120);
            this.configGroupBox.TabIndex = 2;
            this.configGroupBox.TabStop = false;
            this.configGroupBox.Text = "API Key Configuration";

            // 
            // apiKeyTextBox
            // 
            this.apiKeyTextBox.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.apiKeyTextBox.Location = new Point(15, 30);
            this.apiKeyTextBox.Name = "apiKeyTextBox";
            this.apiKeyTextBox.PasswordChar = '*';
            this.apiKeyTextBox.Size = new Size(420, 22);
            this.apiKeyTextBox.TabIndex = 0;

            // 
            // saveButton
            // 
            this.saveButton.BackColor = Color.FromArgb(0, 120, 215);
            this.saveButton.FlatStyle = FlatStyle.Flat;
            this.saveButton.ForeColor = Color.White;
            this.saveButton.Location = new Point(15, 70);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new Size(100, 35);
            this.saveButton.TabIndex = 1;
            this.saveButton.Text = "Save & Test";
            this.saveButton.UseVisualStyleBackColor = false;
            this.saveButton.Click += new EventHandler(this.SaveButton_Click);

            // 
            // clearButton
            // 
            this.clearButton.BackColor = Color.FromArgb(232, 17, 35);
            this.clearButton.FlatStyle = FlatStyle.Flat;
            this.clearButton.ForeColor = Color.White;
            this.clearButton.Location = new Point(125, 70);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new Size(100, 35);
            this.clearButton.TabIndex = 2;
            this.clearButton.Text = "Clear Saved";
            this.clearButton.UseVisualStyleBackColor = false;
            this.clearButton.Click += new EventHandler(this.ClearButton_Click);

            // 
            // cancelButton
            // 
            this.cancelButton.BackColor = Color.Gray;
            this.cancelButton.FlatStyle = FlatStyle.Flat;
            this.cancelButton.ForeColor = Color.White;
            this.cancelButton.Location = new Point(335, 70);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new Size(100, 35);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = false;
            this.cancelButton.Click += new EventHandler(this.CancelButton_Click);

            // 
            // ApiKeyConfigForm
            // 
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(500, 280);
            this.Controls.Add(this.configGroupBox);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.instructionLabel);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ApiKeyConfigForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "OpenAI API Key Configuration";
            this.configGroupBox.ResumeLayout(false);
            this.configGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void LoadCurrentStatus()
        {
            string savedApiKey = ConfigurationHelper.LoadApiKey();
            if (!string.IsNullOrEmpty(savedApiKey))
            {
                statusLabel.Text = "Current Status: API Key Configured ?";
                statusLabel.ForeColor = Color.Green;
            }
            else
            {
                statusLabel.Text = "Current Status: No API Key Configured";
                statusLabel.ForeColor = Color.Red;
            }
        }

        private async void SaveButton_Click(object sender, EventArgs e)
        {
            string apiKey = apiKeyTextBox.Text.Trim();
            if (string.IsNullOrEmpty(apiKey))
            {
                MessageBox.Show("Please enter a valid OpenAI API key.", "API Key Required", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            saveButton.Enabled = false;
            saveButton.Text = "Testing...";

            try
            {
                // Test the API key by creating a service and making a simple call
                var testService = new AIChatService(apiKey);
                await testService.SendMessageAsync("Hello");

                // If we get here, the API key works
                ConfigurationHelper.SaveApiKey(apiKey);
                
                MessageBox.Show("API key saved and tested successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"API key test failed: {ex.Message}\n\nPlease check your API key and try again.", 
                    "API Key Test Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                saveButton.Enabled = true;
                saveButton.Text = "Save & Test";
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to clear the saved API key?", 
                "Clear API Key", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                ConfigurationHelper.ClearApiKey();
                apiKeyTextBox.Clear();
                LoadCurrentStatus();
                MessageBox.Show("API key cleared successfully.", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}