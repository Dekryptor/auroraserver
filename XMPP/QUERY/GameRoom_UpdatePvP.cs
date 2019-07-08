using System.Data;
using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.CLASSES.GAMEROOM;
using AuroraServer.EXCEPTION;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class GameRoom_UpdatePvP : Stanza
	{
		private string Channel;

		private int ErrorId = -1;

		public GameRoom_UpdatePvP(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
			if (SQL.Handler.State == ConnectionState.Closed)
			{
				SQL.Handler.Open();
			}
			GameRoom room = User.Player.RoomPlayer.Room;
			try
			{
				if (room.Mission.Map.FirstChild.Attributes["uid"].InnerText != Query.Attributes["mission_key"].InnerText)
				{
					XmlDocument xmlDocument = GameResources.Maps.Find((XmlDocument Attribute) => Attribute.FirstChild.Attributes["uid"].InnerText == Query.Attributes["mission_key"].InnerText);
					if (xmlDocument.FirstChild.Attributes["release_mission"].InnerText == "0")
					{
						new StanzaException(User, Packet, 1);
					}
					else
					{
						room.Mission.Map = xmlDocument;
						room.Mission.Revision++;
					}
				}
			}
			catch
			{
				ErrorId = 1;
				Process();
				return;
			}
			if (Query.Attributes["private"] != null)
			{
				room.Core.Private = (Query.Attributes["private"].InnerText == "1");
			}
			if (Query.Attributes["friendly_fire"] != null)
			{
				room.CustomParams.FriendlyFire = (Query.Attributes["friendly_fire"].InnerText == "1");
			}
			if (Query.Attributes["enemy_outlines"] != null)
			{
				room.CustomParams.EmenyOutlines = (Query.Attributes["enemy_outlines"].InnerText == "1");
			}
			if (Query.Attributes["auto_team_balance"] != null)
			{
				room.CustomParams.AutoTeamBalance = (Query.Attributes["auto_team_balance"].InnerText == "1");
			}
			if (Query.Attributes["dead_can_chat"] != null)
			{
				room.CustomParams.DeadCanChat = (Query.Attributes["dead_can_chat"].InnerText == "1");
			}
			if (Query.Attributes["join_in_the_process"] != null)
			{
				room.CustomParams.JoinInProcess = (Query.Attributes["join_in_the_process"].InnerText == "1");
			}
			if (Query.Attributes["max_players"] != null)
			{
				room.CustomParams.MaxPlayers = byte.Parse(Query.Attributes["max_players"].InnerText);
			}
			if (Query.Attributes["inventory_slot"] != null)
			{
				room.CustomParams.InventorySlot = int.Parse(Query.Attributes["inventory_slot"].InnerText);
			}
			if (Query["class_rifleman"] != null)
			{
				room.CustomParams.SoldierEnabled = (Query["class_rifleman"].Attributes["enabled"].InnerText == "1");
			}
			if (Query["class_medic"] != null)
			{
				room.CustomParams.MedicEnabled = (Query["class_medic"].Attributes["enabled"].InnerText == "1");
			}
			if (Query["class_engineer"] != null)
			{
				room.CustomParams.EngineerEnabled = (Query["class_engineer"].Attributes["enabled"].InnerText == "1");
			}
			if (Query["class_sniper"] != null)
			{
				room.CustomParams.SniperEnabled = (Query["class_sniper"].Attributes["enabled"].InnerText == "1");
			}
			room.CustomParams.Revision++;
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
			if (room.CustomParams.AutoTeamBalance)
			{
				room.AutoBalanceProcess();
			}
			else
			{
				room.Sync(User);
			}
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
			XElement xElement3 = new XElement("gameroom_update_pvp");
			xElement3.Add(User.Player.RoomPlayer.Room.Serialize());
			xElement2.Add(xElement3);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			Compress(ref xDocument);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
