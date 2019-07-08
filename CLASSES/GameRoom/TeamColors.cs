using System.Xml.Linq;

namespace AuroraServer.CLASSES.GAMEROOM.CORE
{
	internal class TeamColors
	{
		internal long UserId;

		internal int Revision = 2;

		internal XElement Serialize()
		{
			XElement xElement = new XElement("team_colors");
			XElement content = new XElement("team_color", new XAttribute("id", "1"), new XAttribute("color", "4294907157"));
			XElement content2 = new XElement("team_color", new XAttribute("id", "2"), new XAttribute("color", "4279655162"));
			xElement.Add(content);
			xElement.Add(content2);
			return xElement;
		}
	}
}
