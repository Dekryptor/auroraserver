using System;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.NETWORK;

namespace AuroraServer.EXCEPTION
{
	internal class StanzaError : Exception
	{
		internal StanzaError(Client User, Stanza Query, string Message, int CustomCode, int Code = 8)
			: base(Message)
		{
			XDocument xDocument = new XDocument();
			XElement xElement = new XElement(Gateway.JabberNS + "iq");
			xElement.Add(new XAttribute("type", "result"));
			xElement.Add(new XAttribute("from", Query.To));
			xElement.Add(new XAttribute("to", User.JID));
			xElement.Add(new XAttribute("id", Query.Id));
			XElement xElement2 = new XElement(Stanza.NameSpace + "query");
			XElement content = new XElement(Query.Name);
			xElement2.Add(content);
			XElement xElement3 = new XElement((XNamespace)"urn:ietf:params:xml:ns:xmpp-stanzas" + "error");
			xElement3.Add(new XAttribute("type", "continue"));
			xElement3.Add(new XAttribute("code", 8));
			xElement3.Add(new XAttribute("custom_code", CustomCode));
			xElement.Add(xElement2);
			xElement.Add(xElement3);
			xDocument.Add(xElement);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
