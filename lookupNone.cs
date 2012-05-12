using System;
using System.Collections.Generic;
using System.Text;

namespace Fatigue_Calculator_Desktop
{
    class lookupNone: IidentityLookup
    {
        IIdentityService IidentityLookup.source
        {
            get
            {
                // no source required
                return null;
            }
            set
            {
                // no source accepted
                // do nothing
            }
        }

        int IidentityLookup.getMatchCount(string value)
        {
            // value always matches nothing
            return 0;
        }

        string IidentityLookup.getBestMatch(string value)
        {
            return value;
        }

        identity IidentityLookup.validate(string value)
        {
            return new identity(value);
        }

        bool IidentityLookup.displayPage
        {
            get { return true; }
        }
    }
}
