using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.CLASSES.GAMEROOM;
using AuroraServer.NETWORK;
using AuroraServer.EXCEPTION;

namespace AuroraServer.XMPP.QUERY
{
	internal class GameRoom_SetInfo : Stanza
	{
		private string Channel;

		public GameRoom_SetInfo(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
			GameRoom room = User.Player.RoomPlayer.Room;
            string Uid = Query.Attributes["mission_key"].InnerText;
            XmlDocument xmlDocument = GameResources.Maps.Find((XmlDocument Attribute) => Attribute.FirstChild.Attributes["uid"].InnerText == Uid);
            room.Mission.Map = xmlDocument;
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
			XElement xElement3 = new XElement("gameroom_setinfo");
			xElement3.Add(User.Player.RoomPlayer.Room.Serialize());
			xElement2.Add(xElement3);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			Compress(ref xDocument);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
