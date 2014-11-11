using System;

namespace Fatigue_Calculator_Desktop
{
	public class identity : IEquatable<identity>
	{
		private const char QUOTE = '"';
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

		public enum researchStates
		{
			research_unasked,
			research_approved,
			research_denied
		}

		private researchStates _researchApproved = researchStates.research_unasked;

		public researchStates ResearchApproved
		{
			get { return _researchApproved; }
			set { _researchApproved = value; }
		}

		public string logString
		{
			get
			{
				return _name;
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
			result += ",";
			if (_researchApproved == researchStates.research_approved)
				result += QUOTE + "approved" + QUOTE;
			else if (_researchApproved == researchStates.research_denied)
				result += QUOTE + "denied" + QUOTE;
			else
				result += QUOTE + "unknown" + QUOTE;
			return result;
		}
		public string ToSmallString()
		{
			return _id + " : " + _name;		
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
							_researchApproved = researchStates.research_unasked;
							_isValid = false;
						}
						else
							// check for ":" in the middle
							if (values[0].IndexOf(@":") > 0)
							{
								int brk = values[0].IndexOf(@":");
								_id = values[0].Substring(0, brk);
								_name = values[0].Substring(brk);
							}
							else
							{
								// just the name
								_name = line;
								_id = "";
								_researchApproved = researchStates.research_unasked;
								_isValid = true;
							}
						break;
					}
				case 2:
					{
						// name and number
						_id = values[0];
						_name = values[1];
						_researchApproved = researchStates.research_unasked;
						_isValid = true;
						break;
					}
				case 3:
					{
						// name, number and research question
						_id = values[0];
						_name = values[1];
						if (values[2].ToLower() == "approved")
							_researchApproved = researchStates.research_approved;
						else if (values[2].ToLower() == "denied")
							_researchApproved = researchStates.research_denied;
						else
							_researchApproved = researchStates.research_unasked;
						_isValid = true;
						break;
					}
				default:
					{
						// nothing to do, there's no meaningful interpretation
						_name = "";
						_id = "";
						_researchApproved = researchStates.research_unasked;
						_isValid = false;
						break;
					}
			}
		}

		public identity(string name, string id, researchStates research)
		{
			_name = name.ToUpper();
			_id = id.ToUpper();
			_researchApproved = research;
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
			// research question has no bearing on equality
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}