using Microsoft.Win32;
using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using InvAddIn.Core;

namespace TestAddIn
{
    /// <summary>
    /// This is the primary AddIn Server class that implements the ApplicationAddInServer interface
    /// that all Inventor AddIns are required to implement. The communication between Inventor and
    /// the AddIn is via the methods on this interface.
    /// </summary>
    [GuidAttribute("146b656f-a63b-4e92-8749-5b1470fd6c34")]
    public class StandardAddInServer : global::Inventor.ApplicationAddInServer
    {

        // Inventor application object.
        private global::Inventor.Application m_inventorApplication;
        
        // Button instance for our custom ribbon button
        private InvAddIn.Core.InventorButton m_customButton;

        public StandardAddInServer()
        {
        }

        #region ApplicationAddInServer Members

        public void Activate(global::Inventor.ApplicationAddInSite addInSiteObject, bool firstTime)
        {
            // This method is called by Inventor when it loads the addin.
            // The AddInSiteObject provides access to the Inventor Application object.
            // The FirstTime flag indicates if the addin is loaded for the first time.

            try
            {
                // Set up assembly resolution handler to find NuGet dependencies
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
                
                // Initialize AddIn members.
                m_inventorApplication = addInSiteObject.Application;
                
                // Initialize and create the custom button
                m_customButton = new InvAddIn.Core.InventorButton(m_inventorApplication);
                m_customButton.Initialize();
                
                MessageBox.Show("Test AddIn loaded successfully! Look for the 'Open Test AddIn' button in the ribbon.", "AddIn Loaded", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // Handle exceptions that may occur during initialization.
                MessageBox.Show("Error during AddIn activation: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                // Extract the assembly name
                string assemblyName = new AssemblyName(args.Name).Name;
                
                // Get the directory where our add-in DLL is located
                string addInDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                
                // Look for the assembly in the same directory as our add-in
                string assemblyPath = System.IO.Path.Combine(addInDirectory, assemblyName + ".dll");
                
                if (System.IO.File.Exists(assemblyPath))
                {
                    return Assembly.LoadFrom(assemblyPath);
                }
                
                // Also check common NuGet package locations
                string[] possiblePaths = {
                    System.IO.Path.Combine(addInDirectory, "packages", assemblyName + ".dll"),
                    System.IO.Path.Combine(addInDirectory, "lib", assemblyName + ".dll"),
                    System.IO.Path.Combine(addInDirectory, "Dependencies", assemblyName + ".dll")
                };
                
                foreach (string path in possiblePaths)
                {
                    if (System.IO.File.Exists(path))
                    {
                        return Assembly.LoadFrom(path);
                    }
                }
                
                // Log the missing assembly for debugging
                System.Diagnostics.Debug.WriteLine($"Could not resolve assembly: {args.Name}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error resolving assembly {args.Name}: {ex.Message}");
            }
            
            return null;
        }

        public void Deactivate()
        {
            // This method is called by Inventor when the AddIn is unloaded.
            // The AddIn will be unloaded either manually by the user or
            // when the Inventor session is terminated

            try
            {
                // Remove the assembly resolve handler
                AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
                
                // Clean up the custom button
                if (m_customButton != null)
                {
                    m_customButton.Cleanup();
                    m_customButton = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during button cleanup: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Release objects.
            m_inventorApplication = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void ExecuteCommand(int commandID)
        {
            // Note:this method is now obsolete, you should use the 
            // ControlDefinition functionality for implementing commands.
        }

        public object Automation
        {
            // This property is provided to allow the AddIn to expose an API 
            // of its own to other programs. Typically, this  would be done by
            // implementing the AddIn's API interface in a class and returning 
            // that class object through this property.

            get
            {
                // TODO: Add ApplicationAddInServer.Automation getter implementation
                return null;
            }
        }

        #endregion

    }
}
