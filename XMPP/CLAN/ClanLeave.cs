using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.CLASSES.CLAN;
using AuroraServer.EXCEPTION;

namespace AuroraServer.XMPP.QUERY
{
	internal class ClanLeave : Stanza
	{
		public ClanLeave(Client User, XmlDocument Packet = null) : base(User, Packet)
		{
            Clan.ProfileIds.Remove(User.Player.UserID);
            /*if (Clan == null)
			{
				throw new StanzaException(User, Packet, 2);
			}
			else
			{*/
				if (User.Player.ClanPlayer.ClanRole == Clan.CLAN_ROLE.LEADER)
				{
					Client[] array = (from Attribute in User.Player.ClanPlayer.Clan.OnlineUsers
					orderby Attribute.Player.Experience
					select Attribute).ToArray<Client>();
					if (array.Length != 0)
					{
						array[0].Player.ClanPlayer.ClanRole = Clan.CLAN_ROLE.LEADER;
						new ClanMemberUpdated(array[0]);
						array[0].Player.ClanPlayer.Clan.MasterBadge = array[0].Player.BannerBadge;
						array[0].Player.ClanPlayer.Clan.MasterMark = array[0].Player.BannerMark;
						array[0].Player.ClanPlayer.Clan.MasterNick = array[0].Player.Nickname;
						array[0].Player.ClanPlayer.Clan.MasterStripe = array[0].Player.BannerMark;
						array[0].ShowMessage("@clans_you_are_promoted_to_master", false);
					}
				}
			//}
			User.Player.ClanPlayer.ClanRole = Clan.CLAN_ROLE.NOT_IN_CLAN;
			User.Player.ClanPlayer.Points = 0;
            if(Clan.ProfileIds.Count != 0)
            {
                User.Player.ClanPlayer.Clan.Save();
            }
			User.Player.ClanPlayer.Clan = null;
			foreach (Client onlineUser in Clan.OnlineUsers)
			{
				if (onlineUser.JID != null)
				{
					new ClanInfo(onlineUser);
				}
			}
			new ClanInfo(User);
			User.Player.Save();
			Process();
		}
		public ClanLeave(Client User) : base(User, null)
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
				XElement content = new XElement(NameSpace + "query", new XElement("clan_leave"));
				xElement.Add(content);
				xDocument.Add(xElement);
				User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
			}
		}

		private Clan Clan;
	}
}
