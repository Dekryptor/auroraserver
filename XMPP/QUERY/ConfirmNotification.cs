using System;
using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.CLASSES.CLAN;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class ConfirmNotification : Stanza
	{
		private string Channel;

		public ConfirmNotification(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
			foreach (XmlNode childNode in Query.ChildNodes)
			{
				foreach (XmlNode childNode2 in User.Player.Notifications["notifications"].ChildNodes)
				{
					if (childNode2.Attributes["id"].InnerText == childNode.Attributes["id"].InnerText)
					{
						if (childNode2.Attributes["type"].InnerText == "16")
						{
							string ReceivedUser = childNode2["invitation"].Attributes["initiator"].InnerText;
							Client client = ArrayList.OnlineUsers.Find((Client Attribute) => Attribute.Player.Nickname == ReceivedUser);
							if (client != null)
							{
								if (childNode["confirmation"].Attributes["result"].InnerText == "0")
								{
									User.Player.ClanPlayer.Clan = client.Player.ClanPlayer.Clan;
									User.Player.ClanPlayer.ClanRole = Clan.CLAN_ROLE.DEFAULT;
									User.Player.ClanPlayer.InvitationDate = DateTimeOffset.Now.ToUnixTimeSeconds();
									client.Player.ClanPlayer.Clan.ProfileIds.Add(User.Player.UserID);
									client.Player.ClanPlayer.Clan.AddMember(User.Player.UserID, User.Player.Nickname, User.Player.Experience);
									client.Player.AddFriendResultNotification(User.Player.UserID, User.JID, User.Player.Nickname, User.Status, childNode["confirmation"].Attributes["location"].InnerText, User.Player.Experience, childNode["confirmation"].Attributes["result"].InnerText, isClan: true);
									new SyncNotification(client).Process();
									User.Player.Save();
								}
								else
								{
									client.Player.AddFriendResultNotification(User.Player.UserID, User.JID, User.Player.Nickname, User.Status, childNode["confirmation"].Attributes["location"].InnerText, User.Player.Experience, childNode["confirmation"].Attributes["result"].InnerText, isClan: true);
									new SyncNotification(client).Process();
								}
							}
						}
						if (childNode2.Attributes["type"].InnerText == "64")
						{
							string ReceivedUser2 = childNode2["invitation"].Attributes["initiator"].InnerText;
							Client client2 = ArrayList.OnlineUsers.Find((Client Attribute) => Attribute.Player.Nickname == ReceivedUser2);
							if (client2 != null)
							{
								if (childNode["confirmation"].Attributes["result"].InnerText == "0")
								{
									User.Player.AddFriend(client2.Player.UserID.ToString());
									client2.Player.AddFriend(User.Player.UserID.ToString());
									User.Player.AddFriendResultNotification(client2.Player.UserID, client2.JID, client2.Player.Nickname, client2.Status, client2.Location, User.Player.Experience, childNode["confirmation"].Attributes["result"].InnerText);
									new SyncNotification(User).Process();
								}
								client2.Player.AddFriendResultNotification(User.Player.UserID, User.JID, User.Player.Nickname, User.Status, childNode["confirmation"].Attributes["location"].InnerText, User.Player.Experience, childNode["confirmation"].Attributes["result"].InnerText);
								new SyncNotification(client2).Process();
							}
							else
							{
								Player player = new Player
								{
									Nickname = ReceivedUser2
								};
								player.Load();
								if (childNode["confirmation"].Attributes["result"].InnerText == "0")
								{
									User.Player.AddFriend(player.UserID.ToString());
									player.AddFriend(User.Player.UserID.ToString());
									User.Player.AddFriendResultNotification(player.UserID, "", player.Nickname, 0, "", player.Experience, childNode["confirmation"].Attributes["result"].InnerText);
									new SyncNotification(User).Process();
								}
								player.AddFriendResultNotification(User.Player.UserID, User.JID, User.Player.Nickname, User.Status, childNode["confirmation"].Attributes["location"].InnerText, User.Player.Experience, childNode["confirmation"].Attributes["result"].InnerText);
								player.Save();
							}
						}
						User.Player.Notifications["notifications"].RemoveChild(childNode2);
						User.Player.Save();
					}
				}
			}
			User.Player.Save();
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
			XElement content = new XElement("confirm_notification");
			xElement2.Add(content);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			Compress(ref xDocument);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
