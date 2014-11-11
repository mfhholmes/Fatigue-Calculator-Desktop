using System;

using System.Collections.Generic;
using System.Xml;

namespace Fatigue_Calculator_Desktop.Config
{
	public class configItem
	{
		private IValidator _validator;

		public IValidator validator
		{
			get { return _validator; }
		}

		private bool _changed = false;

		public bool changed
		{
			get { return _changed; }
		}

		private string _key = "";

		public string key
		{
			get { return _key; }
			set { _key = value; _changed = true; }
		}

		private string _name = "";

		public string name
		{
			get { return _name; }
			set { _name = value; _changed = true; }
		}

		private string _description = "";

		public string description
		{
			get { return _description; }
			set { _description = value; _changed = true; }
		}

		private string _rawType = "";
		private System.Type _type;

		public System.Type type
		{
			get { return _type; }
			set { _type = value; _rawType = typeString(_type); }
		}

		private string _rawValue = "";

		public int intValue
		{
			get
			{
				int _intvalue;
				if (int.TryParse(_rawValue, out _intvalue))
					return _intvalue;
				else
					return 0;
			}
			set { changeValue(value.ToString()); }
		}

		public double dblValue
		{
			get
			{
				double _dblvalue;
				if (double.TryParse(_rawValue, out _dblvalue))
					return _dblvalue;
				else
					return 0;
			}
			set { changeValue(value.ToString()); }
		}

		public string strValue
		{
			get { return _rawValue; }
			set { changeValue(value); }
		}

		public DateTime dtValue
		{
			get
			{
				DateTime _dtvalue;
				if (DateTime.TryParse(_rawValue, out _dtvalue))
					return _dtvalue;
				else
					return default(DateTime);
			}
			set { changeValue(value.ToString()); }
		}

		private string _lastValidationError;

		public string lastValidationError
		{
			get { return _lastValidationError; }
		}

		private string _validationXML;

		public string validationXML
		{
			get { return _validationXML; }
			set { _validationXML = value; setUpValidation(); _changed = true; }
		}

		private string typeString(System.Type type)
		{
			// gonna try this and see if it works...it may not
			return type.ToString();
		}

		private void changeValue(string value)
		{
			string oldvalue = _rawValue;
			_rawValue = value;
			_changed = true;
			if (!validate())
			{
				_rawValue = oldvalue;
				_changed = false;
				throw new Exception(_lastValidationError);
			}
			return;
		}

