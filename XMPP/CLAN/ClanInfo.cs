using System;
using System.Drawing;
using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;

namespace AuroraServer.XMPP.QUERY
{
    internal class ClanInfo : Stanza
	{
        private string Channel;
        internal ClanInfo(Client User, XmlDocument Packet = null) : base(User, Packet)
		{

		}
        internal ClanInfo(Client User) : base(User, null)
		{
			Process();
		}

        internal override void Process()
		{
			if (Type == "result")
			{
				XDocument xDocument = new XDocument();
				XElement xElement = new XElement("iq");
				xElement.Add(new XAttribute("type", (Type == "get") ? "result" : "get"));
				xElement.Add(new XAttribute("from", "k01.warface"));
				xElement.Add(new XAttribute("to", User.JID));
				xElement.Add(new XAttribute("id", (Type == "get") ? this.Id : ("uid" + User.Player.Random.Next(9999, int.MaxValue).ToString("x8"))));
				XElement xElement2 = new XElement(NameSpace + "query");
				XElement xElement3 = new XElement("clan_info");
				if (User.Player.ClanPlayer.Clan != null)
				{
					xElement3.Add(User.Player.ClanPlayer.Clan.Serialize());
				}
				xElement2.Add(xElement3);
				xElement.Add(xElement2);
				xDocument.Add(xElement);
				//Console.WriteLine(string.Format("[{0}][{1}]", base.GetType().Name, xDocument));
				this.User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
			}
		}
	}
}
