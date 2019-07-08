using System;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace AuroraServer.CLASSES.GAMEROOM.CORE
{
	internal class Mission
	{
		internal long UserId;

		internal int Revision = 2;

		internal XmlDocument Map;

		internal XmlNode PvEInfo;

		internal string Mode
		{
			get
			{
				if (Map == null)
				{
					return null;
				}
				return Map.FirstChild.Attributes["game_mode"].InnerText;
			}
		}

		internal XElement Serialize(bool IncludeData = false)
		{
			if (PvEInfo != null)
			{
				XElement root = XDocument.Parse(PvEInfo.OuterXml).Root;
				if (IncludeData)
				{
					root.Add(new XAttribute("data", Convert.ToBase64String(Encoding.UTF8.GetBytes(Map.InnerXml))));
				}
				return root;
			}
			XElement xElement = new XElement("mission");
			try
			{
				xElement.Add(new XAttribute("mission_key", Map.FirstChild.Attributes["uid"].InnerText));
				xElement.Add(new XAttribute("no_teams", (Mode == "ffa" || Mode == "hnt") ? 1 : 0));
				xElement.Add(new XAttribute("mode", Map.FirstChild.Attributes["game_mode"].InnerText));
				xElement.Add(new XAttribute("mode_name", Map.FirstChild["UI"]["GameMode"].Attributes["text"].InnerText));
				xElement.Add(new XAttribute("image", Map.FirstChild["UI"]["Description"].Attributes["icon"].InnerText));
				xElement.Add(new XAttribute("description", Map.FirstChild["UI"]["Description"].Attributes["text"].InnerText));
				xElement.Add(new XAttribute("name", Map.FirstChild.Attributes["name"].InnerText));
				xElement.Add(new XAttribute("difficulty", "normal"));
				xElement.Add(new XAttribute("type", ""));
				xElement.Add(new XAttribute("setting", Map.FirstChild["Basemap"].Attributes["name"].InnerText));
				xElement.Add(new XAttribute("time_of_day", Map.FirstChild.Attributes["time_of_day"].InnerText));
				xElement.Add(new XAttribute("revision", Revision));
				if (!IncludeData)
				{
					return xElement;
				}
				xElement.Add(new XAttribute("data", Convert.ToBase64String(Encoding.UTF8.GetBytes(Map.InnerXml))));
				return xElement;
			}
			catch
			{
				return xElement;
			}
		}
	}
}
