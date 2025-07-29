using System;
using System.Configuration;
using System.IO;

namespace InvAddIn.Common
{
    public static class ConfigurationHelper
    {
        private static readonly string ConfigFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "TestAddIn",
            "config.txt");

        public static void SaveApiKey(string apiKey)
        {
            try
            {
                string directory = Path.GetDirectoryName(ConfigFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Simple encoding (not secure for production)
                string encodedKey = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(apiKey));
                File.WriteAllText(ConfigFilePath, encodedKey);
            }
            catch (Exception ex)
            {
                // Log error but don't crash
                System.Diagnostics.Debug.WriteLine($"Error saving API key: {ex.Message}");
            }
        }

        public static string LoadApiKey()
        {
            try
            {
                if (File.Exists(ConfigFilePath))
                {
                    string encodedKey = File.ReadAllText(ConfigFilePath);
                    return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encodedKey));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading API key: {ex.Message}");
            }
            return string.Empty;
        }

        public static void ClearApiKey()
        {
            try
            {
                if (File.Exists(ConfigFilePath))
                {
                    File.Delete(ConfigFilePath);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error clearing API key: {ex.Message}");
            }
        }
    }
}