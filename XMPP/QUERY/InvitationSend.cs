using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class InvitationSend : Stanza
	{
		private enum Results
		{
			Rejected = 1,
			AutoRejected = 2,
			MissionLocked = 12,
			RankRestricted = 13,
			FullRoom = 14,
			Banned = 0xF,
			BuildType = 0x10,
			NotInClan = 18,
			Participate = 19,
			AllClassLocked = 20,
			VersionMismatch = 21,
			NoAccessTokens = 22
		}

		private string Channel;

		private InvitationTicket Ticket;

		public InvitationSend(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
			if (!(bool)App.Default["UseOldMode"])
			{
				User.Player.RoomPlayer.GroupId = Query.Attributes["group_id"].InnerText;
			}
            Client client = ArrayList.OnlineUsers.Find((Client Attribute) => Attribute.Player.Nickname == Query.Attributes["nickname"].InnerText);
			InvitationTicket invitationTicket = new InvitationTicket(User, client)
			{
				GroupId = (((bool)App.Default["UseOldMode"]) ? "" : Query.Attributes["group_id"].InnerText)
			};
			if (client != null)
			{
				Ticket = invitationTicket;
				invitationTicket.IsFollow = (Query.Attributes["is_follow"].InnerText == "1");
				if (User.Channel.MinRank > client.Player.Rank || User.Channel.MaxRank < client.Player.Rank)
				{
					invitationTicket.Result = 13;
				}
				if (invitationTicket.Result == byte.MaxValue)
				{
					client.InvitationTicket.Add(invitationTicket);
					User.InvitationTicket.Add(invitationTicket);
					new InvitationRequest(client, invitationTicket);
				}
			}
			else
			{
				invitationTicket.Result = 0;
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
			XElement content = new XElement("invitation_send");
			xElement2.Add(content);
			xElement.Add(xElement2);
			if (Ticket.Result != byte.MaxValue)
			{
				XElement xElement3 = new XElement((XNamespace)"urn:ietf:params:xml:ns:xmpp-stanzas" + "error");
				xElement3.Add(new XAttribute("type", "continue"));
				xElement3.Add(new XAttribute("code", 8));
				xElement3.Add(new XAttribute("custom_code", Ticket.Result));
				xElement.Add(xElement3);
			}
			xDocument.Add(xElement);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
