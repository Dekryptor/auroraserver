using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class GetPlayerStats : Stanza
	{
		private string Channel;

		public GetPlayerStats(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
            try
            {
                if (!(Type == "result"))
                {
                    Process();
                }
            }
            catch{}
		}

		internal override void Process()
		{
			XDocument xDocument = new XDocument();
			XElement xElement = new XElement(Gateway.JabberNS + "iq");
			xElement.Add(new XAttribute("type", (Type == "get") ? "result" : "get"));
			xElement.Add(new XAttribute("from", (Packet != null) ? To : "k01.warface"));
			xElement.Add(new XAttribute("to", User.JID));
			xElement.Add(new XAttribute("id", (Type == "get") ? Id : ("uid" + User.Player.Random.Next(9999, int.MaxValue).ToString("x8"))));
			XElement xElement2 = new XElement(Stanza.NameSpace + "query");
			XElement xElement3 = new XElement("get_player_stats");
			if (User.Player.Stats.FirstChild.ChildNodes.Count > 0)
			{
				foreach (XmlNode childNode in User.Player.Stats.FirstChild.ChildNodes)
				{
					xElement3.Add(XDocument.Parse(childNode.OuterXml).Root);
				}
			}
			xElement2.Add(xElement3);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			Compress(ref xDocument);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
			if (Type == null)
			{
				User.Player.Save();
			}
		}
	}
}
