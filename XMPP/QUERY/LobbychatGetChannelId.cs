using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class LobbychatGetChannelId : Stanza
	{
		private string Channel;

		public LobbychatGetChannelId(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
			Channel = Query.Attributes["channel"].InnerText;
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
			string text = null;
			try
			{
				string channel = Channel;
				if (channel != null)
				{
					if (!(channel == "0"))
					{
						if (!(channel == "1"))
						{
							if (!(channel == "2"))
							{
								if (channel == "3")
								{
									text = $"clan.{User.Player.ClanPlayer.Clan.ID}";
								}
							}
							else
							{
								text = $"team.room.{User.Player.RoomPlayer.Room.Core.RoomId}";
							}
						}
						else
						{
							text = $"room.{User.Player.RoomPlayer.Room.Core.RoomId}";
						}
					}
					else
					{
						text = "global." + User.Channel.Resource;
					}
				}
			}
			catch
			{
				XElement xElement3 = new XElement("error");
				xElement3.Add(new XAttribute("type", "continue"));
				xElement3.Add(new XAttribute("code", 8));
				xElement3.Add(new XElement((XNamespace)"urn:ietf:params:xml:ns:xmpp-stanzas" + "internal-server-error"), new XElement((XNamespace)"urn:ietf:params:xml:ns:xmpp-stanzas" + "text", "Query processing error"));
			}
			XElement xElement4 = new XElement("lobbychat_getchannelid");
			xElement4.Add(new XAttribute("channel", Channel));
			if (text != null)
			{
				xElement4.Add(new XAttribute("channel_id", text));
			}
			xElement4.Add(new XAttribute("service_id", "conference.warface"));
			xElement2.Add(xElement4);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
