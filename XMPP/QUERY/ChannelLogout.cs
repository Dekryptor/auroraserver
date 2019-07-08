using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class ChannelLogout : Stanza
	{
		private string Channel;

		public ChannelLogout(Client User, XmlDocument Packet = null)
			: base(User, Packet)
		{
			if (User.Channel != null)
			{
				User.Channel.Users.Remove(User);
				User.Channel = null;
			}
			if (Packet != null)
			{
				Process();
			}
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
			XElement content = new XElement("channel_logout");
			xElement2.Add(content);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			Compress(ref xDocument);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
