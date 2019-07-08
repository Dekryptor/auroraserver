using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class SetBanner : Stanza
	{
		public SetBanner(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
			try
			{
				User.Player.BannerMark = int.Parse(Query.Attributes["banner_mark"].InnerText);
				User.Player.BannerStripe = int.Parse(Query.Attributes["banner_stripe"].InnerText);
				User.Player.BannerBadge = int.Parse(Query.Attributes["banner_badge"].InnerText);
			}
			catch
			{
			}
			User.Player.Save();
			Process();
		}

		internal override void Process()
		{
			XDocument xDocument = new XDocument();
			XElement xElement = new XElement(Gateway.JabberNS + "iq");
			xElement.Add(new XAttribute("type", "result"));
			xElement.Add(new XAttribute("from", "k01.warface"));
			xElement.Add(new XAttribute("to", User.JID));
			xElement.Add(new XAttribute("id", Id));
			XElement content = new XElement(Stanza.NameSpace + "query", new XElement("set_banner"));
			xElement.Add(content);
			xDocument.Add(xElement);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
