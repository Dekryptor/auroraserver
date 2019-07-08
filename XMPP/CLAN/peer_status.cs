using System;
using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class peer_status : Stanza
	{
        public peer_status(Client User, XmlDocument Packet) : base(User, Packet)
		{
		}

		internal override void Process()
		{
			XDocument Packet = new XDocument();
			XElement iqElement = new XElement(Gateway.JabberNS + "iq");
			iqElement.Add(new XAttribute("type", "result"));
			iqElement.Add(new XAttribute("from", this.To));
			iqElement.Add(new XAttribute("to", this.User.JID));
			iqElement.Add(new XAttribute("id", this.Id));
			XElement queryElement = new XElement(Stanza.NameSpace + "query");
			Packet.Add(iqElement);
			User.Send(Packet.ToString(SaveOptions.DisableFormatting));
		}
	}
}
