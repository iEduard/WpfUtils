using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace CopyAndCompare
{
    public static class Version
    {
        /// <summary>
        /// get the Version of the DLL
        /// </summary>
        /// <returns>Return value fo the version</returns>
        public static string GetVersion()
        {
            string _versionNumber = "";

            // Get the Version of the Assembly
            _versionNumber = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            return _versionNumber;
        }


        /// <summary>
        /// Get the History of the DLL
        /// </summary>
        /// <returns>History text of the lib</returns>
        public static string GetHistory()
        {
            StringBuilder _history  = new StringBuilder();

            try
            {
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CopyAndCompare.History.txt"))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    _history.Append(result);
                }
            }
            catch (Exception err)
            {
                _history = new StringBuilder();
            }

            return _history.ToString();
        }



    }
}
