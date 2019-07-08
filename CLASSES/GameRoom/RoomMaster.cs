using System.Xml.Linq;

namespace AuroraServer.CLASSES.GAMEROOM.CORE
{
	internal class RoomMaster
	{
		internal long UserId;

		internal int Revision = 2;

		internal XElement Serialize()
		{
			XElement xElement = new XElement("room_master");
			xElement.Add(new XAttribute("master", UserId));
			xElement.Add(new XAttribute("revision", Revision));
			return xElement;
		}
	}
}
