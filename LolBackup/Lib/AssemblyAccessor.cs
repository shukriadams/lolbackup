using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace LolBackup
{
    public class AssemblyAccessor
    {

        #region FIELDS

        readonly Assembly _assembly;

        #endregion

        #region CTORS

        public AssemblyAccessor(Type type)
        {

            _assembly = Assembly.GetAssembly(type);
        }

        #endregion

        #region METHODS

        /// <summary> 
        /// Returns a string with the convents  of the given path 
        /// </summary>
        /// <param name="assemblyPath"></param>
        /// <returns></returns>
        public string GetStringDocument(string assemblyPath)
        {
            Stream textStream = null;

            try
            {

                textStream = _assembly.GetManifestResourceStream(assemblyPath);

                if (textStream != null)
                {
                    StreamReader reader = new StreamReader(textStream, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
                else
                    throw new Exception(string.Format("Unable to find the resource for the path '{0}'.", assemblyPath));

            }
            finally
            {
                if (textStream != null)
                {
                    textStream.Flush();
                    textStream.Close();
                }
            }
        }

    #endregion

    }
}
