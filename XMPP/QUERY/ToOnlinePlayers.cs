using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;

namespace AuroraServer.XMPP.QUERY
{
	internal class ToOnlinePlayers : Stanza
	{
		private string Channel;

		private Client Receiver;

		public ToOnlinePlayers(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
			Receiver = ArrayList.OnlineUsers.Find((Client Attribute) => Attribute.JID == To);
			Process();
		}

		internal override void Process()
		{
			if (Receiver != null)
			{
				Receiver.Send(Packet.InnerXml);
				return;
			}
			XDocument xDocument = new XDocument();
			XElement xElement = new XElement("iq");
			xElement.Add(new XAttribute("type", Type));
			if (To != null)
			{
				xElement.Add(new XAttribute("from", To));
			}
			xElement.Add(new XAttribute("to", User.JID));
			xElement.Add(new XAttribute("id", Id));
			XElement content = new XElement(Stanza.NameSpace + "query");
			XElement xElement2 = new XElement("error");
			xElement2.Add(new XAttribute("type", "cancel"));
			xElement2.Add(new XAttribute("code", 503));
			xElement2.Add(new XElement((XNamespace)"urn:ietf:params:xml:ns:xmpp-stanzas" + "service-unavailable"));
			xElement.Add(content);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
