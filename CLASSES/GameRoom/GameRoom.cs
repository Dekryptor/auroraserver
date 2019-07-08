using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AuroraServer.CLASSES.GAMEROOM.CORE;
using AuroraServer.XMPP.QUERY;

namespace AuroraServer.CLASSES.GAMEROOM
{
	internal class GameRoom
	{
		internal static long Seed = 1L;

		internal AuroraServer.CLASSES.GAMEROOM.CORE.Core Core = new AuroraServer.CLASSES.GAMEROOM.CORE.Core();
		internal Client Dedicated;
		internal Session Session = new Session();
		internal RoomMaster RoomMaster = new RoomMaster();
		internal PlayersReserved PlayersReserved = new PlayersReserved();
		internal Mission Mission = new Mission();
		internal CustomParams CustomParams = new CustomParams();
		internal TeamColors TeamColors = new TeamColors();
		internal Players Players = new Players();
		internal string Name = "NoNameYet";
		internal List<long> KickedUsers = new List<long>();
		internal byte MaxPlayerAtTeam => Convert.ToByte((int)CustomParams.MaxPlayers / 2);

		internal void SessionStarter()
		{
			if (Dedicated != null)
			{
				Dedicated.Player.RoomPlayer.Room = this;
				new MissionLoad(Dedicated).Process();
			}

		}

		internal void Sync(Client NonInclude = null)
		{
			try
			{
				Client[] array = Players.Users.ToArray();
				foreach (Client client in array)
				{
					if (NonInclude != client)
					{
						new GameRoom_Sync(client).Process();
					}
				}
			}
			catch
			{
			}
		}

		internal void AutoBalanceProcess()
		{
			Players.Users = (from Attribute in Players.Users.ToList()
			orderby Attribute.Player.RoomPlayer.Status == Status.READY
			select Attribute).ToList();
			Client[] array = Players.Users.ToArray();
			foreach (Client client in array)
			{
				byte b = (byte)Players.Users.Count((Client Attribute) => Attribute.Player.RoomPlayer.TeamId == Teams.WARFACE);
				byte b2 = (byte)Players.Users.Count((Client Attribute) => Attribute.Player.RoomPlayer.TeamId == Teams.BLACKWOOD);
				if (b > b2)
				{
					client.Player.RoomPlayer.TeamId = Teams.BLACKWOOD;
				}
				else if (b2 > b)
				{
					client.Player.RoomPlayer.TeamId = Teams.WARFACE;
				}
			}
			Sync();
		}

		internal XElement Serialize(bool IncludeData = false)
		{
			XElement xElement = Core.Serialize(RoomMaster.UserId, Players);
			XElement xElement2 = new XElement("game_room");
			xElement2.Add(new XAttribute("room_id", Core.RoomId));
			xElement2.Add(new XAttribute("room_type", Core.RoomType));
			xElement.Add(Players.Serialize());
			xElement.Add(PlayersReserved.Serialize());
			xElement.Add(TeamColors.Serialize());
			xElement2.Add(xElement);
			xElement2.Add(Session.Serialize());
			xElement2.Add(Mission.Serialize(IncludeData));
			xElement2.Add(CustomParams.Serialize());
			xElement2.Add(RoomMaster.Serialize());
			return xElement2;
		}
	}
}
