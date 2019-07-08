using System;
using System.Drawing;
using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;

namespace AuroraServer.XMPP.QUERY
{
	internal class ClanMemberUpdated : Stanza
	{
		public ClanMemberUpdated(Client User, XmlDocument Packet = null) : base(User, Packet)
		{
		}

		public ClanMemberUpdated(Client AboutOf) : base(null, null)
		{
			this.AboutOf = AboutOf;
			Process();
		}

		internal override void Process()
		{
			if (Type == "result")
			{
				foreach (Client onlineUser in this.AboutOf.Player.ClanPlayer.Clan.OnlineUsers)
				{
					if (onlineUser.JID != null)
					{
						XDocument xDocument = new XDocument();
						XElement xElement = new XElement("iq");
						xElement.Add(new XAttribute("type", (this.Type == "get") ? "result" : "get"));
						xElement.Add(new XAttribute("from", "k01.warface"));
						xElement.Add(new XAttribute("to", onlineUser.JID));
						xElement.Add(new XAttribute("id", (this.Type == "get") ? this.Id : ("uid" + onlineUser.Player.Random.Next(9999, int.MaxValue).ToString("x8"))));
						XElement content = new XElement(Stanza.NameSpace + "query", new XElement("clan_members_updated", new XElement("update", new object[]
						{
							new XAttribute("profile_id", this.AboutOf.Player.UserID),
							new XElement("clan_member_info", new object[]
							{
								new XAttribute("nickname", this.AboutOf.Player.Nickname),
								new XAttribute("profile_id", this.AboutOf.Player.UserID),
								new XAttribute("experience", this.AboutOf.Player.Experience),
								new XAttribute("clan_points", this.AboutOf.Player.ClanPlayer.Points),
								new XAttribute("invite_date", this.AboutOf.Player.ClanPlayer.InvitationDate),
								new XAttribute("clan_role", (int)this.AboutOf.Player.ClanPlayer.ClanRole),
								new XAttribute("status", this.AboutOf.Status),
								new XAttribute("jid", this.AboutOf.JID)
							})
						})));
						xElement.Add(content);
						xDocument.Add(xElement);
						onlineUser.Send(xDocument.ToString(SaveOptions.DisableFormatting));
					}
				}
				AboutOf.Player.ClanPlayer.Clan.Save();
			}
		}

		private Client AboutOf = null;
	}
}
