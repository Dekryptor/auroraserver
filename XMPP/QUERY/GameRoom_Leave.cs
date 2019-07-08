using System.Linq;
using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.CLASSES.GAMEROOM;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class GameRoom_Leave : Stanza
	{
		private GameRoom Room;

		public GameRoom_Leave(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
			Room = User.Player.RoomPlayer.Room;
			if (Room != null)
			{
				Room.Players.Users.RemoveAll((Client a) => a == User);
				User.Player.RoomPlayer.Room = null;
				if (Room.RoomMaster.UserId == User.Player.UserID)
				{
					Client[] array = (from Attribute in Room.Players.Users.ToList()
					orderby Attribute.Player.Experience
					select Attribute).ToArray();
					if (array.Length != 0)
					{
						Room.RoomMaster.Revision++;
						Room.RoomMaster.UserId = array[0].Player.UserID;
					}
				}
				if (Room.CustomParams.AutoTeamBalance)
				{
					Room.AutoBalanceProcess();
				}
				else
				{
					Room.Sync(User);
				}
			}
			if (base.Packet != null)
			{
				Process();
			}
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
			XElement content = new XElement("gameroom_leave");
			xElement2.Add(content);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
