using System.Collections.Generic;

namespace Fatigue_Calculator_Desktop.Config
{
	internal class FatigueCalcConfig
	{
		private Dictionary<string, configItem> _itemList = new Dictionary<string, configItem>();

		public configItem item(string key)
		{
			configItem result = null;
			if (_itemList.TryGetValue(key, out result))
				return result;
			return new configItem();
		}

		public Dictionary<string, configItem> itemList()
		{
			return _itemList;
		}

		public int lowThreshold
		{
			get
			{
				configItem setting;
				if (_itemList.TryGetValue("lowThreshold", out setting))
				{
					return setting.intValue;
				}
				// create and return the default
				setting = setDefaultLowThreshold();
				return setting.intValue;
			}
			set
			{
				configItem setting;
				if (_itemList.TryGetValue("lowThreshold", out setting))
					setting.intValue = value;
				else
				{
					setting = setDefaultLowThreshold();
					setting.intValue = value;
				}
			}
		}

		public int highThreshold
		{
			get
			{
				configItem setting;
				if (_itemList.TryGetValue("highThreshold", out setting))
				{
					return setting.intValue;
				}
				// create and return the default
				setting = setDefaultHighThreshold();
				return setting.intValue;
			}
			set
			{
				configItem setting;
				if (_itemList.TryGetValue("highThreshold", out setting))
					setting.intValue = value;
				else
				{
					setting = setDefaultHighThreshold();
					setting.intValue = value;
				}
			}
		}

		public string IDLookupFile
		{
			get
			{
				configItem setting;
				if (_itemList.TryGetValue("IDLookupFile", out setting))
				{
					return setting.strValue;
				}
				// create and return the default
				setting = setDefaultIDLookupFile();
				return setting.strValue;
			}
			set
			{
				configItem setting;
				if (_itemList.TryGetValue("IDLookupFile", out setting))
					setting.strValue = value;
				else
				{
					setting = setDefaultIDLookupFile();
					setting.strValue = value;
				}
			}
		}

		public string IDLookupType
		{
			get
			{
				configItem setting;
				if (_itemList.TryGetValue("IDLookupType", out setting))
				{
					return setting.strValue;
				}
				// create and return the default
				setting = setDefaultIDLookupType();
				return setting.strValue;
			}
			set
			{
				configItem setting;
				if (_itemList.TryGetValue("IDLookupType", out setting))
					setting.strValue = value;
				else
				{
					setting = setDefaultIDLookupType();
					setting.strValue = value;
				}
			}
		}

		public string deviceId
		{
			get
			{
				configItem setting;
				if (_itemList.TryGetValue("deviceId", out setting))
				{
					return setting.strValue;
				}
				// create and return the default
				setting = setDefaultDeviceId();
				return setting.strValue;
			}
			set
			{
				configItem setting;
				if (_itemList.TryGetValue("deviceId", out setting))
					setting.strValue = value;
				else
				{
					setting = setDefaultDeviceId();
					setting.strValue = value;
				}
			}
		}

		public string fixedId
		{
			get
			{
				configItem setting;
				if (_itemList.TryGetValue("fixedId", out setting))
				{
					return setting.strValue;
				}
				// create and return the default
				setting = setDefaultFixedId();
				return setting.strValue;
			}
			set
			{
				configItem setting;
				if (_itemList.TryGetValue("fixedId", out setting))
					setting.strValue = value;
				else
				{
					setting = setDefaultFixedId();
					setting.strValue = value;
				}
			}
		}

		public string logServiceUrl
		{
			get
			{
				configItem setting;
				if (_itemList.TryGetValue("logServiceUrl", out setting))
				{
					return setting.strValue;
				}
				// create and return the default
				setting = setDefaultLogServiceUrl();
				return setting.strValue;
			}
			set
			{
				configItem setting;
				if (_itemList.TryGetValue("logServiceUrl", out setting))
					setting.strValue = value;
				else
				{
					setting = setDefaultLogServiceUrl();
					setting.strValue = value;
				}
			}
		}

		public string researchPage
		{
			get
			{
				configItem setting;
				if (_itemList.TryGetValue("researchPage", out setting))
				{
					return setting.strValue;
				}
				// create and return the default
				setting = setDefaultResearchPage();
				return setting.strValue;
			}
			set
			{
				configItem setting;
				if (_itemList.TryGetValue("researchPage", out setting))
					setting.strValue = value;
				else
				{
					setting = setDefaultResearchPage();
					setting.strValue = value;
				}
			}
		}

