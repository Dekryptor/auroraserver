using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.CLASSES.GAMEROOM;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class MissionUnload : Stanza
	{
		private string Channel;

		private GameRoom Room;

		public MissionUnload(Client User, XmlDocument Packet = null)
			: base(User, Packet)
		{
			if (!(Type == "result"))
			{
				Room = User.Player.RoomPlayer.Room;
				Room.Session.Status = 0;
				Room.Dedicated = null;
				User.Player.RoomPlayer.Room = null;
				Room.Sync();
				if (Packet != null)
				{
					Process();
				}
			}
		}

		internal override void Process()
		{
			if (!(Type == "result"))
			{
				XDocument xDocument = new XDocument();
				XElement xElement = new XElement(Gateway.JabberNS + "iq");
				xElement.Add(new XAttribute("type", "get"));
				xElement.Add(new XAttribute("from", "masterserver@warface/wartls"));
				xElement.Add(new XAttribute("to", User.JID));
				xElement.Add(new XAttribute("id", User.Player.Random.Next(99999, int.MaxValue)));
				XElement xElement2 = new XElement(Stanza.NameSpace + "query");
				XElement content = new XElement("mission_unload");
				xElement2.Add(content);
				xElement.Add(xElement2);
				xDocument.Add(xElement);
				User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
			}
		}
	}
}
