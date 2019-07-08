using System;
using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.CLASSES.GAMEROOM;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class SessionJoin : Stanza
	{
		private GameRoom Room;

		public SessionJoin(Client User, XmlDocument Packet = null)
			: base(User, Packet)
		{
			if (!(Type == "result"))
			{
				Room = User.Player.RoomPlayer.Room;
				if (Type == "get" || Type == null)
				{
					Process();
				}
			}
		}

		internal override void Process()
		{
            string text = Room.Dedicated.IPAddress;
			//if (text.StartsWith("127.0.0.1") || (text.StartsWith("192.168.1.") && !User.IPAddress.StartsWith("192.168.1")))
			//{
			//	text = Gateway.WANIP;
			//}
            if (Type == "result") return;
			XDocument xDocument = new XDocument();
			XElement xElement = new XElement(Gateway.JabberNS + "iq");
			xElement.Add(new XAttribute("type", (Type == "get") ? "result" : "get"));
			xElement.Add(new XAttribute("from", "k01.warface"));
			xElement.Add(new XAttribute("to", User.JID));
			xElement.Add(new XAttribute("id", (Type == "get") ? Id : User.Player.Random.Next(99999, int.MaxValue).ToString()));
			XElement xElement2 = new XElement(Stanza.NameSpace + "query");
			XElement xElement3 = new XElement("session_join");
			xElement3.Add(new XAttribute("room_id", Room.Core.RoomId));
			xElement3.Add(new XAttribute("server", $"WarTLS SessId {Room.Core.RoomId}"));
			xElement3.Add(new XAttribute("hostname", text));
			xElement3.Add(new XAttribute("port", Room.Dedicated.DedicatedPort));
			xElement3.Add(new XAttribute("local", "0"));
			xElement3.Add(new XAttribute("session_id", Room.Session.ID));
			xElement3.Add(new XAttribute("helper_server", text));
			xElement3.Add(new XAttribute("helper_port", Room.Dedicated.DedicatedPort + 1));
			xElement3.Attributes("xmlns").Remove();
			xElement2.Add(xElement3);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
			Program.WriteLine($"{User.Player.Nickname} присоединяется на карту. ROOMID: {Room.Core.RoomId}, Канал: {User.Channel.Resource}", ConsoleColor.Red);
			
		}
	}
}
