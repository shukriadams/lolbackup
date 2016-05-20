//////////////////////////////////////////////////////////////////////////////////////
// Author				: Shukri Adams												//
// Contact				: shukri.adams@gmail.com									//
//																					//
// vcFramework : A reuseable library of utility classes                             //
// Copyright (C)																	//
//////////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Xml;
using vcFramework.Parsers;

namespace vcFramework.Assemblies
{

    /// <summary> 
    /// Class for accessing items stored in the assembly 
    /// </summary>
    public class AssemblyAccessor
    {

        #region FIELDS

        Assembly _assembly;

        #endregion


        #region CONSTRUCTORS


        /// <summary>
        /// Constructor which finds uses the assembly which the given type is defined in. 
        /// </summary>
        /// <param name="type">The type, the assembly of which, this object will access</param>
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
