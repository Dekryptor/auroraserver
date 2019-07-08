using System;
using System.Drawing;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.CLASSES.CLAN;
using AuroraServer.NETWORK;
using AuroraServer.EXCEPTION;

namespace AuroraServer.XMPP.QUERY
{
	internal class ClanCreate : Stanza
	{
        public ClanCreate(Client User, XmlDocument Packet) : base(User, Packet)
		{
            Clan = new Clan(Query.Attributes["clan_name"].InnerText, Encoding.UTF8.GetString(Convert.FromBase64String(Query.Attributes["description"].InnerText)), User);
			Process();
        }

        internal override void Process()
		{
			XDocument xDocument = new XDocument();
			XElement xElement = new XElement(Gateway.JabberNS + "iq");
			xElement.Add(new XAttribute("type", "result"));
			xElement.Add(new XAttribute("from", this.To));
			xElement.Add(new XAttribute("to", this.User.JID));
			xElement.Add(new XAttribute("id", this.Id));
			XElement xElement2 = new XElement(Stanza.NameSpace + "query");
			XElement xElement3 = new XElement("clan_create");
			xElement3.Add(Clan.Serialize());
			xElement2.Add(xElement3);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			Console.WriteLine(string.Format("[{0}][{1}]", GetType().Name, xDocument));
			base.Compress(ref xDocument);
			this.User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}

		private string Channel;
		private Clan Clan;
	}
}
