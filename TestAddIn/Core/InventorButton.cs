using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Reflection;
using AutoBeau.Forms;
using AutoBeau.Utils;

namespace AutoBeau.Core
{
    internal class InventorButton
    {
        private global::Inventor.Application m_inventorApplication;
        private global::Inventor.ButtonDefinition m_buttonDefinition;
        private DockableWindowManager m_dockableWindowManager; // Changed from MainForm to DockableWindowManager

        public InventorButton(global::Inventor.Application inventorApp)
        {
            m_inventorApplication = inventorApp;
            // Initialize the dockable window manager
            m_dockableWindowManager = new DockableWindowManager(inventorApp);
        }

        public void Initialize()
        {
            try
            {
                // Create the button definition
                global::Inventor.ControlDefinitions controlDefs = m_inventorApplication.CommandManager.ControlDefinitions;
                
                // Load icons for the button (now using PNG files)
                object standardIcon = LoadIcon(16); // 16x16 for standard icon
                object largeIcon = LoadIcon(32);    // 32x32 for large icon
                
                // Check if button already exists
                try
                {
                    m_buttonDefinition = (global::Inventor.ButtonDefinition)controlDefs["AutoBeau_MainFormButton"];
                }
                catch
                {
                    // Button doesn't exist, create it
                    m_buttonDefinition = controlDefs.AddButtonDefinition(
                        "Open AutoBeau", // DisplayName - Updated
                        "AutoBeau_MainFormButton", // InternalName - Updated
                        global::Inventor.CommandTypesEnum.kShapeEditCmdType, // CommandType
                        System.Guid.NewGuid().ToString(), // ClientId
                        "Opens the AutoBeau Dockable Window", // ToolTipText - Updated
                        "Opens the AutoBeau Dockable Window for drawing operations and AI chat assistance", // DescriptionText - Updated
                        standardIcon, // StandardIcon (16x16)
                        largeIcon, // LargeIcon (32x32)
                        global::Inventor.ButtonDisplayEnum.kDisplayTextInLearningMode // ButtonDisplay
                    );
                }

                // Hook up the OnExecute event
                m_buttonDefinition.OnExecute += OnButtonExecute;

                // Add the button to the ribbon
                AddToRibbon();

                // Initialize the dockable window
                m_dockableWindowManager.Initialize();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing button: {ex.Message}\n\nStack Trace: {ex.StackTrace}", "Button Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private object LoadIcon(int size)
        {
            try
            {
                Bitmap bitmap = null;

                // Try to load PNG favicon files first
                string iconFileName = $"favicon{size}.png";
                
                // Method 1: Try to load from embedded resource first
                Assembly assembly = Assembly.GetExecutingAssembly();
                string resourceName = $"AutoBeau.Resources.{iconFileName}";
                
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        bitmap = new Bitmap(stream);
                        return PictureDispConverter.ConvertBitmapToIPictureDisp(bitmap);
                    }
                }

                // Method 2: Try to load from Properties.Resources (existing logo)
                try
                {
                    // Check if we can access the existing logo resource
                    var resourceManager = AutoBeau.Properties.Resources.ResourceManager;
                    if (resourceManager != null)
                    {
                        // Try to get a specific favicon resource
                        object faviconResource = resourceManager.GetObject($"favicon{size}");
                        if (faviconResource is Bitmap faviconBitmap)
                        {
                            return PictureDispConverter.ConvertBitmapToIPictureDisp(faviconBitmap);
                        }
                        
                        // Fallback to the existing logo resource and resize it
                        object logoResource = resourceManager.GetObject("logo");
                        if (logoResource is Bitmap logoBitmap)
                        {
                            // Resize the logo to the desired size
                            bitmap = new Bitmap(logoBitmap, new Size(size, size));
                            return PictureDispConverter.ConvertBitmapToIPictureDisp(bitmap);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Could not load from Properties.Resources: {ex.Message}");
                }

                // Method 3: Try to load from file system
                string assemblyLocation = Assembly.GetExecutingAssembly().Location;
                string assemblyDir = Path.GetDirectoryName(assemblyLocation);
                string iconPath = Path.Combine(assemblyDir, "Resources", iconFileName);
                
                if (File.Exists(iconPath))
                {
                    bitmap = new Bitmap(iconPath);
                    return PictureDispConverter.ConvertBitmapToIPictureDisp(bitmap);
                }

                // Method 4: Try alternative PNG file names
                string[] alternativeNames = {
                    $"icon{size}.png",
                    $"favicon-{size}x{size}.png",
                    $"icon-{size}x{size}.png"
                };

                foreach (string altName in alternativeNames)
                {
                    string altPath = Path.Combine(assemblyDir, "Resources", altName);
                    if (File.Exists(altPath))
                    {
                        bitmap = new Bitmap(altPath);
                        return PictureDispConverter.ConvertBitmapToIPictureDisp(bitmap);
                    }
                }
                
                // If no icons found, return null (button will show without icon)
                System.Diagnostics.Debug.WriteLine($"No PNG icon found for size {size}x{size}");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading PNG icon (size {size}): {ex.Message}");
                return null;
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
                                if (panel.InternalName.Contains("AutoBeau"))
                                {
                                    testPanel = panel;
                                    break;
                                }
                            }

                            // If panel doesn't exist, create it
                            if (testPanel == null)
                            {
                                testPanel = toolsTab.RibbonPanels.Add(
                                    "AutoBeau", // DisplayName - Updated
                                    "AutoBeau_Panel", // InternalName - Updated
                                    System.Guid.NewGuid().ToString() // ClientId
                                );
                            }
                        }
                        catch
                        {
                            // Create a new panel if we can't find or create the desired one
                            testPanel = toolsTab.RibbonPanels.Add(
                                "AutoBeau", // DisplayName - Updated
                                "AutoBeau_Panel", // InternalName - Updated
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
                MessageBox.Show($"Error adding button to ribbon: {ex.Message}\n\nStack Trace: {ex.StackTrace}", "Ribbon Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnButtonExecute(global::Inventor.NameValueMap context)
        {
            try
            {
                // Toggle the dockable window visibility
                m_dockableWindowManager.ToggleWindow();
                
                // Add a system message if the window is now visible
                if (m_dockableWindowManager.IsVisible)
                {
                    // m_dockableWindowManager.AddSystemMessage("AutoBeau dockable window opened. Ready for operations!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error toggling dockable window: {ex.Message}\n\nStack Trace: {ex.StackTrace}", 
                    "Dockable Window Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Cleanup()
        {
            try
            {
                // Clean up the dockable window manager
                if (m_dockableWindowManager != null)
                {
                    m_dockableWindowManager.Cleanup();
                    m_dockableWindowManager = null;
                }

                if (m_buttonDefinition != null)
                {
                    m_buttonDefinition.OnExecute -= OnButtonExecute;
                    m_buttonDefinition = null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during button cleanup: {ex.Message}");
            }
        }
    }
}