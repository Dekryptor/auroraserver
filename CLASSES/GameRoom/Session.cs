using System;
using System.Xml.Linq;

namespace AuroraServer.CLASSES.GAMEROOM.CORE
{
	internal class Session
	{
		internal byte Status;

		internal byte GameProcess;

		internal DateTime StartTime = DateTime.Now;

		internal int Revision = 1;

		internal long ID = 1L;

		internal XElement Serialize()
		{
			XElement xElement = new XElement("session");
			xElement.Add(new XAttribute("id", ID));
			xElement.Add(new XAttribute("status", Status));
			xElement.Add(new XAttribute("game_progress", GameProcess));
			xElement.Add(new XAttribute("start_time", StartTime));
			xElement.Add(new XAttribute("revision", Revision));
			return xElement;
		}
	}
}
