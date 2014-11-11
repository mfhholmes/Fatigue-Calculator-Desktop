using System.Collections.Generic;
using System.Xml;

namespace Fatigue_Calculator_Desktop.Config
{
	internal static class ConfigSettings
	{
		private static XmlDocument _settingsXML = null;
		private static IConfigSource _source = null;
		private static FatigueCalcConfig _settings;

		public static IConfigSource source
		{
			get
			{
				if (_source == null)
					source = new ConfigSourceFile();
				return _source;
			}
			set
			{
				_source = value;
				_settingsXML = source.getConfigXML();
			}
		}

		public static FatigueCalcConfig settings
		{
			get
			{
				if (_settings == null)
					_settings = loadSettings();
				return _settings;
			}
		}

		private static FatigueCalcConfig loadSettings()
		{
			_settings = new FatigueCalcConfig();
			// have we got an XML?
			if (_settingsXML == null)
				_settingsXML = source.getConfigXML();

			//TODO: check if the XML has a redirection to another source
			if (_settingsXML.InnerXml.Length > 0)
			{
				foreach (XmlElement settingsNode in _settingsXML.DocumentElement.SelectNodes("//setting"))
				{
					// create config items
					configItem newItem = new configItem();
					if (newItem.parseXML(settingsNode))
						_settings.Add(newItem);
				}
			}
			return _settings;
		}

		public static bool saveSettings()
		{
			XmlDocument newParent = new XmlDocument();
			XmlElement config = newParent.CreateElement("config");
			newParent.AppendChild(config);

			XmlElement itemXML;
			foreach (KeyValuePair<string, configItem> itemPair in _settings.itemList())
			{
				itemXML = itemPair.Value.writeXML(newParent);
				config.AppendChild(itemXML);
			}

			if (_source != null)
				return _source.saveConfigXML(newParent);
			else
				return false;
		}
	}
}