using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class GetContacts : Stanza
	{
		private string Channel;

		public GetContacts(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
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
			XElement xElement3 = new XElement("get_contracts");
			xElement3.Add(new XElement("contract", new XAttribute("profile_id", User.Player.UserID), new XAttribute("rotation_id", "9"), new XAttribute("contract_name", ""), new XAttribute("current", "0"), new XAttribute("total", "0"), new XAttribute("rotation_time", "6780.903746"), new XAttribute("status", "0"), new XAttribute("is_available", "0")));
			xElement2.Add(xElement3);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
