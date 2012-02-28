using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private bool changed = false;
        private string _key = "";
        public string key
        {
            get { return _key; }
            set { _key = value; changed = true; }
        }
        private string _name = "";
        public string name
        {
            get { return _name; }
            set { _name = value; changed = true; }
        }
        private string _description = "";
        public string description
        {
            get { return _description; }
            set { _description = value; changed = true; }
        }
        private string _rawType = "";
        private System.Type _type;
        public System.Type type
        {
            get { return _type; }
            set { _type = value; _rawType = typeString(_type); }
        }
        private string _rawValue = "";

        private int _intValue = 0;
        public int intValue
        {
            get { return _intValue ; }
            set { changeValue(value.ToString()); }
        }
        private double _dblValue = 0;
        public double dblValue
        {
            get { return _dblValue; }
            set { changeValue(value.ToString());  }
        }
        private string _strValue = "";
        public string strValue
        {
            get { return _strValue; }
            set { changeValue(value); }
        }
        private DateTime _dtValue = default(DateTime);
        public DateTime dtValue
        {
            get { return _dtValue; }
            set { changeValue(value.ToString()); }
        }
        private string _lastValidationError;
        public string lastValidationError
        {
            get { return _lastValidationError; }
        }

        private string _validationXML;

        private string typeString(System.Type type)
        {
            // gonna try this and see if it works...it may not
            return type.ToString();
        }
        private void changeValue(string value)
        {
            validate();
            changed = true;
            // set the values from the rawvalue
            int.TryParse(_rawValue, out  _intValue);
            DateTime.TryParse(_rawValue, out _dtValue);
            double.TryParse(_rawValue, out _dblValue);
            _strValue = _rawValue;
            return;
        }
        private void validate()
        {
            //TODO: set any validation errors and roll back any changes
            if (validator.validate(this))
            {
                //yay
            }
            else
            {
                // boo
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
                _validationXML = validationNode.InnerXml;
                node = validationNode.SelectSingleNode("type");
                if (node == null)
                    return false;
                _rawType = node.InnerText;
                
                
                switch (_rawType.ToLower())
                {
                    case "int":
                        {
                            _type = typeof(int);
                            _validator = new intValidator();
                            _validator.setup(validationNode);
                            validate();
                            break;
                        }
                    case "file":
                        {
                            _type = typeof(string);
                            _validator = new fileValidator();
                            _validator.setup(validationNode);
                            validate();
                            break;
                        }
                    case "choice":
                        {
                            _type = typeof(string);
                            _validator = new choiceValidator();
                            _validator.setup(validationNode);
                            validate();
                            break;
                        }
                    case "string":
                        {
                            {
                                _type = typeof(string);
                                _validator = new stringValidator();
                                _validator.setup(validationNode);
                                validate();
                                break;
                            }
                        }
                    case "datetime":
                        {
                            {
                                _type = typeof(string);
                                _validator = new dateTimeValidator();
                                _validator.setup(validationNode);
                                validate();
                                break;
                            }

                        }
                    default:
                        {
                            return false;
                        }
                }
            }
            catch (Exception err)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// creates the XML representation of this config setting
        /// </summary>
        /// <param name="parentDoc">the XML document context for this XML</param>
        /// <returns>the element created with the setting in it</returns>
        public XmlElement writeXML(XmlDocument parentDoc)
        {
            XmlElement result = parentDoc.CreateElement("setting");
            parentDoc.SelectSingleNode("config").AppendChild(result);
            //key
            XmlNode node = parentDoc.CreateAttribute("key");
            node.Value = _key;
            result.AppendChild(node);
            // name
            node = parentDoc.CreateElement("name");
            node.Value = _name;
            result.AppendChild(node);
            // description
            node = parentDoc.CreateElement("description");
            node.Value = _description;
            result.AppendChild(node);
            //value
            node = parentDoc.CreateElement("value");
            node.Value = _rawValue;
            result.AppendChild(node);
            //validationXML
            node = parentDoc.CreateElement("validationXML");
            node.InnerXml = _validationXML;
            result.AppendChild(node);
            return result;
        }

        #region "validator subclasses"

        
        private class intValidator : IValidator
        {
            int _min = int.MinValue;
            int _max = int.MaxValue;
            bool IValidator.setup(XmlNode validationNode)
            {
                // int validation has a min and a max
                // TODO: do some checking that the bloody rules parsed
                XmlNode node = validationNode.SelectSingleNode("min");
                int.TryParse(node.Value, out _min);
                node = validationNode.SelectSingleNode("max");
                int.TryParse(node.Value, out _max);
                
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
            int _minLength = 0;
            int _maxLength = 1000;
            bool IValidator.setup(XmlNode validationNode)
            {
                //string validation has a minlength and a maxlength
                // TODO: do some checking that the bloody rules parsed
                XmlNode node = validationNode.SelectSingleNode("minLength");
                int.TryParse(node.Value, out _minLength);
                node = validationNode.SelectSingleNode("maxLength");
                int.TryParse(node.Value, out _maxLength);
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
        private class choiceValidator: IValidator
        {
            List<string> choices = new List<string>();
            bool IValidator.setup(XmlNode validationNode)
            {
                foreach (XmlNode node in validationNode.SelectNodes("fixedValueSet/value"))
                {
                    choices.Add(node.Value.ToLower());
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
        private class fileValidator: IValidator
        {
            bool _mustExist = false;
            bool IValidator.setup(XmlNode validationNode)
            {
                XmlNode node = validationNode.SelectSingleNode("mustExist");
                string test = node.Value.ToLower();

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
                if(_mustExist)
                    if (!testFile.Exists)
                    {
                        parent._lastValidationError = "file does not exist";
                        return false;
                    }
                return true;
            }
        }
        private class dateTimeValidator:IValidator
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
        #endregion
    }
}
