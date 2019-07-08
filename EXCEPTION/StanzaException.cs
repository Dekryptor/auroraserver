using System;
using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;

namespace AuroraServer.EXCEPTION
{
	internal class StanzaException : Exception
	{
        public StanzaException(Client User, XmlDocument Query, int ErrorId)
        {
            User.Send(new XDocument(new object[]
            {
                new XElement("iq", new object[]
                {
                    new XAttribute("from", "aurora@server/StanzaException"),
                    new XAttribute("to", User.JID),
                    new XAttribute("id", Query["iq"].Attributes["id"].InnerText),
                    new XAttribute("type", "result"),
                    new XElement("urn:cryonline:k01" + "query", new XElement(Query["iq"]["query"].FirstChild.Name)),
                    new XElement("error", new object[]
                    {
                        new XAttribute("type", "continue"),
                        new XAttribute("code", "8"),
                        new XAttribute("custom_code", ErrorId),
                        new XElement("urn:ietf:params:xml:ns:xmpp-stanzas" + "public-server-error"),
                        new XElement("urn:ietf:params:xml:ns:xmpp-stanzas" + "text", "Custom query error")
                    })
                })
            }).ToString(SaveOptions.DisableFormatting));
        }
    }
}
