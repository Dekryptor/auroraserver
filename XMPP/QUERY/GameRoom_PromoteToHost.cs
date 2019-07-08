using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.CLASSES.GAMEROOM;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class GameRoom_PromoteToHost : Stanza
	{
		private GameRoom Room;

		public GameRoom_PromoteToHost(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
			Room = User.Player.RoomPlayer.Room;
			long userId = long.Parse(Query.Attributes["new_host_profile_id"].InnerText);
			Room.RoomMaster.UserId = userId;
			Room.RoomMaster.Revision++;
			if ((bool)App.Default["UseOldMode"])
			{
				Room.Core.Revision++;
			}
			Room.Sync(User);
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
			XElement xElement3 = new XElement("gameroom_promote_to_host");
			xElement3.Add(User.Player.RoomPlayer.Room.Serialize());
			xElement2.Add(xElement3);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
