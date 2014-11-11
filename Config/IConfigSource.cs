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