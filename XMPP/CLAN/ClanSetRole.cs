using System;
using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.CLASSES.CLAN;
using AuroraServer.EXCEPTION;

namespace AuroraServer.XMPP.QUERY
{
	internal class ClanSetRole : Stanza
	{
		public ClanSetRole(Client User, XmlDocument Packet = null) : base(User, Packet)
		{
			bool flag = !(this.Type == "result");
			if (flag)
			{
				long Pid = long.Parse(this.Query.Attributes["profile_id"].InnerText);
				Clan.CLAN_ROLE cLAN_ROLE = (Clan.CLAN_ROLE)byte.Parse(this.Query.Attributes["role"].InnerText);
				bool flag2 = !Enum.IsDefined(typeof(Clan.CLAN_ROLE), cLAN_ROLE) || User.Player.ClanPlayer.Clan == null || User.Player.ClanPlayer.ClanRole != Clan.CLAN_ROLE.LEADER;
				if (flag2)
				{
					throw new StanzaException(User, Packet, 7);
				}
				Client client = ArrayList.OnlineUsers.Find((Client Attribute) => Attribute.Player.UserID == Pid);
				client.Player.ClanPlayer.ClanRole = cLAN_ROLE;
				bool flag3 = cLAN_ROLE == Clan.CLAN_ROLE.LEADER;
				if (flag3)
				{
					User.Player.ClanPlayer.ClanRole = Clan.CLAN_ROLE.CO_LEADER;
					User.Player.ClanPlayer.Clan.MasterBadge = client.Player.BannerBadge;
					User.Player.ClanPlayer.Clan.MasterMark = client.Player.BannerMark;
					User.Player.ClanPlayer.Clan.MasterStripe = client.Player.BannerStripe;
					User.Player.ClanPlayer.Clan.MasterNick = client.Player.Nickname;
				}
				new ClanMemberUpdated(client);
				new ClanMemberUpdated(User);
				switch (cLAN_ROLE)
				{
				case Clan.CLAN_ROLE.LEADER:
					client.ShowMessage("@clans_you_are_promoted_to_master", false);
					break;
				case Clan.CLAN_ROLE.CO_LEADER:
					client.ShowMessage("@clans_you_are_promoted_to_officer", false);
					break;
				case Clan.CLAN_ROLE.DEFAULT:
					client.ShowMessage("@clans_you_are_demoted_to_regular", false);
					break;
				}
				Process();
			}
		}

		public ClanSetRole(Client User) : base(User, null)
		{
		}

		internal override void Process()
		{
			bool flag = !(this.Type == "result");
			if (flag)
			{
				XDocument xDocument = new XDocument();
				XElement xElement = new XElement("iq");
				xElement.Add(new XAttribute("type", (this.Type == "get") ? "result" : "get"));
				xElement.Add(new XAttribute("from", "k01.warface"));
				xElement.Add(new XAttribute("to", this.User.JID));
				xElement.Add(new XAttribute("id", (this.Type == "get") ? this.Id : ("uid" + this.User.Player.Random.Next(9999, int.MaxValue).ToString("x8"))));
				XElement content = new XElement(Stanza.NameSpace + "query", new XElement("clan_set_member_role"));
				xElement.Add(content);
				xDocument.Add(xElement);
				User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
			}
		}
	}
}
