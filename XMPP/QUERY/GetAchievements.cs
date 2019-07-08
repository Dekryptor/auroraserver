using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class GetAchievements : Stanza
	{
		private string Channel;

		public GetAchievements(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
			Process();
		}

		internal override void Process()
		{
			Client client = ArrayList.OnlineUsers.Find((Client Attribute) => Attribute.Player.UserID == long.Parse(Query.FirstChild.Attributes["profile_id"].InnerText));
			if (client == null)
			{
				Player player = new Player
				{
					UserID = long.Parse(Query.FirstChild.Attributes["profile_id"].InnerText)
				};
				if (!player.Load())
				{
					return;
				}
				client = new Client
				{
					Player = player
				};
			}
			XDocument xDocument = new XDocument();
			XElement xElement = new XElement(Gateway.JabberNS + "iq");
			xElement.Add(new XAttribute("type", "result"));
			xElement.Add(new XAttribute("from", To));
			xElement.Add(new XAttribute("to", User.JID));
			xElement.Add(new XAttribute("id", Id));
			XElement xElement2 = new XElement(Stanza.NameSpace + "query");
			XElement xElement3 = new XElement("get_achievements");
			XElement xElement4 = new XElement("achievement");
			xElement4.Add(new XAttribute("profile_id", client.Player.UserID));
			if (client.Player.Achievements.FirstChild.ChildNodes.Count != 0)
			{
				foreach (XmlNode childNode in client.Player.Achievements.FirstChild.ChildNodes)
				{
					xElement4.Add(XDocument.Parse(childNode.OuterXml).Root);
				}
			}
			xElement3.Add(xElement4);
			xElement2.Add(xElement3);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			Compress(ref xDocument);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
