using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Fatigue_Calculator_Desktop.Config
{
    public interface IValidator
    {
        bool setup(XmlNode validationNode);
        bool validate(configItem parent);
    }
}
