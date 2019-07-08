using System.Collections.Generic;
using System.Xml.Linq;

namespace AuroraServer.CLASSES.GAMEROOM.CORE
{
	internal class Players
	{
		internal List<Client> Users = new List<Client>();

		internal XElement Serialize()
		{
			XElement xElement = new XElement("players");
			Client[] array = Users.ToArray();
			foreach (Client client in array)
			{
				xElement.Add(client.ToElement(isWarface: false));
			}
			return xElement;
		}
	}
}
