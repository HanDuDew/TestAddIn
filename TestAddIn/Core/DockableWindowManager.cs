using System;
using System.Windows.Forms;
using AutoBeau.Utils;

namespace AutoBeau.Core
{
    /// <summary>
    /// Main window for the AddIn
    /// Manages the AutoBeau Dockable Window in Inventor
    /// Handles creation, initialization, and lifecycle of the dockable window
    /// </summary>
    internal class DockableWindowManager
    {
        private global::Inventor.Application m_inventorApplication;
        private global::Inventor.DockableWindow m_dockableWindow;
        private AutoBeauDockableWindow m_dockableWindowControl;
        private const string DOCKABLE_WINDOW_INTERNAL_NAME = "AutoBeau.DockableWindow";

        public DockableWindowManager(global::Inventor.Application inventorApp)
        {
            m_inventorApplication = inventorApp;
        }

        /// <summary>
        /// Creates and initializes the dockable window
        /// </summary>
        public void Initialize()
        {
            try
            {
                // Check if dockable window already exists
                try
                {
                    m_dockableWindow = m_inventorApplication.UserInterfaceManager.DockableWindows[DOCKABLE_WINDOW_INTERNAL_NAME];
                }
                catch
                {
                    // Dockable window doesn't exist, create it
                    CreateDockableWindow();
                }

                if (m_dockableWindow != null)
                {
                    // Create and initialize the user control
                    if (m_dockableWindowControl == null)
                    {
                        m_dockableWindowControl = new AutoBeauDockableWindow();
                        m_dockableWindowControl.SetInventorApplication(m_inventorApplication);
                        m_dockableWindowControl.SetDockableWindow(m_dockableWindow);
                    }

                    // Add the control to the dockable window
                    m_dockableWindow.AddChild(m_dockableWindowControl.Handle);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing dockable window: {ex.Message}\n\nStack Trace: {ex.StackTrace}", 
                    "Dockable Window Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Creates the dockable window using Inventor's API
        /// </summary>
        private void CreateDockableWindow()
        {
            try
            {
                // Create the dockable window with minimal parameters
                m_dockableWindow = m_inventorApplication.UserInterfaceManager.DockableWindows.Add(
                    DOCKABLE_WINDOW_INTERNAL_NAME,     // InternalName
                    System.Guid.NewGuid().ToString(),   // ClientId  
                    "AutoBeau"                          // Title
                );

                // Set additional properties after creation
                if (m_dockableWindow != null)
                {
                    m_dockableWindow.DockingState = global::Inventor.DockingStateEnum.kDockRight;
                    m_dockableWindow.Width = 680;  // Increased from 420 to ensure switch tab is visible
                    m_dockableWindow.Height = 750; // Increased from 600 to provide more space
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating dockable window: {ex.Message}", 
                    "Dockable Window Creation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Loads icon for the dockable window
        /// </summary>
        /// <param name="size">Icon size (16 or 32)</param>
        /// <returns>Icon object for Inventor</returns>
        private object LoadIcon(int size)
        {
            try
            {
                // Try to get the logo from resources and resize it
                var logoResource = AutoBeau.Properties.Resources.logo;
                if (logoResource != null)
                {
                    var resizedBitmap = new System.Drawing.Bitmap(logoResource, new System.Drawing.Size(size, size));
                    return PictureDispConverter.ConvertBitmapToIPictureDisp(resizedBitmap);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading dockable window icon (size {size}): {ex.Message}");
            }
            
            return null;
        }

        /// <summary>
        /// Shows the dockable window
        /// </summary>
        public void ShowWindow()
        {
            try
            {
                if (m_dockableWindow != null)
                {
                    m_dockableWindow.Visible = true;
                }
                else
                {
                    // Initialize if not already done
                    Initialize();
                    if (m_dockableWindow != null)
                    {
                        m_dockableWindow.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error showing dockable window: {ex.Message}", 
                    "Show Window Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Hides the dockable window
        /// </summary>
        public void HideWindow()
        {
            try
            {
                if (m_dockableWindow != null)
                {
                    m_dockableWindow.Visible = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error hiding dockable window: {ex.Message}");
            }
        }

        /// <summary>
        /// Toggles the visibility of the dockable window
        /// </summary>
        public void ToggleWindow()
        {
            try
            {
                if (m_dockableWindow != null)
                {
                    if (m_dockableWindow.Visible)
                    {
                        HideWindow();
                    }
                    else
                    {
                        ShowWindow();
                    }
                }
                else
                {
                    ShowWindow();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error toggling dockable window: {ex.Message}", 
                    "Toggle Window Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Gets whether the dockable window is currently visible
        /// </summary>
        public bool IsVisible
        {
            get
            {
                try
                {
                    return m_dockableWindow?.Visible ?? false;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Adds a system message to the chat window
        /// </summary>
        /// <param name="message">The message to add</param>
        public void AddSystemMessage(string message)
        {
            try
            {
                m_dockableWindowControl?.AddSystemMessage(message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding system message: {ex.Message}");
            }
        }

        /// <summary>
        /// Cleanup resources when add-in is unloaded
        /// </summary>
        public void Cleanup()
        {
            try
            {
                if (m_dockableWindowControl != null)
                {
                    m_dockableWindowControl.Dispose();
                    m_dockableWindowControl = null;
                }

                // Note: We don't dispose the dockable window itself as Inventor manages it
                m_dockableWindow = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during dockable window cleanup: {ex.Message}");
            }
        }
    }
}