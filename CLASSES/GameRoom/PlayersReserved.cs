using System.Xml.Linq;

namespace AuroraServer.CLASSES.GAMEROOM.CORE
{
	internal class PlayersReserved
	{
		internal long UserId;

		internal int Revision = 2;

		internal XElement Serialize()
		{
			return new XElement("playersReserved");
		}
	}
}
