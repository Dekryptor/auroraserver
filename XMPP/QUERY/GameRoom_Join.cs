using System.Linq;
using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.CLASSES.GAMEROOM;
using AuroraServer.EXCEPTION;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class GameRoom_Join : Stanza
	{
		private GameRoom Room;

		public GameRoom_Join(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
            try
            {
                if (User.Player.RoomPlayer.Room != null)
                {
                    new GameRoom_Leave(User, null);
                }
                Room = User.Channel.GameRoomList.Find((GameRoom Attribute) => Attribute.Core.RoomId == long.Parse(Query.Attributes["room_id"].InnerText));
                if (Room == null)
                {
                    new StanzaException(User, base.Packet, 2);
                    return;
                }
                if (Room.KickedUsers.Contains(User.Player.UserID))
                {
                    new StanzaException(User, base.Packet, 2);
                    return;
                }
                if (Room.Players.Users.Count >= Room.CustomParams.MaxPlayers)
                {
                    new StanzaException(User, base.Packet, 4);
                    return;
                }
                byte num = (byte)Room.Players.Users.Count((Client Attribute) => Attribute.Player.RoomPlayer.TeamId == Teams.WARFACE);
                byte b = (byte)Room.Players.Users.Count((Client Attribute) => Attribute.Player.RoomPlayer.TeamId == Teams.BLACKWOOD);
                if (num > b)
                {
                    User.Player.RoomPlayer.TeamId = Teams.BLACKWOOD;
                }
                else
                {
                    User.Player.RoomPlayer.TeamId = Teams.WARFACE;
                }
                User.Player.RoomPlayer.Room = Room;
                Room.Players.Users.Add(User);
                Process();
                if (Room.CustomParams.AutoTeamBalance)
                {
                    Room.AutoBalanceProcess();
                }
                else
                {
                    Room.Sync(User);
                }
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
			XElement xElement3 = new XElement("gameroom_join");
			xElement3.Add(Room.Serialize());
			xElement3.Add(new XAttribute("room_id", Room.Core.RoomId));
			xElement3.Add(new XAttribute("code", 0));
			xElement2.Add(xElement3);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			Compress(ref xDocument);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
