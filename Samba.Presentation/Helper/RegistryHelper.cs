using System;


    public static class RegistryHelper
    {
        static string Regname = "Senlite";
        public static bool writeRegistryKey(string Key, string value)
        {
            try
            {
                Microsoft.Win32.RegistryKey key;

                key = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(Regname);
                key.SetValue(Key, value);
                key.Close();
                return true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public static string readRegistryKey(string Value)
        {
            try
            {

                string keyValue = null;
                Microsoft.Win32.RegistryKey key;
                key = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(Regname);
                keyValue = key.GetValue(Value).ToString();
                key.Close();
                return keyValue;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
    }
