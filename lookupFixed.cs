using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fatigue_Calculator_Desktop
{
    class lookupFixed : IidentityLookup
    {
        identity _fixed;
        public lookupFixed()
        {
            _fixed = new identity(Properties.Settings.Default.FixedID);
        }
        IIdentityService IidentityLookup.source
        {
            get
            {
                return null;
            }
            set
            {
                // do nothing
                // we don't use a source
            }
        }

        int IidentityLookup.getMatchCount(string value)
        {
            return 1;
        }

        string IidentityLookup.getBestMatch(string value)
        {
            return _fixed.Name;
        }

        string IidentityLookup.validate(string value)
        {
            return _fixed.Name;
        }

        public bool displayPage
        {
            get { return false; }
        }
    }
}
