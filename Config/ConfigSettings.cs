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
        private static FatigueCalcConfig _settings;

        public static IConfigSource source
        {
            get 
            {
                if (_source == null)
                    source = new ConfigSourceFile();
                return _source; 
            }
            set {
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
            if (_settingsXML.InnerXml.Length == 0)
            {
                // can't populate from the xml because it's blank, so we need to create the default XML and then save that
                if (createDefaultConfig())
                    saveSettings();
            }
            else
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

        private static bool createDefaultConfig()
        {
            _settings = new FatigueCalcConfig();
            configItem setting;

            //low threshold
            setting = new configItem();
            setting.validationXML = "<validation><type>int</type><min>1</min><max>24</max></validation>";
            setting.key = "lowThreshold";
            setting.name = "Low Threshold";
            setting.description = "The calculation score that separates Moderate fatigue risk from High fatigue risk";
            setting.intValue = 6;
            _settings.Add(setting);

            //high threshold
            setting = new configItem();
            setting.validationXML = "<validation><type>int</type><min>2</min><max>24</max></validation>";
            setting.key = "highThreshold";
            setting.name = "High Threshold";
            setting.description = "The calculation score that separates High fatigue risk from Extreme fatigue risk";
            setting.intValue = 12;
            _settings.Add(setting);

            //ID Lookup File
            setting = new configItem();
            setting.validationXML = "<validation><type>file</type><mustExist>no</mustExist></validation>";
            setting.key = "IDLookupFile";
            setting.name = "Identity Lookup File";
            setting.description = "The file containing the list of valid identities for this calculator";
            setting.strValue = "%appdata%/fatigue calculator/Identities.csv";
            _settings.Add(setting);

            //ID Lookup Type
            setting = new configItem();
            setting.validationXML = "<validation><type>choice</type><fixedValueSet><value>none</value><value>fixed</value><value>open</value><value>closed</value></fixedValueSet></validation>";
            setting.key = "IDLookupType";
            setting.name = "Identity Lookup Type";
            setting.description = "Four possible values: None (anything goes), Fixed (uses the FixedId setting), Open(new names added to the store), Closed(only names in the store)";
            setting.strValue = "Closed";
            _settings.Add(setting);

            //log service URL
            setting = new configItem();
            setting.validationXML = "<validation><type>file</type><mustExist>no</mustExist></validation>";
            setting.key = "logServiceUrl";
            setting.name = "Calculation Logging File";
            setting.description = "The file containing the log of calculations that this calculator has performed";
            setting.strValue = "%documents%fatiguelog.csv";
            _settings.Add(setting);

            //device ID
            setting = new configItem();
            setting.validationXML = "<validation><type>string</type><maxLength>25</maxLength><minLength>1</minLength><HTMLencode>no</HTMLencode></validation>";
            setting.key = "deviceId";
            setting.name = "Device Name";
            setting.description = "The name of the PC, kiosk or device that the calculator is being run on";
            setting.strValue = "TestDevice";
            _settings.Add(setting);

            //fixed ID
            setting = new configItem();
            setting.validationXML = "<validation><type>string</type><maxLength>25</maxLength><minLength>1</minLength><HTMLencode>no</HTMLencode></validation>";
            setting.key = "fixedId";
            setting.name = "Fixed Identity";
            setting.description = "If the ID Lookup Type is Fixed, this is the identity used for all calculations.";
            setting.strValue = "SingleUser";
            _settings.Add(setting);

            //research page
            setting = new configItem();
            setting.validationXML = "<validation><type>choice</type><fixedValueSet><value>shown</value><value>not shown</value></fixedValueSet></validation>";
            setting.key = "researchPage";
            setting.name = "Show Research Page";
            setting.description = "Determines if the Research Page asking the user for their consent for research data is shown or not. Values can be 'shown' or 'not shown'";
            return true;
        }

        public static bool saveSettings()
        {
            XmlDocument newParent = new XmlDocument();
            XmlElement config = newParent.CreateElement("config");
            newParent.AppendChild(config);

            XmlElement itemXML;
            foreach(KeyValuePair<string,configItem> itemPair in _settings.itemList())
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
