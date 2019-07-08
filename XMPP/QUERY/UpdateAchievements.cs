using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class UpdateAchievements : Stanza
	{
		private string Channel;

		public UpdateAchievements(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
			Client client = ArrayList.OnlineUsers.Find((Client Attribute) => Attribute.Player.UserID == long.Parse(Query.FirstChild.Attributes["profile_id"].InnerText));
			Player player = null;
			if (client == null)
			{
				player = new Player
				{
					UserID = long.Parse(Query.FirstChild.Attributes["profile_id"].InnerText)
				};
				if (player.Load())
				{
					client = new Client
					{
						Player = player
					};
				}
			}
			if (client != null)
			{
				foreach (XmlNode childNode in Query.FirstChild.ChildNodes)
				{
					client.Player.UpdateAchievement(childNode);
				}
				client.Player.Save();
			}
			Process();
		}

		internal override void Process()
		{
			XDocument xDocument = new XDocument();
			XElement xElement = new XElement(Gateway.JabberNS + "iq");
			xElement.Add(new XAttribute("type", "result"));
			xElement.Add(new XAttribute("from", "masterserver@warface/aurora"));
			xElement.Add(new XAttribute("to", User.JID));
			xElement.Add(new XAttribute("id", Id));
			XElement xElement2 = new XElement(Stanza.NameSpace + "query");
			XElement content = new XElement("update_achievements");
			xElement2.Add(content);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
