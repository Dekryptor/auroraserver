using System.Linq;
using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.CLASSES.GAMEROOM;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class GameRoom_SetPlayer : Stanza
	{
		private string Channel;

		public GameRoom_SetPlayer(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
			GameRoom room = User.Player.RoomPlayer.Room;
			if (room == null)
			{
				return;
			}
			byte teamId = byte.Parse(Query.Attributes["team_id"].InnerText);
			if (User.Channel.ChannelType == "pve")
			{
				User.Player.RoomPlayer.TeamId = Teams.WARFACE;
			}
			byte b = (byte)room.Players.Users.Count((Client Attribute) => Attribute.Player.RoomPlayer.TeamId == Teams.WARFACE);
			byte b2 = (byte)room.Players.Users.Count((Client Attribute) => Attribute.Player.RoomPlayer.TeamId == Teams.BLACKWOOD);
			if (!room.CustomParams.AutoTeamBalance)
			{
				if (b > room.MaxPlayerAtTeam)
				{
					User.Player.RoomPlayer.TeamId = Teams.BLACKWOOD;
				}
				if (b2 > room.MaxPlayerAtTeam)
				{
					User.Player.RoomPlayer.TeamId = Teams.WARFACE;
				}
				else
				{
					User.Player.RoomPlayer.TeamId = (Teams)teamId;
				}
			}
			Status status = (Status)byte.Parse(Query.Attributes["status"].InnerText);
			if (User.Player.RoomPlayer.Status != status && room.Dedicated != null && room.Dedicated.Status < 4)
			{
				new MissionUpdate(room.Dedicated).Process();
			}
			User.Player.RoomPlayer.Status = status;
			User.Player.CurrentClass = byte.Parse(Query.Attributes["class_id"].InnerText);
			if (!room.CustomParams.SoldierEnabled || !room.CustomParams.EngineerEnabled || !room.CustomParams.SniperEnabled || !room.CustomParams.MedicEnabled)
			{
				Client[] array = room.Players.Users.ToArray();
				foreach (Client client in array)
				{
					bool flag = false;
					if (client.Player.CurrentClass == 0 && !room.CustomParams.SoldierEnabled)
					{
						flag = true;
					}
					if (client.Player.CurrentClass == 4 && !room.CustomParams.EngineerEnabled)
					{
						flag = true;
					}
					if (client.Player.CurrentClass == 3 && !room.CustomParams.MedicEnabled)
					{
						flag = true;
					}
					if (client.Player.CurrentClass == 2 && !room.CustomParams.SniperEnabled)
					{
						flag = true;
					}
					if (flag)
					{
						if (room.CustomParams.SoldierEnabled)
						{
							client.Player.CurrentClass = 0;
						}
						if (room.CustomParams.EngineerEnabled)
						{
							client.Player.CurrentClass = 4;
						}
						if (room.CustomParams.MedicEnabled)
						{
							client.Player.CurrentClass = 3;
						}
						if (room.CustomParams.SniperEnabled)
						{
							client.Player.CurrentClass = 2;
						}
					}
				}
			}
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
			XElement xElement3 = new XElement("gameroom_setplayer");
			xElement3.Add(User.Player.RoomPlayer.Room.Serialize());
			xElement2.Add(xElement3);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			Compress(ref xDocument);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
