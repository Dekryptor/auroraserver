using System;
using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.CLASSES.CLAN;
using AuroraServer.EXCEPTION;

namespace AuroraServer.XMPP.QUERY
{
	internal class ClanKick : Stanza
	{
		public ClanKick(Client User, XmlDocument Packet = null) : base(User, Packet)
		{
			if (User.Player.ClanPlayer.Clan == null || User.Player.ClanPlayer.ClanRole != Clan.CLAN_ROLE.LEADER)
			{
				throw new StanzaException(User, Packet, 4);
			}
			pid = long.Parse(Query.Attributes["profile_id"].InnerText);
			Client client = ArrayList.OnlineUsers.Find((Client Attribute) => Attribute.Player.UserID == pid);
			if (!User.Player.ClanPlayer.Clan.ProfileIds.Contains(pid))
			{
				throw new StanzaException(User, Packet, 4);
			}
			User.Player.ClanPlayer.Clan.ProfileIds.Remove(pid);
			if (client != null)
			{
				client.Player.ClanPlayer.Points = 0;
				client.Player.ClanPlayer.Clan = null;
				new ClanInfo(client);
				client.ShowMessage("@clans_you_was_kicked", false);
				foreach (Client onlineUser in User.Player.ClanPlayer.Clan.OnlineUsers)
				{
					new ClanInfo(onlineUser);
				}
			}
			else
			{
				Player player = new Player
				{
					UserID = pid
                };
				if (!player.Load(true))
				{
					throw new StanzaException(User, Packet, 4);
				}
				player.ClanPlayer.ClanRole = Clan.CLAN_ROLE.NOT_IN_CLAN;
				player.ClanPlayer.Clan = null;
				player.ClanPlayer.InvitationDate = 0L;
				player.ClanPlayer.Points = 0;
				player.AddCustomMessage("@clans_you_was_kicked");
				player.Save();
			}
			User.Player.ClanPlayer.Clan.Save();
			Process();
		}

		public ClanKick(Client User) : base(User, null)
		{
		}

        internal override void Process()
		{
			if (Type == "result")
			{
				XDocument xDocument = new XDocument();
				XElement xElement = new XElement("iq");
				xElement.Add(new XAttribute("type", (Type == "get") ? "result" : "get"));
				xElement.Add(new XAttribute("from", To));
				xElement.Add(new XAttribute("to", User.JID));
				xElement.Add(new XAttribute("id", (Type == "get") ? Id : ("uid" + User.Player.Random.Next(9999, int.MaxValue).ToString("x8"))));
				XElement content = new XElement(Stanza.NameSpace + "query", new XElement("clan_kick"));
				xElement.Add(content);
				xDocument.Add(xElement);
				User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
				new ClanInfo(User);
			}
		}

		private long pid = 0L;
	}
}
