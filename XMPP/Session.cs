using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP
{
	internal class Session
	{
		private readonly XNamespace NameSpace = "urn:ietf:params:xml:ns:xmpp-session";

		internal Session(Client User, XmlDocument Packet)
		{
			XElement xElement = new XElement(Gateway.JabberNS + "iq");
			xElement.Add(new XAttribute("type", "result"));
			xElement.Add(new XAttribute("id", Packet["iq"].Attributes["id"].InnerText));
			if (User.JID != null)
			{
				xElement.Add(new XAttribute("to", User.JID));
			}
			XElement content = new XElement(NameSpace + "session");
			xElement.Add(content);
			User.Send(xElement.ToString(SaveOptions.DisableFormatting));
		}
	}
}
