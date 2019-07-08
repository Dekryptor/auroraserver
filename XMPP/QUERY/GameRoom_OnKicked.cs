using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.CLASSES.GAMEROOM;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class GameRoom_OnKicked : Stanza
	{
		internal enum Reason
		{
			KickedByUser = 1,
			NonActivity = 2,
			KickedByVoting = 3,
			KickedByRank = 6,
			KickedClan = 7,
			SystemSecurityViolation = 8,
			VersionMismatch = 9,
			NoAccessPoints = 10
		}

		private string Channel;

		private GameRoom Room;

		private Client Target;

		internal Reason KickReason;

		public GameRoom_OnKicked(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
		}

		public GameRoom_OnKicked(Client User, Reason Reason = Reason.KickedByUser)
			: base(User, null)
		{
			Room = User.Player.RoomPlayer.Room;
			KickReason = Reason;
			Room.KickedUsers.Add(User.Player.UserID);
			Room.Players.Users.RemoveAll((Client a) => a == User);
			User.Player.RoomPlayer.Room = null;
			if (Room.CustomParams.AutoTeamBalance)
			{
				Room.AutoBalanceProcess();
			}
			else
			{
				Room.Sync();
			}
			Process();
		}

		internal override void Process()
		{
			XDocument xDocument = new XDocument();
			XElement xElement = new XElement(Gateway.JabberNS + "iq");
			xElement.Add(new XAttribute("type", "get"));
			xElement.Add(new XAttribute("from", "k01.warface"));
			xElement.Add(new XAttribute("to", User.JID));
			xElement.Add(new XAttribute("id", User.Player.Random.Next(1, int.MaxValue)));
			XElement xElement2 = new XElement(Stanza.NameSpace + "query");
			XElement xElement3 = new XElement("gameroom_on_kicked");
			xElement3.Add(new XAttribute("reason", (byte)KickReason));
			xElement2.Add(xElement3);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
