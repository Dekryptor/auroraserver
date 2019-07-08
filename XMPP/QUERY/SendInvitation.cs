using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.CLASSES.CLAN;
using AuroraServer.EXCEPTION;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class SendInvitation : Stanza
	{
		private enum Error
		{
			SUCCESSFILY_SENDED = 3,
			REJECTED = 1,
			IN_PROGRESS = 2,
			ALREADY_IN_FRIEND = 4,
			ALREADY_IN_CLAN = 5,
			NOT_PERMISSIONS = 6,
			KICK_TIMEOUT = 7,
			USER_OFFLINE = 8,
			USER_NOT_FOUND = 9,
			USER_NOT_FOUND_IN_LOBBY = 10,
			LIMIT_REACHED = 11,
			FRIEND_LIMIT_REACHED = 12,
			TIMEOUT = 14,
			ENABLED_DND = 0xF
		}

		private string Target;

		private Error ErrorId = Error.SUCCESSFILY_SENDED;

		public SendInvitation(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
			if (Query.Attributes["type"].InnerText == "64")
			{
				Target = Query.Attributes["target"].InnerText;
			}
			else
			{
				if (User.Player.ClanPlayer.ClanRole == Clan.CLAN_ROLE.DEFAULT || User.Player.ClanPlayer.ClanRole == Clan.CLAN_ROLE.NOT_IN_CLAN)
				{
					throw new StanzaException(User, Packet, 6);
				}
				if (User.Player.ClanPlayer.Clan.UserProperties.Count >= 50)
				{
					throw new StanzaException(User, Packet, 11);
				}
				Target = Client.ResolveNickname(long.Parse(Query.Attributes["target_id"].InnerText));
			}
			if (Target == User.Player.Nickname)
			{
				return;
			}
			Client client = ArrayList.OnlineUsers.Find((Client Attribute) => Attribute.Player.Nickname == Target);
			if (client != null)
			{
				if (Query.Attributes["type"].InnerText == "16" && client.Player.ClanPlayer.ClanRole != 0)
				{
					throw new StanzaException(User, Packet, 5);
				}
				if (Query.Attributes["type"].InnerText == "64")
				{
					foreach (XmlNode childNode in User.Player.friends["friends"].ChildNodes)
					{
						if (childNode.InnerText == client.Player.UserID.ToString())
						{
							ErrorId = Error.ALREADY_IN_FRIEND;
							Process();
							return;
						}
					}
				}
				foreach (XmlNode childNode2 in client.Player.Notifications["notifications"].ChildNodes)
				{
					if (childNode2.Attributes["type"].InnerText == "64" && childNode2["invitation"].Attributes["initiator"].InnerText == User.Player.Nickname)
					{
						ErrorId = Error.IN_PROGRESS;
						Process();
						return;
					}
				}
				if (Query.Attributes["type"].InnerText == "16")
				{
					client.Player.AddFriendNotification(User.Player.Nickname, Query.Attributes["type"].InnerText == "16", User.Player.ClanPlayer.Clan.ID, User.Player.ClanPlayer.Clan.Name);
				}
				else
				{
					client.Player.AddFriendNotification(User.Player.Nickname, Query.Attributes["type"].InnerText == "16", 0L, null);
				}
				client.Player.Save();
				new SyncNotification(client).Process();
			}
			else
			{
				if (Query.Attributes["type"].InnerText == "16")
				{
                    throw new StanzaException(User, Packet, 8);
                }
				Player player = new Player
				{
					Nickname = Target
				};
				if (!player.Load())
				{
					ErrorId = Error.USER_NOT_FOUND;
					Process();
					return;
				}
				foreach (XmlNode childNode3 in User.Player.friends["friends"].ChildNodes)
				{
					if (childNode3.InnerText == player.UserID.ToString())
					{
						ErrorId = Error.ALREADY_IN_FRIEND;
						Process();
						return;
					}
				}
				foreach (XmlNode childNode4 in player.Notifications["notifications"].ChildNodes)
				{
					if (childNode4.Attributes["type"].InnerText == "64" && childNode4["invitation"].Attributes["initiator"].InnerText == User.Player.Nickname)
					{
						ErrorId = Error.IN_PROGRESS;
						Process();
						return;
					}
				}
				player.AddFriendNotification(User.Player.Nickname, Query.Attributes["type"].InnerText == "16", 0L);
				player.Save();
			}
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
			XElement xElement2 = new XElement(NameSpace + "query");
			XElement content = new XElement("send_invitation", new XAttribute("type", Query.Attributes["type"].InnerText), new XAttribute("target", Target));
			xElement2.Add(content);
			if (ErrorId != Error.SUCCESSFILY_SENDED)
			{
				XElement xElement3 = new XElement((XNamespace)"urn:ietf:params:xml:ns:xmpp-stanzas" + "error");
				xElement3.Add(new XAttribute("type", "continue"));
				xElement3.Add(new XAttribute("code", 8));
				xElement3.Add(new XAttribute("custom_code", (int)ErrorId));
				xElement.Add(xElement3);
			}
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