		/// <summary>
		/// adds a configItem to the config settings
		/// </summary>
		/// <param name="newItem">the configItem to add</param>
		public void Add(configItem newItem)
		{
			// check if we've already got an item with this name
			configItem existing;
			if (_itemList.TryGetValue(newItem.key, out existing))
			{
				// remove the existing item so we can add the new one
				_itemList.Remove(existing.key);
			}
			// add the new one
			_itemList.Add(newItem.key, newItem);
		}

		private configItem setDefaultLowThreshold()
		{
			//low threshold
			configItem setting = new configItem();
			setting.validationXML = "<validation><type>int</type><min>1</min><max>24</max></validation>";
			setting.key = "lowThreshold";
			setting.name = "Low Threshold";
			setting.description = "The calculation score that separates Moderate fatigue risk from High fatigue risk";
			setting.intValue = 6;
			_itemList.Add(setting.key, setting);
			return setting;
		}

		private configItem setDefaultHighThreshold()
		{
			//high threshold
			configItem setting = new configItem();
			setting.validationXML = "<validation><type>int</type><min>2</min><max>24</max></validation>";
			setting.key = "highThreshold";
			setting.name = "High Threshold";
			setting.description = "The calculation score that separates High fatigue risk from Extreme fatigue risk";
			setting.intValue = 12;
			_itemList.Add(setting.key, setting);
			return setting;
		}

		private configItem setDefaultIDLookupFile()
		{
			//ID Lookup File
			configItem setting = new configItem();
			setting.validationXML = "<validation><type>file</type><mustExist>no</mustExist></validation>";
			setting.key = "IDLookupFile";
			setting.name = "Identity Lookup File";
			setting.description = "The file containing the list of valid identities for this calculator";
			setting.strValue = "%appdata%/fatigue calculator/Identities.csv";
			_itemList.Add(setting.key, setting);
			return setting;
		}

		private configItem setDefaultIDLookupType()
		{
			//ID Lookup Type
			configItem setting = new configItem();
			setting.validationXML = "<validation><type>choice</type><fixedValueSet><value>none</value><value>fixed</value><value>open</value><value>closed</value></fixedValueSet></validation>";
			setting.key = "IDLookupType";
			setting.name = "Identity Lookup Type";
			setting.description = "Four possible values: None (anything goes), Fixed (uses the FixedId setting), Open(new names added to the store), Closed(only names in the store)";
			setting.strValue = "Closed";
			_itemList.Add(setting.key, setting);
			return setting;
		}

		private configItem setDefaultLogServiceUrl()
		{
			//log service URL
			configItem setting = new configItem();
			setting.validationXML = "<validation><type>url</type><mustExist>no</mustExist></validation>";
			setting.key = "logServiceUrl";
			setting.name = "Calculation Logging File";
			setting.description = "The file containing the log of calculations that this calculator has performed";
			setting.strValue = "%documents%fatiguelog.csv";
			_itemList.Add(setting.key, setting);
			return setting;
		}

		private configItem setDefaultDeviceId()
		{
			//device ID
			configItem setting = new configItem();
			setting.validationXML = "<validation><type>string</type><maxLength>25</maxLength><minLength>1</minLength><HTMLencode>no</HTMLencode></validation>";
			setting.key = "deviceId";
			setting.name = "Device Name";
			setting.description = "The name of the PC, kiosk or device that the calculator is being run on";
			setting.strValue = "TestDevice";
			_itemList.Add(setting.key, setting);
			return setting;
		}

		private configItem setDefaultFixedId()
		{
			//fixed ID
			configItem setting = new configItem();
			setting.validationXML = "<validation><type>string</type><maxLength>25</maxLength><minLength>1</minLength><HTMLencode>no</HTMLencode></validation>";
			setting.key = "fixedId";
			setting.name = "Fixed Identity";
			setting.description = "If the ID Lookup Type is Fixed, this is the identity used for all calculations.";
			setting.strValue = "SingleUser";
			_itemList.Add(setting.key, setting);
			return setting;
		}

		private configItem setDefaultResearchPage()
		{
			//research page
			configItem setting = new configItem();
			setting.validationXML = "<validation><type>choice</type><fixedValueSet><value>shown</value><value>not shown</value></fixedValueSet></validation>";
			setting.key = "researchPage";
			setting.name = "Show Research Page";
			setting.description = "Determines if the Research Page asking the user for their consent for research data is shown or not. Values can be 'shown' or 'not shown'";
			setting.strValue = "shown";
			_itemList.Add(setting.key, setting);
			return setting;
		}
	}
}