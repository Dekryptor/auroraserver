using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class InvitationAccept : Stanza
	{
		private InvitationTicket Ticket;

		public InvitationAccept(Client User, XmlDocument Packet = null)
			: base(User, Packet)
		{
			if (!(Type == "result"))
			{
				if ((bool)App.Default["UseOldMode"])
				{
					Ticket = User.InvitationTicket.Find((InvitationTicket Attribute) => Attribute.ID == Query.Attributes["token"].InnerText);
				}
				else
				{
					Ticket = User.InvitationTicket.Find((InvitationTicket Attribute) => Attribute.ID == Query.Attributes["ticket"].InnerText);
				}
				Ticket.Result = byte.Parse(Query.Attributes["result"].InnerText);
				new InvitationResult(Ticket.Sender, Ticket);
				Process();
			}
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
			XElement content = new XElement("invitation_accept");
			xElement2.Add(content);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			Compress(ref xDocument);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
