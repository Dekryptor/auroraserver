using System.Linq;
using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.CLASSES.GAMEROOM;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class GameRoom_Kick : Stanza
	{
		private string Channel;

		private GameRoom Room;

		private Client Target;

		public GameRoom_Kick(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
			Room = User.Player.RoomPlayer.Room;
			long TargetId = long.Parse(Query.Attributes["target_id"].InnerText);
			Target = Room.Players.Users.ToList().Find((Client Attribute) => Attribute.Player.UserID == TargetId);
			new GameRoom_OnKicked(Target);
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
			XElement xElement3 = new XElement("gameroom_kick");
			xElement3.Add(Room.Serialize());
			xElement2.Add(xElement3);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			Compress(ref xDocument);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
