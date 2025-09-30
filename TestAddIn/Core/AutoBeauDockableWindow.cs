using System;
using System.Drawing;
using System.Windows.Forms;
using AutoBeau.Common;
using AutoBeau.Inventor;
using AutoBeau.Forms;
using AutoBeau.Services;
using AutoBeau.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoBeau.Core
{
    /// <summary>
    /// AutoBeau Dockable Window - Native Inventor integration
    /// Migrated from MainForm to provide better Inventor integration
    /// </summary>
    public partial class AutoBeauDockableWindow : UserControl
    {
        private global::Inventor.Application m_inventorApplication;
        private global::Inventor.DockableWindow m_dockableWindow;
        private bool _isAIMode = false; // Track current mode
        private bool _hasShownWelcome = false;

        // UI Controls - migrated from MainForm
        private MenuStrip menuStrip;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem configureAPIKeyToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem switchToolStripMenuItem;
        
        private GroupBox controlBox;
        private CheckBox autoArrangeCheckBox;
        private CheckBox checkBox2; // Hole Table
        private CheckBox checkBox3; // Centermarks
        private CheckBox retrieveDimsCheckBox; // Retrieve Dimensions
        private Button applyButton; // Apply button
        private PictureBox logoBox;
        private ToolStripMenuItem chooseToolStripMenuItem;
        private Panel statusPanel;
        private Label currentUserLabel;
        private Label currentMachineLabel;
        private ChatWindow chatWindow;

        public AutoBeauDockableWindow()
        {
            InitializeComponent();
            SetMode(false); // Initialize in manual mode
        }

        /// <summary>
        /// Sets the Inventor Application reference for this dockable window
        /// </summary>
        /// <param name="inventorApp">The Inventor Application object</param>
        public void SetInventorApplication(global::Inventor.Application inventorApp)
        {
            m_inventorApplication = inventorApp;
            InventorContextService.Instance.Initialize(inventorApp);
        }

        /// <summary>
        /// Sets the dockable window reference
        /// </summary>
        /// <param name="dockableWindow">The Inventor DockableWindow object</param>
        public void SetDockableWindow(global::Inventor.DockableWindow dockableWindow)
        {
            m_dockableWindow = dockableWindow;
        }

        private void InitializeComponent()
        {
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configureAPIKeyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chooseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.switchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.controlBox = new System.Windows.Forms.GroupBox();
            this.retrieveDimsCheckBox = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.autoArrangeCheckBox = new System.Windows.Forms.CheckBox();
            this.applyButton = new System.Windows.Forms.Button();
            this.logoBox = new System.Windows.Forms.PictureBox();
            this.statusPanel = new System.Windows.Forms.Panel();
            this.currentMachineLabel = new System.Windows.Forms.Label();
            this.currentUserLabel = new System.Windows.Forms.Label();
            this.chatWindow = new AutoBeau.Forms.ChatWindow();
            this.menuStrip.SuspendLayout();
            this.controlBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoBox)).BeginInit();
            this.statusPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.switchToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(619, 39);
            this.menuStrip.TabIndex = 0;
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configureAPIKeyToolStripMenuItem,
            this.chooseToolStripMenuItem});
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
            // chooseToolStripMenuItem
            // 
            this.chooseToolStripMenuItem.Name = "chooseToolStripMenuItem";
            this.chooseToolStripMenuItem.Size = new System.Drawing.Size(355, 44);
            this.chooseToolStripMenuItem.Text = "Choose";
            this.chooseToolStripMenuItem.Click += new System.EventHandler(this.chooseToolStripMenuItem_Click);
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
            this.switchToolStripMenuItem.Name = "switchToolStripMenuItem";
            this.switchToolStripMenuItem.Size = new System.Drawing.Size(245, 35);
            this.switchToolStripMenuItem.Text = "Switch to AI Mode";
            this.switchToolStripMenuItem.Click += new System.EventHandler(this.switchToolStripMenuItem_Click);
            // 
            // controlBox
            // 
            this.controlBox.Controls.Add(this.retrieveDimsCheckBox);
            this.controlBox.Controls.Add(this.checkBox3);
            this.controlBox.Controls.Add(this.checkBox2);
            this.controlBox.Controls.Add(this.autoArrangeCheckBox);
            this.controlBox.Location = new System.Drawing.Point(398, 822);
            this.controlBox.Name = "controlBox";
            this.controlBox.Size = new System.Drawing.Size(380, 296);
            this.controlBox.TabIndex = 2;
            this.controlBox.TabStop = false;
            this.controlBox.Text = "Manual Controls";
            this.controlBox.Visible = false;
            this.controlBox.Enter += new System.EventHandler(this.controlBox_Enter);
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
            this.retrieveDimsCheckBox.Visible = false;
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
            this.checkBox3.Visible = false;
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
            this.checkBox2.Visible = false;
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
            this.autoArrangeCheckBox.Visible = false;
            // 
            // applyButton
            // 
            this.applyButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.applyButton.Location = new System.Drawing.Point(235, 554);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(111, 60);
            this.applyButton.TabIndex = 3;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // logoBox
            // 
            this.logoBox.Image = global::AutoBeau.Properties.Resources.logo;
            this.logoBox.Location = new System.Drawing.Point(104, 143);
            this.logoBox.Name = "logoBox";
            this.logoBox.Size = new System.Drawing.Size(379, 60);
            this.logoBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.logoBox.TabIndex = 1;
            this.logoBox.TabStop = false;
            // 
            // statusPanel
            // 
            this.statusPanel.AutoSize = true;
            this.statusPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.statusPanel.Controls.Add(this.currentMachineLabel);
            this.statusPanel.Controls.Add(this.currentUserLabel);
            this.statusPanel.ForeColor = System.Drawing.Color.Black;
            this.statusPanel.Location = new System.Drawing.Point(72, 270);
            this.statusPanel.Name = "statusPanel";
            this.statusPanel.Size = new System.Drawing.Size(448, 225);
            this.statusPanel.TabIndex = 5;
            // 
            // currentMachineLabel
            // 
            this.currentMachineLabel.AutoSize = true;
            this.currentMachineLabel.Location = new System.Drawing.Point(12, 66);
            this.currentMachineLabel.Name = "currentMachineLabel";
            this.currentMachineLabel.Size = new System.Drawing.Size(322, 24);
            this.currentMachineLabel.TabIndex = 7;
            this.currentMachineLabel.Text = "Current Machine: RCS25031N";
            // 
            // currentUserLabel
            // 
            this.currentUserLabel.AutoSize = true;
            this.currentUserLabel.Location = new System.Drawing.Point(12, 17);
            this.currentUserLabel.Name = "currentUserLabel";
            this.currentUserLabel.Size = new System.Drawing.Size(250, 24);
            this.currentUserLabel.TabIndex = 6;
            this.currentUserLabel.Text = "Current User: han.du";
            // 
            // chatWindow
            // 
            this.chatWindow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.chatWindow.Location = new System.Drawing.Point(12, 809);
            this.chatWindow.Name = "chatWindow";
            this.chatWindow.Size = new System.Drawing.Size(380, 400);
            this.chatWindow.TabIndex = 4;
            this.chatWindow.Visible = false;
            this.chatWindow.Load += new System.EventHandler(this.chatWindow_Load);
            // 
            // AutoBeauDockableWindow
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.statusPanel);
            this.Controls.Add(this.chatWindow);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.controlBox);
            this.Controls.Add(this.logoBox);
            this.Controls.Add(this.menuStrip);
            this.Name = "AutoBeauDockableWindow";
            this.Size = new System.Drawing.Size(619, 1153);
            this.Load += new System.EventHandler(this.AutoBeauDockableWindow_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.controlBox.ResumeLayout(false);
            this.controlBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoBox)).EndInit();
            this.statusPanel.ResumeLayout(false);
            this.statusPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void SetMode(bool isAIMode)
        {
            _isAIMode = isAIMode;
            InventorContextService.Instance.UpdateUIMode(isAIMode);

            if (isAIMode)
            {
                // AI Chat Mode - Hide manual controls, show expanded chat
                controlBox.Visible = false;
                applyButton.Visible = false;
                logoBox.Visible = false;
                chatWindow.Visible = true;

                // Update menu text
                switchToolStripMenuItem.Text = "Switch to Manual Mode";

                // Resize chat window to take more space
                chatWindow.Location = new Point(10, 35);
                chatWindow.Size = new Size(this.Width - 20, this.Height - 45);

                // Hide status panel in AI mode
                statusPanel.Visible = false;
            }
            else
            {
                // Manual Control Mode - Show controls, hide chat
                controlBox.Visible = false; //bsolete, moved to menustrip bar
                applyButton.Visible = true;
                logoBox.Visible = true;
                chatWindow.Visible = false;

                // Update menu text
                switchToolStripMenuItem.Text = "Switch to AI Mode";

                // Reset chat window position for when we switch back
                chatWindow.Location = new Point(10, 85);
                chatWindow.Size = new Size(380, 400);

                // Show status panel in manual mode
                statusPanel.Visible = true;
            }

            this.Refresh();
        }

        private void AutoBeauDockableWindow_Load(object sender, EventArgs e)
        {
            // Initialize MCP server (safe multi-call) so chat or future tooling can invoke methods
            try
            {
                AutoBeau.MCP.InventorMCPServer.Instance.Initialize(m_inventorApplication, this);
            }
            catch (Exception mcpEx)
            {
                System.Diagnostics.Debug.WriteLine("MCP init error: " + mcpEx.Message);
            }

            // Load saved method selections
            LoadMethodSelections();
            SyncContextMethodSelections();

            // Show welcome message (only once when window loads)
            if (!_hasShownWelcome)
            {
                _hasShownWelcome = true;
                MessageBox.Show("Welcome to AutoBeau Dockable Window!\n\n",
                               "Welcome to AutoBeau", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // Initialize chat window
            if (chatWindow != null)
            {
                // Check if API key is already saved
                string savedApiKey = ConfigurationHelper.LoadApiKey();
                if (string.IsNullOrEmpty(savedApiKey))
                {
                    chatWindow.AddSystemMessage("To use AI mode, please configure your OpenAI API key via Settings ¡ú Configure API Key.");
                }
                else
                {
                    chatWindow.AddSystemMessage("Your API key is configured. AI mode is ready to use!");
                }

                // Add quick MCP help message
                chatWindow.AddSystemMessage("MCP tools ready. Try: list methods, select methods, execute method <name>.");
            }
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
                applyButton.Enabled = false;
                applyButton.Text = "Apply";

                try
                {
                    // Create the queued methods helper
                    var queuedHelper = new QueuedInventorMethodsHelper(m_inventorApplication);

                    // Set the current selections based on checkbox states
                    queuedHelper.SetMethodSelections(
                        retrieveDimsCheckBox.Checked,
                        autoArrangeCheckBox.Checked,
                        checkBox2.Checked,
                        checkBox3.Checked
                    );

                    // Execute all selected methods in priority order
                    var results = queuedHelper.ExecuteSelectedMethods();

                    // Process and display results
                    ProcessExecutionResults(results);
                }
                finally
                {
                    // Re-enable the button
                    applyButton.Enabled = true;
                    applyButton.Text = "Apply";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error executing operations: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                applyButton.Enabled = true;
                applyButton.Text = "Apply";
            }
        }

        /// <summary>
        /// Process and display the execution results from the queued methods
        /// </summary>
        /// <param name="results">List of method execution results</param>
        private void ProcessExecutionResults(List<MethodExecutionResult> results)
        {
            if (!results.Any())
            {
                return;
            }

            var successCount = results.Count(r => r.Success);
            var warningCount = results.Count(r => !r.Success && r.IsWarning);
            var errorCount = results.Count(r => !r.Success && !r.IsWarning);

            // Build summary message
            var summaryBuilder = new StringBuilder();
            summaryBuilder.AppendLine("Execution Results:");
            summaryBuilder.AppendLine($" + Successful: {successCount}");
            if (warningCount > 0)
                summaryBuilder.AppendLine($" - Warnings: {warningCount}");
            if (errorCount > 0)
                summaryBuilder.AppendLine($" - Errors: {errorCount}");
            summaryBuilder.AppendLine();

            // Add detailed results
            foreach (var result in results.OrderBy(r => r.Success ? 0 : 1)) // Show successes first
            {
                string icon = result.Success ? "+" : (result.IsWarning ? "-" : "--");
                summaryBuilder.AppendLine($"{icon} {result.MethodName}: {result.Message}");
            }

            // Add note about the improved workflow if successful
            if (successCount > 0 && errorCount == 0)
            {
                summaryBuilder.AppendLine();
                summaryBuilder.AppendLine("Note: All operations used the same selected drawing view for consistency.");
            }

            // Determine message box icon based on results
            MessageBoxIcon messageIcon;
            string title;
            if (errorCount > 0)
            {
                messageIcon = MessageBoxIcon.Error;
                title = "Execution Completed with Errors";
            }
            else if (warningCount > 0)
            {
                messageIcon = MessageBoxIcon.Warning;
                title = "Execution Completed with Warnings";
            }
            else if (successCount > 0)
            {
                messageIcon = MessageBoxIcon.Information;
                title = "Execution Completed Successfully";
            }
            else
            {
                messageIcon = MessageBoxIcon.Warning;
                title = "No Operations Selected";
            }

            MessageBox.Show(summaryBuilder.ToString(), title, MessageBoxButtons.OK, messageIcon);
        }

        private void switchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Toggle between AI mode and manual mode
            SetMode(!_isAIMode);
        }

        private void configureAPIKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var configForm = new ApiKeyConfigForm())
            {
                if (configForm.ShowDialog() == DialogResult.OK)
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
                                  "+ Manual Controls: Precise operations selection\n" +
                                  "+ AI Chat Mode: Interactive assistance and guidance\n" +
                                  "+ Hole Table Generation\n" +
                                  "+ Auto Dimension Arrangement\n" +
                                  "+ Retrieve Dimensions \n" +
                                  "- Centermarks (Coming Soon)\n\n" +
                                  $"Current Mode: {currentMode}\n" +
                                  "Toggle between modes using the menu switch option.\n\n" +
                                  "Note: This dockable window integrates natively with Inventor\n" +
                                  "and can be docked, undocked, or resized as needed.\n\n" +
                                  "For support, please configure your OpenAI API key in Settings.";

            MessageBox.Show(aboutMessage, "About AutoBeau", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void chooseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var chooseMethodsForm = new ChooseInventorMethodsForm())
            {
                // Set current selections in the form
                chooseMethodsForm.SetCurrentSelections(
                    autoArrangeCheckBox.Checked,
                    checkBox2.Checked,
                    checkBox3.Checked,
                    retrieveDimsCheckBox.Checked
                );

                // Show the form as a modal dialog
                if (chooseMethodsForm.ShowDialog(this) == DialogResult.OK)
                {
                    // Get the selections from the form
                    bool autoArrange, holeTable, centermarks, retrieveDims;
                    chooseMethodsForm.GetSelections(out autoArrange, out holeTable, out centermarks, out retrieveDims);

                    // Update the main window checkboxes
                    autoArrangeCheckBox.Checked = autoArrange;
                    checkBox2.Checked = holeTable;
                    checkBox3.Checked = centermarks;
                    retrieveDimsCheckBox.Checked = retrieveDims;

                    // Save the selections to preferences
                    SaveMethodSelections();

                    // Optional: Show confirmation message
                    //MessageBox.Show("Inventor method selections updated successfully!", "Settings Updated", 
                    //    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// Handle window resize to adjust child controls
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (_isAIMode && chatWindow != null && chatWindow.Visible)
            {
                // Resize chat window to fill available space in AI mode
                chatWindow.Size = new Size(this.Width - 20, this.Height - 45);
            }
            // TODO: Handle resizing for manual mode (logo and panel)
        }

        /// <summary>
        /// Public method to add system messages to chat (for external use)
        /// </summary>
        /// <param name="message">The message to add</param>
        public void AddSystemMessage(string message)
        {
            if (chatWindow != null)
            {
                chatWindow.AddSystemMessage(message);
            }
        }

        /// <summary>
        /// Save current method selections to user preferences
        /// </summary>
        private void SaveMethodSelections()
        {
            try
            {
                // Using Windows Registry or app settings to save preferences
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"SOFTWARE\AutoBeau\MethodSelections");
                key.SetValue("AutoArrange", autoArrangeCheckBox.Checked);
                key.SetValue("HoleTable", checkBox2.Checked);
                key.SetValue("Centermarks", checkBox3.Checked);
                key.SetValue("RetrieveDimensions", retrieveDimsCheckBox.Checked);
                key.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving method selections: {ex.Message}");
            }

            SyncContextMethodSelections();
        }

        /// <summary>
        /// Load method selections from user preferences
        /// </summary>
        private void LoadMethodSelections()
        {
            try
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\AutoBeau\MethodSelections");
                if (key != null)
                {
                    autoArrangeCheckBox.Checked = bool.Parse(key.GetValue("AutoArrange", "false").ToString());
                    checkBox2.Checked = bool.Parse(key.GetValue("HoleTable", "false").ToString());
                    checkBox3.Checked = bool.Parse(key.GetValue("Centermarks", "false").ToString());
                    retrieveDimsCheckBox.Checked = bool.Parse(key.GetValue("RetrieveDimensions", "false").ToString());
                    key.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading method selections: {ex.Message}");
            }

            SyncContextMethodSelections();
        }

        private void chatWindow_Load(object sender, EventArgs e)
        {

        }

        private void controlBox_Enter(object sender, EventArgs e)
        {

        }

        private void SyncContextMethodSelections()
        {
            try
            {
                if (retrieveDimsCheckBox == null)
                {
                    return;
                }

                InventorContextService.Instance.UpdateMethodSelections(
                    retrieveDimsCheckBox.Checked,
                    autoArrangeCheckBox.Checked,
                    checkBox2.Checked,
                    checkBox3.Checked);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Context sync error: {ex.Message}");
            }
        }
    }
}
