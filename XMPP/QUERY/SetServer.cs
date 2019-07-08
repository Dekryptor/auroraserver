using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class SetServer : Stanza
	{
		private string Channel;

		public SetServer(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
			User.DedicatedPort = ushort.Parse(Query.Attributes["port"].InnerText);
			User.Status = byte.Parse(Query.Attributes["status"].InnerText);
			if (User.Status == 1)
			{
				new MissionUpdate(User).Process();
			}
			if (User.Status == 4 || (User.Player.RoomPlayer.Room != null && User.Player.RoomPlayer.Room.Players.Users.Count == 0))
			{
				new MissionUnload(User).Process();
			}
			Process();
		}

		internal override void Process()
		{
			XDocument xDocument = new XDocument();
			XElement xElement = new XElement(Gateway.JabberNS + "iq");
			xElement.Add(new XAttribute("type", "result"));
			xElement.Add(new XAttribute("from", "masterserver@warface/wartls"));
			xElement.Add(new XAttribute("to", User.JID));
			xElement.Add(new XAttribute("id", Id));
			XElement xElement2 = new XElement(Stanza.NameSpace + "query");
			XElement content = new XElement("setserver");
			xElement2.Add(content);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
