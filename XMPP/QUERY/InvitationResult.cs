using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class InvitationResult : Stanza
	{
		private InvitationTicket Ticket;

		public InvitationResult(Client User, XmlDocument Packet = null)
			: base(User, Packet)
		{
		}

		public InvitationResult(Client User, InvitationTicket Ticket)
			: base(User, null)
		{
			this.Ticket = Ticket;
			Process();
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
			XElement xElement3 = new XElement("invitation_result");
			xElement3.Add(new XAttribute("result", Ticket.Result));
			xElement3.Add(new XAttribute("user", Ticket.Receiver.Player.Nickname));
			xElement3.Add(new XAttribute("is_follow", Ticket.IsFollow ? 1 : 0));
			xElement3.Add(new XAttribute("user_id", Ticket.Receiver.Player.TicketId));
			xElement2.Add(xElement3);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			Compress(ref xDocument);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
			Ticket.Receiver.InvitationTicket.Remove(Ticket);
			Ticket.Sender.InvitationTicket.Remove(Ticket);
		}
	}
}
