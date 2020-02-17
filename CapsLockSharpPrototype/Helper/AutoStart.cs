using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapsLockSharpPrototype.Helper
{
    public class AutoStart
    {
        public static bool CheckEnabled(string appName)
        {
            var key = GetRegistryKey();
            var ret = key.GetValue(appName);
            key.Close();
            return ret != null;
        }

        private static RegistryKey GetRegistryKey()
        {
            var subkey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
            RegistryKey key = Registry.CurrentUser.OpenSubKey(subkey, true);//打开注册表项
            if (key == null)
            {
                key = Registry.CurrentUser.CreateSubKey(subkey);
            }
            return key;
        }

        public static void Enable(string appName, string path)
        {
            var key = GetRegistryKey();
            key.SetValue(appName, $"\"{path}\"");
            key.Close();
        }
        public static void Disable(string appName)
        {
            var key = GetRegistryKey();
            key.DeleteValue(appName);
            key.Close();
        }
    }
}
