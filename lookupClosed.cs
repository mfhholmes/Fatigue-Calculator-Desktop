using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fatigue_Calculator_Desktop
{
    class lookupClosed : IidentityLookup
    {
        private IIdentityService _source;

        public lookupClosed(IIdentityService source)
        {
            _source = source;
        }

        IIdentityService IidentityLookup.source
        {
            get
            {
                return _source;
            }
            set
            {
                _source = value;
            }
        }

        int IidentityLookup.getMatchCount(string value)
        {
            // return zero if no source
            if (_source == null)
                return 0;
            // always return zero matches for an empty string
            if (value.Length == 0)
                return 0;
            // unsure whether they're entering a name or an id, so return the one with the largest matches
            int nameMatchCount = 0;
            nameMatchCount = _source.LookUpName(value).Count;
            int idMatchCount = 0;
            idMatchCount = _source.LookUpId(value).Count;
            if (nameMatchCount > idMatchCount)
                return nameMatchCount;
            else
                return idMatchCount;
        }

        string IidentityLookup.getBestMatch(string value)
        {
            // return empty string if no source
            if (_source == null)
                return "";

            // always return an empty match for an empty string
            if (value.Length == 0)
                return "";
            // unsure whether they're entering a name or an id, so return the one with the largest matches
            List<identity> matches  = _source.LookUpName(value);
            if(matches.Count > 0)
                return matches.ElementAt<identity>(0).Name;

            matches = _source.LookUpId(value);
            if (matches.Count > 0)
                return matches.ElementAt<identity>(0).Id;

            // no exact matches
            return "";
        }

        identity IidentityLookup.validate(string value)
        {
            // return empty string if no source
            if (_source == null)
                return null;

            // check that we've got an exact match
            // if not, add the name to the file (assume the value is a name)
            // and return it
            // always return an empty match for an empty string
            if (value.Length == 0)
                return null;
            // unsure whether they're entering a name or an id, so return the one with the largest matches
            List<identity> matches = _source.ExactMatchName(value);
            if (matches.Count == 1)
                return matches.ElementAt<identity>(0);

            matches = _source.ExactMatchId(value);
            if (matches.Count == 1)
                return matches.ElementAt<identity>(0);

            // no exact matches, so return empty string
            return null;
        }

        bool IidentityLookup.displayPage
        {
            get { return true; }
        }
    }
}
