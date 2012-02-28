using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fatigue_Calculator_Desktop
{
    public class identity : IEquatable<identity>
    {
        const char QUOTE = '"';
        private string _id = "";
        private bool _isValid = false;

        public string Id
        {
            get { return _id; }
            set { _id = value.ToUpper(); }
        }
        private string _name = "";
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value.ToUpper();
                _isValid = true;
            }
        }
        public bool isValid
        {
            get { return _isValid; }
        }
        public override string ToString()
        {
            string result = "";
            result += QUOTE + _id + QUOTE;
            result += ",";
            result += QUOTE + _name + QUOTE;
            return result;
        }
        public identity(string line)
        {
            //string noquotes = line.Replace(QUOTE.ToString(), String.Empty);
            string[] values = line.ToUpper().Split(',');
            //remove any whitespace outside the quotes
            for (int i = 0; i < values.Length; i++)
            {
                string item = values[i];
                // check for open quote
                if (item.IndexOf(QUOTE) > -1)
                {
                    // ignore anything before the first quote
                    item = item.Substring(item.IndexOf(QUOTE) + 1);
                    // check for close quote and ignore anything after it (up to the comma or end of line obviously)
                    if (item.LastIndexOf(QUOTE) > 1)
                        item = item.Substring(0, item.LastIndexOf(QUOTE));
                    else
                        item = ""; // missing close quote means the item is messed up, so clear the item and hope the others are OK
                }
                // allow items with no quotes at all to pass through...it's possible the spec changed or we're dealing with an outside file, or something
                values[i] = item;
            }
            switch (values.Length)
            {
                case 1:
                    {
                        // check for zero-length
                        if (values[0].Length == 0)
                        {
                            _name = "";
                            _id = "";
                            _isValid = false;
                            break;
                        }
                        else
                        {
                            _name = line;
                            _id = "";
                            _isValid = true;
                            break;
                        }
                    }
                case 2:
                    {
                        // name and number
                        _id = values[0];
                        _name = values[1];
                        _isValid = true;
                        break;
                    }
                default:
                    {
                        // nothing to do, there's no meaningful interpretation
                        _name = "";
                        _id = "";
                        _isValid = false;
                        break;
                    }
            }
        }

        public identity(string name, string id)
        {
            _name = name;
            _id = id;
            _isValid = true;
        }

        bool IEquatable<identity>.Equals(identity other)
        {
            return ((_id == other.Id) && (_name == other.Name));
        }
        public override bool Equals(object other)
        {
            // check for null
            if (other == null)
                return false;
            // check for identity object
            identity otherId = other as identity;
            if (otherId == null)
                return false;
            // check for equal values
            if ((otherId.Name.ToUpper() == this.Name.ToUpper()) && (otherId.Id.ToUpper() == this.Id.ToUpper()))
                return true;
            else
                return false;

        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
