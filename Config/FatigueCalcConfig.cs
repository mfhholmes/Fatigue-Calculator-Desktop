using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fatigue_Calculator_Desktop.Config
{
    class FatigueCalcConfig
    {

        private Dictionary<string,configItem> _itemList = new Dictionary<string,configItem>();
        public configItem item(string key)
        {
            configItem result = null;
            if(_itemList.TryGetValue(key, out result))
                return result;    
            return new configItem();
        }
        public Dictionary<string,configItem> itemList()
        {
            return _itemList;
        }
        private configItem _lowThreshold;
        public int lowThreshold
        {
            get { return (int)_lowThreshold.intValue; }
            set { _lowThreshold.intValue = value; }
        }
        private configItem _highThreshold;
        public int highThreshold
        {
            get { return _highThreshold.intValue;}
            set { _highThreshold.intValue = value; }
        }
        private configItem _IDLookupFile;
        public string IDLookupFile
        {
            get { return _IDLookupFile.strValue; }
            set { _IDLookupFile.strValue = value; }
        }
        private configItem _IDLookupType;
        public string IDLookupType
        {
            get{return _IDLookupType.strValue;}
            set{_IDLookupType.strValue = value;}
        }
        private configItem _deviceId;
        public string deviceId
        {
            get{return _deviceId.strValue;}
            set{_deviceId.strValue = value;}
        }
        private configItem _fixedId;
        public string fixedId
        {
            get { return _fixedId.strValue; }
            set { _fixedId.strValue = value; }
        }
        private configItem _logServiceURL;
        public string logServiceUrl
        {
            get { return _logServiceURL.strValue; }
            set { _logServiceURL.strValue = value; }
        }
        private configItem _researchPage;
        public string researchPage
        {
            get { return _researchPage.strValue; }
            set { _researchPage.strValue = value; }
        }
        /// <summary>
        /// adds a configItem to the config settings
        /// </summary>
        /// <param name="newItem">the configItem to add</param>
        public void Add(configItem newItem)
        {
            // work out which config item this is
            switch (newItem.key.ToLower())
            {
                case "lowthreshold":
                    {
                        _lowThreshold = newItem;
                        break;
                    }
                case "highthreshold":
                    {
                        _highThreshold = newItem;
                        break;
                    }
                case "idlookupfile":
                    {
                        _IDLookupFile = newItem;
                        break;
                    }
                case "idlookuptype":
                    {
                        _IDLookupType = newItem;
                        break;
                    }
                case "deviceid":
                    {
                        _deviceId = newItem;
                        break;
                    }
                case "fixedid":
                    {
                        _fixedId = newItem;
                        break;
                    }
                case "logserviceurl":
                    {
                        _logServiceURL = newItem;
                        break;
                    }
                case "researchpage":
                    {
                        _researchPage = newItem;
                        break;
                    }
                default:
                    {
                        //throw new Exception("unknown config handle");
                        // just add it to the list but don't assign a handle
                        break;
                    }
            }
            _itemList.Add(newItem.key, newItem);
            return;
        }
    }
}
