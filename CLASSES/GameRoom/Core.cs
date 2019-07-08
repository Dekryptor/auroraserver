using System.Xml.Linq;

namespace AuroraServer.CLASSES.GAMEROOM.CORE
{
	internal class Core
	{
		internal long RoomId = 1L;
		internal byte RoomType = 2;
		internal string Name = "NoNameYet";
		internal int TeamSwitched;
		internal bool Private;
		internal bool CanStart;
		internal bool TeamBalanced;
		internal byte MinReadyPlayers = 2;

		internal int Revision = 1;

		internal XElement Serialize(long Master = 0L, Players P = null)
		{
			Revision++;
			XElement xElement = new XElement("core");
			xElement.Add(new XAttribute("teams_switched", TeamSwitched));
			xElement.Add(new XAttribute("room_name", Name));
			xElement.Add(new XAttribute("private", Private ? 1 : 0));
			xElement.Add(new XAttribute("players", P.Users.Count));
			xElement.Add(new XAttribute("can_start", 1));
			xElement.Add(new XAttribute("team_balanced", 1));
			xElement.Add(new XAttribute("min_ready_players", MinReadyPlayers));
			xElement.Add(new XAttribute("master", Master));
			xElement.Add(new XAttribute("revision", Revision));
			return xElement;
		}
	}
}
