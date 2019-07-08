using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class RemoveFriend : Stanza
	{
		private string Target;

		public RemoveFriend(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
			Target = Query.Attributes["target"].InnerText;
			Client client = ArrayList.OnlineUsers.Find((Client Attribute) => Attribute.Player.Nickname == Target);
			if (client != null)
			{
				User.Player.RemoveFriend(client.Player.UserID.ToString());
				client.Player.RemoveFriend(User.Player.UserID.ToString());
				client.Player.Save();
				new FriendList(client).Process();
			}
			else
			{
				Player player = new Player
				{
					Nickname = Target
				};
				if (!player.Load())
				{
					Process();
					return;
				}
				User.Player.RemoveFriend(player.UserID.ToString());
				player.RemoveFriend(User.Player.UserID.ToString());
				player.Save();
			}
			new FriendList(User).Process();
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
			XElement content = new XElement("remove_friend");
			xElement2.Add(content);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			Compress(ref xDocument);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
