using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class GetAccountProfiles : Stanza
	{
		public GetAccountProfiles(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
            try
            {
                Process();
            }
            catch { }
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
			XElement xElement3 = new XElement("get_account_profiles");
			if (User.Player.ProfileCreated)
			{
				XElement xElement4 = new XElement("profile");
				xElement4.Add(new XAttribute("id", User.Player.UserID));
				xElement4.Add(new XAttribute("nickname", User.Player.Nickname));
				xElement3.Add(xElement4);
			}
			xElement2.Add(xElement3);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