		private bool validate()
		{
			if (validator == null)
				setUpValidation();
			if (validator.validate(this))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// parses an XML element to create the config setting values
		/// </summary>
		/// <param name="settingsNode">the XML element to parse</param>
		/// <returns>true if the parse was successful</returns>
		public bool parseXML(XmlElement settingsNode)
		{
			try
			{
				XmlNode node;
				node = settingsNode.GetAttributeNode("key");
				if (node == null)
					return false;
				_key = node.Value;
				node = settingsNode.SelectSingleNode("name");
				if (node == null)
					return false;
				_name = node.InnerText;
				node = settingsNode.SelectSingleNode("value");
				if (node == null)
					return false;
				_rawValue = node.InnerText;
				node = settingsNode.SelectSingleNode("description");
				if (node == null)
					return false;
				_description = node.InnerText;
				// now work out the actual type
				XmlNode validationNode = settingsNode.SelectSingleNode("validation");
				if (validationNode == null)
					return false;
				_validationXML = validationNode.OuterXml;
				setUpValidation();
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// translates the validationXML into the appropriate validator object
		/// </summary>
		private void setUpValidation()
		{
			XmlDocument validationXML = new XmlDocument();
			validationXML.LoadXml(_validationXML);

			XmlNode node;
			node = validationXML.SelectSingleNode("validation/type");
			if (node == null)
				return;
			_rawType = node.InnerText;
			XmlNode validationNode = validationXML.SelectSingleNode("validation");

			switch (_rawType.ToLower())
			{
				case "int":
					{
						_type = typeof(int);
						_validator = new intValidator();
						_validator.setup(validationNode);
						break;
					}
				case "file":
					{
						_type = typeof(string);
						_validator = new fileValidator();
						_validator.setup(validationNode);
						break;
					}
				case "choice":
					{
						_type = typeof(string);
						_validator = new choiceValidator();
						_validator.setup(validationNode);
						break;
					}
				case "string":
					{
						_type = typeof(string);
						_validator = new stringValidator();
						_validator.setup(validationNode);
						break;
					}
				case "datetime":
					{
						_type = typeof(string);
						_validator = new dateTimeValidator();
						_validator.setup(validationNode);
						break;
					}
				case "url":
					{
						_type = typeof(string);
						_validator = new URLValidator();
						_validator.setup(validationNode);
						break;
					}
				default:
					{
						return;
					}
			}
		}

		/// <summary>
		/// creates the XML representation of this config setting
		/// </summary>
		/// <param name="parentDoc">the XML document context for this XML</param>
		/// <returns>the element created with the setting in it</returns>
		public XmlElement writeXML(XmlDocument parentDoc)
		{
			XmlElement result = parentDoc.CreateElement("setting");
			XmlElement node;
			//key
			XmlAttribute attr = parentDoc.CreateAttribute("key");
			attr.Value = _key;
			result.SetAttributeNode(attr);
			// name
			node = parentDoc.CreateElement("name");
			node.InnerText = _name;
			result.AppendChild(node);
			// description
			node = parentDoc.CreateElement("description");
			node.InnerText = _description;
			result.AppendChild(node);
			//value
			node = parentDoc.CreateElement("value");
			node.InnerText = _rawValue;
			result.AppendChild(node);
			//validationXML
			XmlDocumentFragment frag;
			frag = parentDoc.CreateDocumentFragment();
			frag.InnerXml = _validationXML;
			result.AppendChild(frag);
			return result;
		}

		#region "validator subclasses"

		private class intValidator : IValidator
		{
			private int _min = int.MinValue;
			private int _max = int.MaxValue;

			bool IValidator.setup(XmlNode validationNode)
			{
				// int validation has a min and a max
				XmlNode node = validationNode.SelectSingleNode("min");
				if (!int.TryParse(node.InnerText, out _min))
					_min = int.MinValue;
				node = validationNode.SelectSingleNode("max");
				if (!int.TryParse(node.InnerText, out _max))
					_max = int.MaxValue;
				return true;
			}

			bool IValidator.validate(configItem parent)
			{
				if (parent.intValue < _min)
				{
					parent._lastValidationError = "minimum value is " + _min.ToString();
					return false;
				}
				if (parent.intValue > _max)
				{
					parent._lastValidationError = "maximum value is " + _max.ToString();
					return false;
				}
				return true;
			}
		}

		private class stringValidator : IValidator
		{
			private int _minLength = 0;
			private int _maxLength = 1000;

			bool IValidator.setup(XmlNode validationNode)
			{
				//string validation has a minlength and a maxlength
				XmlNode node = validationNode.SelectSingleNode("minLength");
				if (!int.TryParse(node.InnerText, out _minLength))
					_minLength = 0;
				node = validationNode.SelectSingleNode("maxLength");
				if (!int.TryParse(node.InnerText, out _maxLength))
					_maxLength = 1024; // number plucked from thin air, but reasonable.

				return true;
			}

			bool IValidator.validate(configItem parent)
			{
				if (parent.strValue.Length < _minLength)
				{
					parent._lastValidationError = "minimum value length is " + _minLength.ToString() + " characters";
					return false;
				}
				if (parent.strValue.Length > _maxLength)
				{
					parent._lastValidationError = "maximum value length is " + _maxLength.ToString() + " characters";
					return false;
				}
				return true;
			}
		}

		private class choiceValidator : IValidator
		{
			private List<string> choices = new List<string>();

			bool IValidator.setup(XmlNode validationNode)
			{
				foreach (XmlNode node in validationNode.SelectNodes("fixedValueSet/value"))
				{
					choices.Add(node.InnerText.ToLower());
				}
				return true;
			}

			bool IValidator.validate(configItem parent)
			{
				foreach (string test in choices)
				{
					if (parent.strValue.ToLower() == test)
						return true;
				}
				parent._lastValidationError = "value is not one of the acceptable choices";
				return false;
			}
		}

		private class fileValidator : IValidator
		{
			private bool _mustExist = false;

			bool IValidator.setup(XmlNode validationNode)
			{
				XmlNode node = validationNode.SelectSingleNode("mustExist");
				string test = node.InnerText.ToLower();

				if (test.IndexOf("yes") > -1 || test.IndexOf("true") > -1)
					_mustExist = true;
				return true;
			}

			bool IValidator.validate(configItem parent)
			{
				// check if it's a valid path
				System.IO.FileInfo testFile;
				try
				{
					testFile = new System.IO.FileInfo(parent.strValue);
				}
				catch
				{
					// failed
					parent._lastValidationError = "value is not a valid file path";
					return false;
				}
				if (_mustExist)
					if (!testFile.Exists)
					{
						parent._lastValidationError = "file does not exist";
						return false;
					}
				return true;
			}
		}

		private class URLValidator : IValidator
		{
			private IValidator subTest = new fileValidator();

			bool IValidator.setup(XmlNode validationNode)
			{
				// we may need to test it by pinging in the future
				// but for now, nothing.
				// but setup the subtest
				subTest.setup(validationNode);
				return true;
			}

			bool IValidator.validate(configItem parent)
			{
				try
				{
					string url = parent.strValue;
					Uri testresult;
					bool result;
					result = Uri.TryCreate(url, UriKind.Absolute, out testresult);
					if (!result)
					{
						// possible it's an absolute path to a file instead of an internet URL
						// so test it with the file validator
						return subTest.validate(parent);
					}
					else
					{
						return true;
					}
				}
				catch
				{
					return false;
				}
			}
		}

		private class dateTimeValidator : IValidator
		{
			bool IValidator.setup(XmlNode validationNode)
			{
				// none specified yet
				return true;
			}

			bool IValidator.validate(configItem parent)
			{
				// no validation methods specified yet
				return true;
			}
		}

		#endregion "validator subclasses"
	}
}