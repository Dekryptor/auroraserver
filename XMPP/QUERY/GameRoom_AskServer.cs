using System;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.CLASSES.GAMEROOM;
using AuroraServer.EXCEPTION;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class GameRoom_AskServer : Stanza
	{
		private enum AskServerError
		{
			NOT_MASTER = 0,
			INVALID_MISSION = 1,
			NOT_BALANCED = 3,
			ALREADY_STARTED = 7
		}

		private GameRoom Room;

		public GameRoom_AskServer(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
            Room = User.Player.RoomPlayer.Room;
			Room.Players.Users.Count((Client Attribute) => Attribute.Player.RoomPlayer.Status == Status.READY);
			Room.Players.Users.Count(delegate(Client Attribute)
			{
				if (Attribute.Player.RoomPlayer.TeamId == Teams.WARFACE)
				{
					return Attribute.Player.RoomPlayer.Status == Status.READY;
				}
				return false;
			});
			Room.Players.Users.Count(delegate(Client Attribute)
			{
				if (Attribute.Player.RoomPlayer.TeamId == Teams.BLACKWOOD) return Attribute.Player.RoomPlayer.Status == Status.READY;
				return false;
			});
			Room.Dedicated = ArrayList.OnlineUsers.Find(delegate(Client Attribute)
			{
				if (Attribute.Dedicated) return Attribute.Player.RoomPlayer.Room == null;
				return false;
			});
            if (Room.Dedicated == null)
			{
                foreach(Client user in Room.Players.Users)
                {
                    user.ShowMessage("Все сервера для запуска игры заняты",true);
                }
				new StanzaException(User, Packet, 2);
			}
			new Thread(Room.SessionStarter).Start();
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
			XElement content = new XElement("gameroom_askserver");
			xElement2.Add(content);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
