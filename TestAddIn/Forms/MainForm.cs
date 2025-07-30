using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InvAddIn.Common;
using InvAddIn.Inventor;

namespace InvAddIn.Forms
{
    public partial class MainForm : Form
    {
        private global::Inventor.Application m_inventorApplication;
        private bool _isAIMode = false; // Track current mode

        public MainForm()
        {
            InitializeComponent();
            // Initialize in manual mode
            SetMode(false);
            
            // Configure form for modeless operation
            this.StartPosition = FormStartPosition.Manual;
            this.ShowInTaskbar = true;
            this.TopMost = true; // Keep the form on top of other windows
            this.FormBorderStyle = FormBorderStyle.Sizable; // Allow resizing
            this.MinimizeBox = true; // Allow minimizing to taskbar
            this.MaximizeBox = false; // Disable maximize to prevent full screen blocking
        }

        /// <summary>
        /// Sets the Inventor Application reference for this form
        /// </summary>
        /// <param name="inventorApp">The Inventor Application object</param>
        public void SetInventorApplication(global::Inventor.Application inventorApp)
        {
            m_inventorApplication = inventorApp;
        }

        private void SetMode(bool isAIMode)
        {
            _isAIMode = isAIMode;

            if (isAIMode)
            {
                // AI Chat Mode - Hide manual controls, show expanded chat
                controlBox.Visible = false;
                button1.Visible = false;
                pictureBox1.Visible = false; // Hide the picture box in AI mode
                chatWindow.Visible = true;

                // Update menu text and appearance
                switchToolStripMenuItem.Text = "Switch to Manual Mode";

                // Resize chat window to take more space
                chatWindow.Location = new Point(22, 48); // Start right after menu bar
                chatWindow.Size = new Size(936, 590);
                
                // Update window title
                this.Text = "AutoBeau - AI Chat Mode";
            }
            else
            {
                // Manual Control Mode - Show controls, hide chat
                controlBox.Visible = true;
                button1.Visible = true;
                pictureBox1.Visible = true; // Show the picture box in manual mode
                chatWindow.Visible = false; // Hide chat window in manual mode

                // Update menu text and appearance
                switchToolStripMenuItem.Text = "Switch to AI Mode";

                // Reset chat window to original size and position (for when we switch back)
                chatWindow.Location = new Point(417, 84);
                chatWindow.Size = new Size(541, 565);
                
                // Update window title
                this.Text = "AutoBeau - Manual Mode";
            }
            
            // Force refresh
            this.Refresh();
        }

        private void switchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetMode(!_isAIMode);
        }

        private void modeToggleButton_Click(object sender, EventArgs e)
        {
            SetMode(!_isAIMode);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_inventorApplication == null)
                {
                    MessageBox.Show("Inventor Application reference not set.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Disable the button to prevent multiple clicks
                button1.Enabled = false;
                button1.Text = "Processing...";

                try
                {
                    // Check if Add Hole Table checkbox is checked (checkBox2)
                    if (checkBox2.Checked)
                    {
                        try
                        {
                            // Execute the CreateHoleTable functionality
                            CreateHoleTable holeTableCreator = new CreateHoleTable();
                            holeTableCreator.CreateHoleTables(m_inventorApplication);
                            
                            // Show success message
                            MessageBox.Show("Hole table created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error creating hole table: {ex.Message}", "Hole Table Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    // Check if Auto Arrange checkbox is checked
                    if (autoArrangeCheckBox.Checked)
                    {
                        try
                        {
                            // Execute the AutoArrange functionality
                            AutoArrange autoArrange = new AutoArrange();
                            autoArrange.ArrangeAllGeneralDimensions(m_inventorApplication);
                            
                            // Show success message
                            MessageBox.Show("Dimensions arranged automatically!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error arranging dimensions: {ex.Message}", "Auto Arrange Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    if (checkBox3.Checked)
                    {
                        // TODO: Implement add centermarks functionality
                        MessageBox.Show("Add Centermarks functionality not yet implemented.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    // If no checkboxes are selected
                    if (!checkBox2.Checked && !autoArrangeCheckBox.Checked && !checkBox3.Checked)
                    {
                        MessageBox.Show("Please select at least one option before clicking Apply.", "No Options Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                finally
                {
                    // Re-enable the button
                    button1.Enabled = true;
                    button1.Text = "Apply";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error executing operation: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                button1.Enabled = true;
                button1.Text = "Apply";
            }
        }

        private void configureAPIKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var configForm = new ApiKeyConfigForm())
            {
                if (configForm.ShowDialog(this) == DialogResult.OK)
                {
                    // API key was successfully configured
                    MessageBox.Show("API key configuration updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // If we're in AI mode, reinitialize the chat service
                    if (_isAIMode && chatWindow.Visible)
                    {
                        chatWindow.InitializeAIService();
                    }
                }
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string currentMode = _isAIMode ? "AI Chat Mode" : "Manual Mode";
            string aboutMessage = "AutoBeau - Beautify your drawing with AI Chat\n\n" +
                                  "Version: 1.0\n" +
                                  "This add-in helps you create hole tables, arrange dimensions, " +
                                  "and provides AI assistance for your Inventor project.\n\n" +
                                  "Features:\n" +
                                  "? Manual Controls: Precise operations selection\n" +
                                  "? AI Chat Mode: Interactive assistance and guidance\n" +
                                  "? Hole Table Generation\n" +
                                  "? Auto Dimension Arrangement\n" +
                                  "? Centermarks (Coming Soon)\n\n" +
                                  $"Current Mode: {currentMode}\n" +
                                  "Toggle between modes using the menu bar switch option.\n\n" +
                                  "Note: This window stays on top of other windows to remain\n" +
                                  "accessible while working. You can minimize it to the taskbar\n" +
                                  "when not needed.\n\n" +
                                  "For support, please configure your OpenAI API key in Settings.";

            MessageBox.Show(aboutMessage, "About AutoBeau", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Initialize form settings
            this.Text = "AutoBeau - Manual Mode";
            
            // Position the form to not overlap with Inventor
            PositionFormNextToInventor();
            
            // Show welcome message (only once when form loads)
            if (!_hasShownWelcome)
            {
                _hasShownWelcome = true;
                MessageBox.Show("Welcome to AutoBeau!\n\n" +
                               "This window stays on top to remain accessible while you work.\n" +
                               "You can minimize it to the taskbar when not needed.\n\n" +
                               "Manual Mode: Use checkboxes to select operations and click Apply.\n" +
                               "AI Mode: Switch using the menu bar for interactive chat assistance.\n\n" +
                               "Configure your OpenAI API key in Settings for AI functionality.",
                               "Welcome to AutoBeau", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private bool _hasShownWelcome = false;

        private void PositionFormNextToInventor()
        {
            try
            {
                // Position the form in a convenient location that doesn't block Inventor's main workspace
                // Place it in the upper right area of the screen
                var screen = Screen.PrimaryScreen.WorkingArea;
                this.Location = new Point(screen.Width - this.Width - 50, 50);
                this.StartPosition = FormStartPosition.Manual;
            }
            catch
            {
                // If positioning fails, use default
                this.Location = new Point(200, 100);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Allow the form to close normally
            base.OnFormClosing(e);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chatWindow_Load(object sender, EventArgs e)
        {

        }
    }
}