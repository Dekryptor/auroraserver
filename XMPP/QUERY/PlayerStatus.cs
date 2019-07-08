using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class PlayerStatus : Stanza
	{
		public PlayerStatus(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
			User.Status = int.Parse(Query.Attributes["new_status"].InnerText);
			User.Location = Query.Attributes["to"].InnerText;
			Process();
			if (User.Player.RoomPlayer.Room != null)
			{
				User.Player.RoomPlayer.Room.Sync();
			}
		}

		internal override void Process()
		{
			XDocument xDocument = new XDocument();
			XElement xElement = new XElement(Gateway.JabberNS + "iq");
			xElement.Add(new XAttribute("type", "result"));
			xElement.Add(new XAttribute("from", "k01.warface"));
			xElement.Add(new XAttribute("to", User.JID));
			xElement.Add(new XAttribute("id", Id));
			XElement content = new XElement(Stanza.NameSpace + "query");
			xElement.Add(content);
			xDocument.Add(xElement);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
