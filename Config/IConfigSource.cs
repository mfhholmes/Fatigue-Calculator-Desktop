using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Fatigue_Calculator_Desktop.Config
{
    public interface IConfigSource
    {
        XmlDocument getConfigXML();
        bool saveConfigXML(XmlDocument newVersion);
        string URL { get; }
    }
}
