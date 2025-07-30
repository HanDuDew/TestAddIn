using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace InvAddIn.Utils
{
    public class CommonUtils
    {
        /// <summary>
        /// Gets the current domain user
        /// </summary>
        public static string GetDomainUser()
        {
            //get current user name
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            string userName = identity.Name;
            if (!string.IsNullOrEmpty(userName) && userName.Contains("\\"))
            {
                userName = userName.Substring(userName.IndexOf('\\') + 1);
            }
            return userName;
        }

        /// <summary>
        /// Gets the current machine name
        /// </summary>
        public static string GetMachineName()
        {
            //get current machine name
            string machineName = Environment.MachineName;
            return machineName;
        }
    }
}
