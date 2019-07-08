using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.CLASSES.GAMEROOM;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class InvitationRequest : Stanza
	{
		private GameRoom Room;

		private InvitationTicket Ticket;

		public InvitationRequest(Client User, XmlDocument Packet = null)
			: base(User, Packet)
		{
		}

		public InvitationRequest(Client User, InvitationTicket Ticket)
			: base(User, null)
		{
			if (!(Type == "result"))
			{
				this.Ticket = Ticket;
				Room = Ticket.Sender.Player.RoomPlayer.Room;
				Process();
			}
		}

		internal override void Process()
		{
			XDocument xDocument = new XDocument();
			XElement xElement = new XElement(Gateway.JabberNS + "iq");
			xElement.Add(new XAttribute("type", "get"));
			xElement.Add(new XAttribute("from", "masterserver@warface/" + User.Channel.Resource));
			xElement.Add(new XAttribute("to", User.JID));
			xElement.Add(new XAttribute("id", User.Player.Random.Next(1, int.MaxValue)));
			XElement xElement2 = new XElement(Stanza.NameSpace + "query");
			XElement xElement3 = new XElement("invitation_request");
			if (Room != null)
			{
				xElement3.Add(new XAttribute("from", Ticket.Sender.Player.Nickname));
				xElement3.Add(new XAttribute(((bool)App.Default["UseOldMode"]) ? "token" : "ticket", Ticket.ID));
				xElement3.Add(new XAttribute("room_id", Room.Core.RoomId));
				xElement3.Add(new XAttribute("ms_resource", Ticket.Sender.Channel.Resource));
				xElement3.Add(new XAttribute("is_follow", Ticket.IsFollow ? 1 : 0));
				xElement3.Add(Room.Serialize());
			}
			else
			{
				XElement xElement4 = new XElement((XNamespace)"urn:ietf:params:xml:ns:xmpp-stanzas" + "error");
				xElement4.Add(new XAttribute("type", "cancel"));
				xElement4.Add(new XAttribute("code", 8));
				xElement4.Add(new XAttribute("custom_code", 1));
				xElement.Add(xElement4);
			}
			xElement2.Add(xElement3);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			Compress(ref xDocument);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
