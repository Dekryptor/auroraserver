using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class Account : Stanza
	{
		private string Login;

		private string Password;

		public Account(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
			if (Query.Name == "account")
			{
				Login = Query.Attributes["login"].InnerText;
				Password = Query.Attributes["password"].InnerText.Replace("{:B:}row_emul", "");
			}
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
			XElement xElement3 = new XElement(Query.Name);
			if (Query.Name == "account")
			{
				xElement3.Add(new XAttribute("user", User.Player.TicketId));
				xElement3.Add(new XAttribute("survival_lb_enabled", "0"));
				xElement3.Add(new XAttribute("active_token", " "));
				xElement3.Add(new XAttribute("nickname", ""));
			}
			XElement xElement4 = new XElement("masterservers");
			foreach (Channel channel in ArrayList.Channels)
			{
				xElement4.Add(channel.Serialize());
			}
			xElement3.Add(xElement4);
			xElement2.Add(xElement3);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
