using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.CLASSES.GAMEROOM;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class GameRoom_SetName : Stanza
	{
		private string Channel;

		public GameRoom_SetName(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
			GameRoom room = User.Player.RoomPlayer.Room;
			room.Core.Name = Query.Attributes["room_name"].InnerText;
			room.Core.Revision++;
			Process();
			room.Sync(User);
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
			XElement xElement3 = new XElement("gameroom_setname");
			xElement3.Add(User.Player.RoomPlayer.Room.Serialize());
			xElement2.Add(xElement3);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			Compress(ref xDocument);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
