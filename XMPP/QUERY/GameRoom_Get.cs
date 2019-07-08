using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.CLASSES.GAMEROOM;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class GameRoom_Get : Stanza
	{
		public GameRoom_Get(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
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
			XElement xElement3 = new XElement("gameroom_get");
			GameRoom[] array = User.Channel.GameRoomList.ToArray();
			foreach (GameRoom gameRoom in array)
			{
				try
				{
					if (gameRoom.Players.Users.Count == 0)
					{
						User.Channel.GameRoomList.Remove(gameRoom);
					}
					else
					{
						xElement3.Add(gameRoom.Serialize());
					}
				}
				catch
				{
					User.Channel.GameRoomList.Remove(gameRoom);
				}
			}
			xElement2.Add(xElement3);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			Compress(ref xDocument);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
