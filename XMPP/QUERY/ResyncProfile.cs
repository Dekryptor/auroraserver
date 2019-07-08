using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class ResyncProfile : Stanza
	{
		private string Channel;

		public ResyncProfile(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
			Process();
		}
		public ResyncProfile(Client User)
			: base(User, null)
		{
			Process();
		}

		internal override void Process()
		{
            if ((Type == "result")) return;
			
				XDocument xDocument = new XDocument();
				XElement xElement = new XElement(Gateway.JabberNS + "iq");
				xElement.Add(new XAttribute("type", (Type == "get") ? "result" : "get"));
				xElement.Add(new XAttribute("from",(Packet != null) ? To : "k01.warface"));
				xElement.Add(new XAttribute("to", User.JID));
				xElement.Add(new XAttribute("id", (Type == "get") ? Id : ("uid" + User.Player.Random.Next(9999, int.MaxValue).ToString("x8"))));



				XElement xElement2 = new XElement(Stanza.NameSpace + "query");
				XElement xElement3 = new XElement("resync_profile");
				foreach (Item item in User.Player.Items)
				{
					xElement3.Add(item.Serialize());
				}
				XElement xElement4 = new XElement("money");
				xElement4.Add(new XAttribute("cry_money", User.Player.CryMoney));
				xElement4.Add(new XAttribute("crown_money", User.Player.CrownMoney));
				xElement4.Add(new XAttribute("game_money", User.Player.GameMoney));
				XElement xElement5 = new XElement("character");
				xElement5.Add(new XAttribute("nick", User.Player.Nickname));
				xElement5.Add(new XAttribute("gender", User.Player.Gender));
				xElement5.Add(new XAttribute("height", User.Player.Height));
				xElement5.Add(new XAttribute("fatness", User.Player.Fatness));
				xElement5.Add(new XAttribute("head", User.Player.Head));
				xElement5.Add(new XAttribute("current_class", User.Player.CurrentClass));
				xElement5.Add(new XAttribute("experience", User.Player.Experience));
				xElement5.Add(new XAttribute("pvp_rating", "0"));
				xElement5.Add(new XAttribute("pvp_rating_points", "0"));
				xElement5.Add(new XAttribute("banner_badge", User.Player.BannerBadge));
				xElement5.Add(new XAttribute("banner_mark", User.Player.BannerMark));
				xElement5.Add(new XAttribute("banner_stripe", User.Player.BannerStripe));
				xElement5.Add(new XAttribute("game_money", User.Player.GameMoney));
				xElement5.Add(new XAttribute("cry_money", User.Player.CryMoney));
				xElement5.Add(new XAttribute("crown_money", User.Player.CrownMoney));
				XElement xElement6 = new XElement("profile_progression_state");
				xElement6.Add(new XAttribute("profile_id", User.Player.UserID));
				xElement6.Add(new XAttribute("mission_unlocked", User.Player.UnlockedMissions));
				xElement6.Add(new XAttribute("tutorial_unlocked", User.Player.TutorialSuggest));
				xElement6.Add(new XAttribute("tutorial_passed", User.Player.TutorialPassed));
				xElement6.Add(new XAttribute("class_unlocked", User.Player.UnlockedClasses));
				XElement xElement7 = new XElement("profile_progression_state");
				xElement7.Add(xElement6);
				xElement3.Add(xElement4);
				xElement3.Add(xElement5);
				xElement3.Add(xElement7);
				xElement2.Add(xElement3);
				xElement.Add(xElement2);
				xDocument.Add(xElement);
				Compress(ref xDocument);
				User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
			
		}
	}
}
