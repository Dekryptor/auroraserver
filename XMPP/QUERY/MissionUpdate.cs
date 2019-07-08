using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.CLASSES.GAMEROOM;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class MissionUpdate : Stanza
	{
		private GameRoom Room;

		public MissionUpdate(Client User, XmlDocument Packet = null)
			: base(User, Packet)
		{
			Room = User.Player.RoomPlayer.Room;
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
				XElement xElement3 = new XElement("mission_update");
				xElement3.Add(Room.Serialize());
				xElement2.Add(xElement3);
				xElement.Add(xElement2);
				xDocument.Add(xElement);
				Compress(ref xDocument);
				User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
			}
		}
	}
}
