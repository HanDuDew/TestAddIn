using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AutoBeau.Utils
{
    /// <summary>
    /// Utility class for converting System.Drawing.Bitmap objects to COM IPictureDisp objects
    /// required by Inventor's ribbon button icons
    /// </summary>
    internal static class PictureDispConverter
    {
        /// <summary>
        /// Converts a .NET Bitmap to COM IPictureDisp object for use with Inventor ribbon buttons
        /// </summary>
        /// <param name="bitmap">The bitmap to convert</param>
        /// <returns>IPictureDisp object or null if conversion fails</returns>
        public static object ConvertBitmapToIPictureDisp(Bitmap bitmap)
        {
            try
            {
                if (bitmap == null) return null;

                // Method 1: Try using AxHost.GetIPictureDispFromPicture via reflection
                try
                {
                    // Access the protected static method via reflection
                    var method = typeof(AxHost).GetMethod("GetIPictureDispFromPicture", 
                        BindingFlags.Static | BindingFlags.NonPublic);
                    if (method != null)
                    {
                        return method.Invoke(null, new object[] { bitmap });
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"AxHost method failed: {ex.Message}");
                }

                // Method 2: Use Windows API to convert bitmap to IPictureDisp
                IntPtr hBitmap = bitmap.GetHbitmap(Color.Transparent);
                try
                {
                    // Create IPictureDisp from HBITMAP using OLE automation
                    var pictDesc = new PICTDESC.Bitmap
                    {
                        cbSizeOfStruct = Marshal.SizeOf<PICTDESC.Bitmap>(),
                        picType = PICTYPE.PICTYPE_BITMAP,
                        hbitmap = hBitmap,
                        hpal = IntPtr.Zero
                    };

                    // Use IID_IPictureDisp GUID directly
                    Guid iPictureDispGuid = new Guid("7BF80981-BF32-101A-8BBB-00AA00300CAB");
                    object picture;
                    int hr = OleCreatePictureIndirect(ref pictDesc, ref iPictureDispGuid, true, out picture);
                    
                    if (hr == 0) // S_OK
                    {
                        return picture;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"OleCreatePictureIndirect failed with HRESULT: 0x{hr:X8}");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Windows API method failed: {ex.Message}");
                }
                finally
                {
                    DeleteObject(hBitmap);
                }

                // Method 3: Fallback - return null to use text-only button
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error converting bitmap to IPictureDisp: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Convenience method to convert bitmap and handle null cases gracefully
        /// </summary>
        /// <param name="bitmap">The bitmap to convert (can be null)</param>
        /// <returns>IPictureDisp object or null</returns>
        public static object TryConvertBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                return null;
                
            try
            {
                return ConvertBitmapToIPictureDisp(bitmap);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to convert bitmap: {ex.Message}");
                return null;
            }
        }

        #region WIN32 API and COM Interop

        [DllImport("oleaut32.dll")]
        private static extern int OleCreatePictureIndirect(
            ref PICTDESC.Bitmap pPictDesc,
            ref Guid riid,
            bool fOwn,
            out object ppvObj);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        private static class PICTYPE
        {
            public const short PICTYPE_BITMAP = 1;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PICTDESC
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct Bitmap
            {
                public int cbSizeOfStruct;
                public int picType;
                public IntPtr hbitmap;
                public IntPtr hpal;
            }
        }

        #endregion
    }
}
