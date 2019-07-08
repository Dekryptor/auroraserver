using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class ProfileItemGetStatus : Stanza
	{
		private string Nickname;

		private Client OnlineUser;

		public ProfileItemGetStatus(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
			Nickname = Query.Attributes["nickname"].InnerText;
			OnlineUser = ArrayList.OnlineUsers.Find((Client Attribute) => Attribute.Player.Nickname == Nickname);
			Client onlineUser = OnlineUser;
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
			XElement xElement3 = new XElement("profile_info_get_status");
			if (OnlineUser == null)
			{
				Player player = new Player
				{
					Nickname = Nickname
				};
				if (!player.Load())
				{
					return;
				}
				OnlineUser = new Client
				{
					Player = player
				};
			}
			xElement3.Add(new XAttribute("nickname", Nickname));
			xElement3.Add(new XElement("profile_info", new XElement("info", new XAttribute("nickname", Nickname), new XAttribute("online_id", (OnlineUser.JID != null) ? OnlineUser.JID : ""), new XAttribute("status", OnlineUser.Status), new XAttribute("rank", OnlineUser.Player.Rank), new XAttribute("user_id", OnlineUser.Player.TicketId), new XAttribute("profile_id", OnlineUser.Player.UserID))));
			xElement2.Add(xElement3);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
