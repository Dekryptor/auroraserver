using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class GameRoom_Sync : Stanza
	{
		private string Channel;

		public GameRoom_Sync(Client User, XmlDocument Packet = null)
			: base(User, Packet)
		{
		}

		internal override void Process()
		{
			if (User.Player.RoomPlayer.Room != null)
			{
				XDocument xDocument = new XDocument();
				XElement xElement = new XElement(Gateway.JabberNS + "iq");
				xElement.Add(new XAttribute("type", (Type == "get") ? "result" : "get"));
				xElement.Add(new XAttribute("from", "k01.warface"));
				xElement.Add(new XAttribute("to", User.JID));
				xElement.Add(new XAttribute("id", (Type == "get") ? Id : User.Player.Random.Next(99999, int.MaxValue).ToString()));
				XElement xElement2 = new XElement(Stanza.NameSpace + "query");
				XElement xElement3 = new XElement("gameroom_sync");
				xElement3.Add(User.Player.RoomPlayer.Room.Serialize());
				xElement2.Add(xElement3);
				xElement.Add(xElement2);
				xDocument.Add(xElement);
				Compress(ref xDocument);
				User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
			}
		}
	}
}
