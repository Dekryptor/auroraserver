using System.Xml.Linq;

namespace AuroraServer.CLASSES.GAMEROOM.CORE
{
	internal class CustomParams
	{
		internal bool FriendlyFire;

		internal bool EmenyOutlines;

		internal bool AutoTeamBalance = true;

		internal bool DeadCanChat = true;

		internal bool JoinInProcess = true;

		internal byte MaxPlayers = 16;

		internal int InventorySlot;

		internal bool SoldierEnabled = true;

		internal bool MedicEnabled = true;

		internal bool EngineerEnabled = true;

		internal bool SniperEnabled = true;

		internal int Revision = 2;

		internal byte ClassRestriction
		{
			get
			{
				byte b = 224;
				if (SoldierEnabled)
				{
					b = (byte)(b + 1);
				}
				if (MedicEnabled)
				{
					b = (byte)(b + 8);
				}
				if (EngineerEnabled)
				{
					b = (byte)(b + 16);
				}
				if (SniperEnabled)
				{
					b = (byte)(b + 4);
				}
				return b;
			}
		}

		internal XElement Serialize()
		{
			XElement xElement = new XElement("custom_params");
			xElement.Add(new XAttribute("friendly_fire", FriendlyFire ? 1 : 0));
			xElement.Add(new XAttribute("enemy_outlines", EmenyOutlines ? 1 : 0));
			xElement.Add(new XAttribute("auto_team_balance", AutoTeamBalance ? 1 : 0));
			xElement.Add(new XAttribute("dead_can_chat", DeadCanChat ? 1 : 0));
			xElement.Add(new XAttribute("join_in_the_process", JoinInProcess ? 1 : 0));
			xElement.Add(new XAttribute("max_players", MaxPlayers));
			xElement.Add(new XAttribute("inventory_slot", InventorySlot));
			xElement.Add(new XAttribute("class_restriction", ClassRestriction));
			xElement.Add(new XAttribute("revision", Revision));
			return xElement;
		}
	}
}
