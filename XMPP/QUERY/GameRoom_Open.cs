using System;
using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.CLASSES.GAMEROOM;
using AuroraServer.EXCEPTION;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class GameRoom_Open : Stanza
	{
		private string Channel;

		private int Code;

		public GameRoom_Open(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
			if (User.Channel.ChannelType == "pve")
			{
				Query.Attributes["max_players"].InnerText = "5";
			}
			else
			{
				if (byte.Parse(Query.Attributes["max_players"].InnerText) > 16)
				{
					Code = 3;
				}
				if (byte.Parse(Query.Attributes["max_players"].InnerText) < 4)
				{
					Code = 3;
				}
				if ((int)byte.Parse(Query.Attributes["max_players"].InnerText) % 2 != 0)
				{
					Code = 3;
				}
			}
			if (Code != 0)
			{
				Process();
				return;
			}
			string Uid = Query.Attributes["mission"].InnerText;
			if (User.Player.RoomPlayer.Room != null)
			{
				new GameRoom_Leave(User, null);
			}
			GameRoom gameRoom = new GameRoom
			{
				Core = 
				{
					RoomId = GameRoom.Seed
				}
			};
			GameRoom.Seed++;
			gameRoom.RoomMaster.UserId = User.Player.UserID;
			if (User.Channel.ChannelType != "pve")
			{
				XmlDocument xmlDocument = GameResources.Maps.Find((XmlDocument Attribute) => Attribute.FirstChild.Attributes["uid"].InnerText == Uid);
				if (xmlDocument.FirstChild.Attributes["release_mission"].InnerText == "0")
				{
					new StanzaException(User, Packet, 1);
				}
				else
				{
					gameRoom.Mission.Map = xmlDocument;
				}
			}
            if(User.Channel.ChannelType == "pve")
            {
                XmlDocument xmlDocument = GameResources.Maps.Find((XmlDocument Attribute) => Attribute.FirstChild.Attributes["uid"].InnerText == Uid);
                if (xmlDocument.FirstChild.Attributes["release_mission"].InnerText == "1")
                {
                    new StanzaException(User, Packet, 1);
                }
                else
                {
                    gameRoom.Mission.Map = xmlDocument;
                }
            }
			gameRoom.Core.Name = Query.Attributes["room_name"].InnerText;
			gameRoom.Core.RoomType = (byte)((User.Channel.ChannelType == "pve") ? 1 : 2);
			if (Query.Attributes["group_id"] != null)
			{
				User.Player.RoomPlayer.GroupId = Query.Attributes["group_id"].InnerText;
			}
			if (Query.Attributes["private"] != null)
			{
				gameRoom.Core.Private = (Query.Attributes["private"].InnerText == "1");
			}
			if (Query.Attributes["friendly_fire"] != null)
			{
				gameRoom.CustomParams.FriendlyFire = (Query.Attributes["friendly_fire"].InnerText == "1");
			}
			if (Query.Attributes["enemy_outlines"] != null)
			{
				gameRoom.CustomParams.EmenyOutlines = (Query.Attributes["enemy_outlines"].InnerText == "1");
			}
			if (Query.Attributes["auto_team_balance"] != null)
			{
				gameRoom.CustomParams.AutoTeamBalance = (Query.Attributes["auto_team_balance"].InnerText == "1");
			}
			if (Query.Attributes["dead_can_chat"] != null)
			{
				gameRoom.CustomParams.DeadCanChat = (Query.Attributes["dead_can_chat"].InnerText == "1");
			}
			if (Query.Attributes["join_in_the_process"] != null)
			{
				gameRoom.CustomParams.JoinInProcess = (Query.Attributes["join_in_the_process"].InnerText == "1");
			}
			if (Query.Attributes["max_players"] != null)
			{
				gameRoom.CustomParams.MaxPlayers = (byte)((User.Channel.ChannelType == "pve") ? 5 : byte.Parse(Query.Attributes["max_players"].InnerText));
			}
			if (Query.Attributes["inventory_slot"] != null)
			{
				gameRoom.CustomParams.InventorySlot = int.Parse(Query.Attributes["inventory_slot"].InnerText);
			}
			if (Query.Attributes["class_rifleman"] != null)
			{
				gameRoom.CustomParams.SoldierEnabled = (Query["class_rifleman"].Attributes["enabled"].InnerText == "1");
			}
			if (Query.Attributes["class_medic"] != null)
			{
				gameRoom.CustomParams.MedicEnabled = (Query["class_medic"].Attributes["enabled"].InnerText == "1");
			}
			if (Query.Attributes["class_engineer"] != null)
			{
				gameRoom.CustomParams.EngineerEnabled = (Query["class_engineer"].Attributes["enabled"].InnerText == "1");
			}
			if (Query.Attributes["class_sniper"] != null)
			{
				gameRoom.CustomParams.SniperEnabled = (Query["class_sniper"].Attributes["enabled"].InnerText == "1");
			}
			User.Player.RoomPlayer.Room = gameRoom;
			User.Player.RoomPlayer.TeamId = Teams.WARFACE;
			User.Channel.GameRoomList.Add(gameRoom);
			gameRoom.Players.Users.Add(User);
			Program.WriteLine($"Игрок {User.Player.Nickname} создал комнату. Карта: {gameRoom.Mission.Map}, ID: {gameRoom.Core.RoomId}", ConsoleColor.Yellow);
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
			XElement xElement3 = new XElement("gameroom_open");
			if (Code != 0)
			{
				XElement xElement4 = new XElement((XNamespace)"urn:ietf:params:xml:ns:xmpp-stanzas" + "error");
				xElement4.Add(new XAttribute("type", "continue"));
				xElement4.Add(new XAttribute("code", 8));
				xElement4.Add(new XAttribute("custom_code", Code));
				xElement.Add(xElement4);
			}
			else
			{
				xElement3.Add(User.Player.RoomPlayer.Room.Serialize());
			}
			xElement2.Add(xElement3);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			if (Code == 0)
			{
				Compress(ref xDocument);
			}
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
