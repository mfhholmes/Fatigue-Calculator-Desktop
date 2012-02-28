using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Fatigue_Calculator_Desktop.Config
{
    static class ConfigSettings
    {
        private static XmlDocument _settingsXML = null;
        private static IConfigSource _source = null;
        private static FatigueCalcConfig _settings = loadSettings();

        public static IConfigSource source
        {
            get { return _source; }
            set {
                _source = value;
                _settingsXML = source.getConfigXML();
            }
        }

        public static FatigueCalcConfig settings
        {
            get
            { return _settings; }
        }
        private static FatigueCalcConfig loadSettings()
        {
            //have we got a source?
            // if not grab the default source file
            if (_source == null)
                source = new ConfigSourceFile();
            // have we got an XML?
            if (_settingsXML == null)
                _settingsXML = source.getConfigXML();
            // set the config settings
            if (_settings == null)
                _settings = new FatigueCalcConfig();
            

            //TODO: check if the XML has a redirection to another source
            
            // iterate the XML
            foreach (XmlElement settingsNode in _settingsXML.DocumentElement.SelectNodes("//setting"))
            {
                // create config items
                configItem newItem = new configItem();
                if(newItem.parseXML(settingsNode))
                    _settings.Add(newItem);
                
            }
            return _settings;
        }
    }
}
