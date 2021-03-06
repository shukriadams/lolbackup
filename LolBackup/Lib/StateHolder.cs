//////////////////////////////////////////////////////////////////////////////////////
// Author				: Shukri Adams												//
// Contact				: shukri.adams@gmail.com									//
//																					//
// vcFramework : A reuseable library of utility classes                             //
// Copyright (C)																	//
//////////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Xml;
using vcFramework.Serialization;

namespace vcFramework.Xml
{
	/// <summary>  
	/// Stores and retrieves values in an xml file.
	/// </summary>
	public class StateHolder
	{
		#region MEMBERS
		 
		/// <summary> 
		/// Xml document which this class stores its data in 
		/// </summary>
		private XmlDocument _dataHolder;

		/// <summary> 
		/// Local drive path for xml document - must be passed in as construcotr argument 
		/// </summary>
		private string _documentPath;
        
        #endregion

		#region PROPERTIES

        /// <summary>
        /// If true, state data will be saved to file each time state is changed. This 
        /// can be expensive if there is a lot of state data, but removes the need to 
        /// explicityl save state data from the client code
        /// </summary>
		public bool SaveOnTheFly { get;set;}

		#endregion

		#region CONSTRUCTORS

		/// <summary> 
		/// Default Constructor 
		/// </summary>
        /// <param name="documentPath"></param>
		public StateHolder(string documentPath)
		{
            _documentPath = documentPath;
			
			// sets up xml document on which this object is based
			CreateXmlBase();
			
			// sets default
			this.SaveOnTheFly = true;
		}

		#endregion

		#region METHODS
		
		/// <summary>
		/// loads or creates Xmldocument in which state data is stored
		/// </summary>
		private void CreateXmlBase()
		{

            _dataHolder = new XmlDocument();

            if (File.Exists(_documentPath))
            {
                _dataHolder.Load(_documentPath);
            }
            else
            {
                _dataHolder.InnerXml = "<!-- This is an autogenerated file - do not modify it --><StateItems/>";
                _dataHolder.Save(_documentPath);
            }
		}


		/// <summary> 
		/// Stores an xml node udner the given name 
		/// </summary>
        /// <param name="key"></param>
        /// <param name="item"></param>
		public void Add(string key, object item)
		{
            if (_dataHolder == null || _dataHolder.DocumentElement == null)
                throw new Exception("State document is null or has invalid content.");

			// ###########################################
			// removes item at named position if that 
			// item already exists
			// -------------------------------------------
            Remove(key);
			

			// ###########################################
			// adds new item
			// -------------------------------------------
			string serialized = SerializeLib.SerializeToXmlString(item);

            XmlDocumentFragment fragment = _dataHolder.CreateDocumentFragment();
            fragment.InnerXml = string.Format("<StateItem UniqueNameKey='{0}'>{1}</StateItem>", key, serialized);

            _dataHolder.DocumentElement.AppendChild(fragment);
			
			if (SaveOnTheFly)
				this.Save();

		}
		
		
		/// <summary> 
		/// Retrieves an xml node stored under the given name 
		/// </summary>
        /// <param name="key"></param>
		/// <returns></returns>
		public object Retrieve(string key)
		{
            if (_dataHolder == null || _dataHolder.DocumentElement == null)
                throw new Exception("State document is null or has invalid content.");

			XmlNode node = null;

            string path = string.Format(".//StateItem [@UniqueNameKey='{0}']", key);
            if (_dataHolder.DocumentElement.SelectSingleNode(path) != null)
                node = _dataHolder.DocumentElement.SelectSingleNode(path).FirstChild;

            return SerializeLib.DeserializeFromXmlString(node.OuterXml);

		}
		

		/// <summary>
		/// Removes the given item if it exists. Returns true if the item was removed
		/// </summary>
        /// <param name="key"></param>
		public bool Remove(string key)
		{
            if (_dataHolder == null || _dataHolder.DocumentElement == null)
                throw new Exception("State document is null or has invalid content.");

            XmlNode node = _dataHolder.DocumentElement.SelectSingleNode(string.Format(".//StateItem [@UniqueNameKey='{0}']", key));
            if (node == null)
				return false;

            _dataHolder.DocumentElement.RemoveChild(node);			
			return true;
		}


		/// <summary> 
		/// returns true if underlying xml storage document contains the item with the given name.  
		/// </summary>
        /// <param name="key"></param>
		/// <returns></returns>
		public bool Contains(string key)
		{
            if (_dataHolder == null || _dataHolder.DocumentElement == null)
                throw new Exception("State document is null or has invalid content.");

            if (_dataHolder.DocumentElement.SelectSingleNode(string.Format(".//StateItem [@UniqueNameKey='{0}']", key)) != null)
				return true;
			return false;
		}


		/// <summary> 
		/// Saves object state to disk 
		/// </summary>
		public void Save()
		{
            _dataHolder.Save(_documentPath);
		}
		

		/// <summary> 
		/// Forceabily recreates the Xml base document for this objec
		/// </summary>
		public void ForceReload()
		{
			CreateXmlBase();
		}


		/// <summary>
		/// Deletes all state data
		/// </summary>
		public void Clear()
		{
            if (_dataHolder == null || _dataHolder.DocumentElement == null)
                throw new Exception("State document is null or has invalid content.");

            while (_dataHolder.DocumentElement.ChildNodes.Count > 0)
                _dataHolder.DocumentElement.RemoveChild(
                    _dataHolder.DocumentElement.ChildNodes[0]);

			if (SaveOnTheFly)
				this.Save();
		}

		#endregion
	}
}
