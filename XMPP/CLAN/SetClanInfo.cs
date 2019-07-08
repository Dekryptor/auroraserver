using System;
using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;

namespace AuroraServer.XMPP.QUERY
{
	internal class SetClanInfo : Stanza
	{
		public SetClanInfo(Client User, XmlDocument Packet = null) : base(User, Packet)
		{
		}

		public SetClanInfo(Client User) : base(User, null)
		{
			Process();
		}

		internal override void Process()
		{
			if (Type != "result")
			{
				XDocument Packet = new XDocument();
				XElement iqElement = new XElement("iq");
				iqElement.Add(new XAttribute("type", (this.Type == "get") ? "result" : "get"));
				iqElement.Add(new XAttribute("from", "k01.warface"));
				iqElement.Add(new XAttribute("to", this.User.JID));
				iqElement.Add(new XAttribute("id", (this.Type == "get") ? this.Id : ("uid" + this.User.Player.Random.Next(9999, int.MaxValue).ToString("x8"))));
				XElement queryElement = new XElement(Stanza.NameSpace + "query");
				XElement clanInfo = new XElement("clan_info");
				bool flag2 = this.User.Player.ClanPlayer.Clan != null;
				if (flag2)
				{
					clanInfo.Add(this.User.Player.ClanPlayer.Clan.Serialize());
				}
				queryElement.Add(clanInfo);
				iqElement.Add(queryElement);
				Packet.Add(iqElement);
				this.User.Send(Packet.ToString(SaveOptions.DisableFormatting));
			}
		}
	}
}
