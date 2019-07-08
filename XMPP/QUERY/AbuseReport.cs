using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class AbuseReport : Stanza
	{
		private string Channel;

		public AbuseReport(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
			string innerText = Query.Attributes["type"].InnerText;
			string Nickname = Query.Attributes["target"].InnerText;
			if (innerText == "cheat" && User.Player.Privilegie >= PrivilegieId.MODERATOR)
			{
				Client client = ArrayList.OnlineUsers.Find((Client Attribute) => Attribute.Player.Nickname == Nickname);
				if (client == null)
				{
					Player player = new Player
					{
						Nickname = Nickname
					};
					if (!player.Load())
					{
						User.ShowMessage("User by name: " + Nickname + " not found at server!", Green: true);
						return;
					}
					client = new Client
					{
						Player = player
					};
				}
				client.Player.BanType = BanType.ALL_PERMANENT;
				client.Player.UnbanTime = 0L;
				client.Player.Save();
				if (client.JID != null)
				{
					client.Dispose();
				}
				User.ShowMessage("User by name: " + Nickname + " successfily banned at server!");
			}
			Process();
		}

		internal override void Process()
		{
			XDocument xDocument = new XDocument();
			XElement xElement = new XElement(Gateway.JabberNS + "iq");
			xElement.Add(new XAttribute("type", "result"));
			xElement.Add(new XAttribute("from", To));
			xElement.Add(new XAttribute("to", User.JID));
			xElement.Add(new XAttribute("id", Id));
			XElement xElement2 = new XElement(Stanza.NameSpace + "query");
			XElement content = new XElement("abuse_report");
			xElement2.Add(content);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
