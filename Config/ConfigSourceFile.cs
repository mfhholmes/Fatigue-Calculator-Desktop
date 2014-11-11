using System;
using System.IO;
using System.Xml;

namespace Fatigue_Calculator_Desktop.Config
{
	/// <summary>
	/// handles the fetching and saving of the config XML from a file
	/// </summary>
	internal class ConfigSourceFile : IConfigSource
	{
		private string _filename = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"/Fatigue Calculator/config.xml";

		/// <summary>
		/// fetches the xml file from the appdata directory and returns the xml document
		/// </summary>
		/// <returns>the xml document from the file</returns>
		System.Xml.XmlDocument IConfigSource.getConfigXML()
		{
			//for this iteration the filename is fixed as 'config.xml' and stored in appdata

			XmlDocument result = new XmlDocument();
			// check it's there
			FileInfo test = new FileInfo(_filename);
			if (!test.Exists)
			{
				//nope, create it
				result = createConfigFile();
			}
			else
			{
				try
				{
					result.Load(_filename);
				}
				catch
				{
					// can't load the config file
					// gotta create a new blank config file then
					result = createConfigFile();
				}
			}
			return result;
		}

		bool IConfigSource.saveConfigXML(XmlDocument newVersion)
		{
			// save the document to the file...easy
			try
			{
				// check we've got a valid directory
				FileInfo file = new FileInfo(_filename);
				createDir(file.Directory);
				DirectoryInfo dir = file.Directory;
				if (dir.Exists)
					newVersion.Save(_filename);
				else
					dir.Create();
			}
			catch
			{
				return false;
			}
			return true;
		}

		private void createDir(DirectoryInfo dir)
		{
			if (!dir.Exists)
			{
				createDir(dir.Parent);
				dir.Create();
			}
		}

		private XmlDocument createConfigFile()
		{
			XmlDocument result = new XmlDocument();
			return result;
		}

		string IConfigSource.URL
		{
			get { return _filename; }
		}
	}
}