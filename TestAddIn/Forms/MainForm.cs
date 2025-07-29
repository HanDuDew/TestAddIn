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

        public MainForm()
        {
            InitializeComponent();
            // Menu items are now properly configured in the designer
        }

        /// <summary>
        /// Sets the Inventor Application reference for this form
        /// </summary>
        /// <param name="inventorApp">The Inventor Application object</param>
        public void SetInventorApplication(global::Inventor.Application inventorApp)
        {
            m_inventorApplication = inventorApp;
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

                // Check if Add Hole Table checkbox is checked (checkBox2)
                if (checkBox2.Checked)
                {
                    try
                    {
                        // Execute the CreateHoleTable functionality
                        CreateHoleTable holeTableCreator = new CreateHoleTable();
                        holeTableCreator.CreateHoleTables(m_inventorApplication);
                        
                        // Add success message to chat
                        chatWindow.AddSystemMessage("Hole table created successfully!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error creating hole table: {ex.Message}", "Hole Table Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        chatWindow.AddSystemMessage($"Error creating hole table: {ex.Message}");
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
                        
                        // Add success message to chat
                        chatWindow.AddSystemMessage("Dimensions arranged automatically!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error arranging dimensions: {ex.Message}", "Auto Arrange Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        chatWindow.AddSystemMessage($"Error arranging dimensions: {ex.Message}");
                    }
                }

                if (checkBox3.Checked)
                {
                    // TODO: Implement add centermarks functionality
                    MessageBox.Show("Add Centermarks functionality not yet implemented.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    chatWindow.AddSystemMessage("Add Centermarks functionality not yet implemented.");
                }

                // If no checkboxes are selected
                if (!checkBox2.Checked && !autoArrangeCheckBox.Checked && !checkBox3.Checked)
                {
                    MessageBox.Show("Please select at least one option before clicking Generate.", "No Options Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error executing operation: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                chatWindow.AddSystemMessage($"Error executing operation: {ex.Message}");
            }
        }

        private void configureAPIKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var configForm = new ApiKeyConfigForm())
            {
                if (configForm.ShowDialog(this) == DialogResult.OK)
                {
                    // API key was successfully configured, refresh the chat window
                    chatWindow.InitializeAIService();
                    chatWindow.AddSystemMessage("API key configuration updated successfully!");
                }
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string aboutMessage = "AutoBeau - Beautify your drawing with AI Chat\n\n" +
                                  "Version: 1.0\n" +
                                  "This add-in helps you create hole tables, arrange dimensions, " +
                                  "and provides AI assistance for your Inventor project.\n\n" +
                                  "Features:\n" +
                                  "? Hole Table Generation\n" +
                                  "? Auto Dimension Arrangement\n" +
                                  "? AI Chat Assistant\n" +
                                  "? Centermarks (Coming Soon)\n\n" +
                                  "For support, please configure your OpenAI API key in Settings.";

            MessageBox.Show(aboutMessage, "About Test AddIn", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Initialize form settings if needed
            this.Text = "AutoBeau - Beautify your drawing with AI Chat";
            
            // Set default checkbox states if desired
            // checkBox2.Checked = true; // Default to creating hole table
            
            // Add welcome message to chat
            chatWindow.AddSystemMessage("Welcome to the Inventor AddIn with AI Chat! You can ask me questions about Inventor API, CAD operations, or anything related to this add-in.");
            
            // Check if API key is already saved
            string savedApiKey = ConfigurationHelper.LoadApiKey();
            if (string.IsNullOrEmpty(savedApiKey))
            {
                chatWindow.AddSystemMessage("To get started, please configure your OpenAI API key via Settings ¡ú Configure API Key.");
            }
            else
            {
                chatWindow.AddSystemMessage("Your API key is already configured. You can start chatting with the AI!");
            }
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