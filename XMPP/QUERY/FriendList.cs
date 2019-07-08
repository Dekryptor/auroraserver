using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;

namespace AuroraServer.XMPP.QUERY
{
	internal class FriendList : Stanza
	{
		private string Channel;

		public FriendList(Client User, XmlDocument Packet = null)
			: base(User, Packet)
		{
		}

		internal override void Process()
		{
			if (!(Type == "result"))
			{
				XDocument xDocument = new XDocument();
				XElement xElement = new XElement("iq");
				xElement.Add(new XAttribute("type", (Type == "get") ? "result" : "get"));
				xElement.Add(new XAttribute("from", "masterserver@warface/" + User.Channel.Resource));
				xElement.Add(new XAttribute("to", User.JID));
				xElement.Add(new XAttribute("id", (Type == "get") ? Id : ("uid" + User.Player.Random.Next(9999, int.MaxValue).ToString("x8"))));
				XElement xElement2 = new XElement(Stanza.NameSpace + "query");
				xElement2.Add(User.Player.Friends);
				xElement.Add(xElement2);
				xDocument.Add(xElement);
				Compress(ref xDocument);
				User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
			}
		}
	}
}
