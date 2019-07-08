using System;
using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class GetLastSeenDate : Stanza
	{
		private string LastSeen = "1";

		private string ProfileId;

		public GetLastSeenDate(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
			Player player = null;
			ProfileId = (((bool)App.Default["UseOldMode"]) ? Query.Attributes["nickname"].InnerText : Query.Attributes["profile_id"].InnerText);
			long num = ((bool)App.Default["UseOldMode"]) ? (-1) : long.Parse(ProfileId);
			player = ((num <= 0) ? new Player
			{
				Nickname = ProfileId
			} : new Player
			{
				UserID = num
			});
			if (!player.Load())
			{
				Process();
				return;
			}
			LastSeen = ((DateTimeOffset)player.LastSeen).ToUnixTimeSeconds().ToString();
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
			XElement xElement3 = new XElement("get_last_seen_date");
			xElement3.Add(new XAttribute("profile_id", ProfileId));
			xElement3.Add(new XAttribute("last_seen", LastSeen));
			xElement2.Add(xElement3);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
