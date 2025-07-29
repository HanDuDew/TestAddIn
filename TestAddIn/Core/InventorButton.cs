using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InvAddIn.Forms;

namespace InvAddIn.Core
{
    internal class InventorButton
    {
        private global::Inventor.Application m_inventorApplication;
        private global::Inventor.ButtonDefinition m_buttonDefinition;

        public InventorButton(global::Inventor.Application inventorApp)
        {
            m_inventorApplication = inventorApp;
        }

        public void Initialize()
        {
            try
            {
                // Create the button definition
                global::Inventor.ControlDefinitions controlDefs = m_inventorApplication.CommandManager.ControlDefinitions;
                
                // Check if button already exists
                try
                {
                    m_buttonDefinition = (global::Inventor.ButtonDefinition)controlDefs["TestAddIn_MainFormButton"];
                }
                catch
                {
                    // Button doesn't exist, create it
                    m_buttonDefinition = controlDefs.AddButtonDefinition(
                        "Open Test AddIn", // DisplayName
                        "TestAddIn_MainFormButton", // InternalName
                        global::Inventor.CommandTypesEnum.kShapeEditCmdType, // CommandType
                        System.Guid.NewGuid().ToString(), // ClientId
                        "Opens the Test Add In Main Form", // ToolTipText
                        "Opens the Test AddIn Main Form for hole table operations", // DescriptionText
                        null, // StandardIcon
                        null, // LargeIcon
                        global::Inventor.ButtonDisplayEnum.kDisplayTextInLearningMode // ButtonDisplay
                    );
                }

                // Hook up the OnExecute event
                m_buttonDefinition.OnExecute += OnButtonExecute;

                // Add the button to the ribbon
                AddToRibbon();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing button: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddToRibbon()
        {
            try
            {
                // Get the ribbon for Drawing environment first (since we're working with hole tables)
                global::Inventor.Ribbons ribbons = m_inventorApplication.UserInterfaceManager.Ribbons;
                
                global::Inventor.Ribbon ribbon = null;
                try
                {
                    ribbon = ribbons["Drawing"]; // Try Drawing first
                }
                catch
                {
                    try
                    {
                        ribbon = ribbons["Part"]; // Fall back to Part
                    }
                    catch
                    {
                        try
                        {
                            ribbon = ribbons["Assembly"]; // Fall back to Assembly
                        }
                        catch
                        {
                            // Use the first available ribbon
                            if (ribbons.Count > 0)
                            {
                                ribbon = ribbons[1];
                            }
                        }
                    }
                }

                if (ribbon != null)
                {
                    // Get or create the Tools tab
                    global::Inventor.RibbonTab toolsTab = null;
                    try
                    {
                        toolsTab = ribbon.RibbonTabs["id_TabTools"];
                    }
                    catch
                    {
                        // If Tools tab doesn't exist, use the first available tab
                        if (ribbon.RibbonTabs.Count > 0)
                        {
                            toolsTab = ribbon.RibbonTabs[1];
                        }
                    }

                    if (toolsTab != null)
                    {
                        // Create or get a panel for our button
                        global::Inventor.RibbonPanel testPanel = null;
                        try
                        {
                            // Try to find existing panel
                            foreach (global::Inventor.RibbonPanel panel in toolsTab.RibbonPanels)
                            {
                                if (panel.InternalName.Contains("TestAddIn"))
                                {
                                    testPanel = panel;
                                    break;
                                }
                            }

                            // If panel doesn't exist, create it
                            if (testPanel == null)
                            {
                                testPanel = toolsTab.RibbonPanels.Add(
                                    "Test AddIn", // DisplayName
                                    "TestAddIn_Panel", // InternalName
                                    System.Guid.NewGuid().ToString() // ClientId
                                );
                            }
                        }
                        catch
                        {
                            // Create a new panel if we can't find or create the desired one
                            testPanel = toolsTab.RibbonPanels.Add(
                                "Test AddIn", // DisplayName
                                "TestAddIn_Panel", // InternalName
                                System.Guid.NewGuid().ToString() // ClientId
                            );
                        }

                        if (testPanel != null)
                        {
                            // Add the button to the panel
                            testPanel.CommandControls.AddButton(m_buttonDefinition, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding button to ribbon: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnButtonExecute(global::Inventor.NameValueMap context)
        {
            try
            {
                // Launch the MainForm when button is clicked
                MainForm mainForm = new MainForm();
                mainForm.SetInventorApplication(m_inventorApplication);
                mainForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error launching main form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Cleanup()
        {
            try
            {
                if (m_buttonDefinition != null)
                {
                    m_buttonDefinition.OnExecute -= OnButtonExecute;
                    m_buttonDefinition = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during button cleanup: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}