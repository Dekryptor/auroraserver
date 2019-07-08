using System.Linq;
using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;

namespace AuroraServer.XMPP.QUERY
{
	internal class SyncNotification : Stanza
	{
		private string Channel;

		private XElement FastNotify;

		public SyncNotification(Client User, XmlDocument Packet = null)
			: base(User, Packet)
		{
		}

		public SyncNotification(Client User, XElement FastNotify)
			: base(User, null)
		{
			this.FastNotify = FastNotify;
		}

		internal override void Process()
		{
			if (Type == "result")return;
			XDocument xDocument = new XDocument();
			XElement xElement = new XElement("iq");
			xElement.Add(new XAttribute("type", (Type == "get") ? "result" : "get"));
			xElement.Add(new XAttribute("from", "masterserver@warface/wartls"));
			xElement.Add(new XAttribute("to", User.JID));
			xElement.Add(new XAttribute("id", (Type == "get") ? Id : ("uid" + User.Player.Random.Next(9999, int.MaxValue).ToString("x8"))));

			XElement xElement2 = new XElement(Stanza.NameSpace + "query");
			XElement xElement3 = new XElement("sync_notifications");
			if (FastNotify == null)
			{
				if (User.Player.Notifications.FirstChild.ChildNodes.Count > 0)
				{
					foreach (XmlNode childNode in User.Player.Notifications.FirstChild.ChildNodes)
						xElement3.Add(XDocument.Parse(childNode.OuterXml).Root);
					
				}
			}
			else
			{
				xElement3.Add(FastNotify);
			}
			xElement2.Add(xElement3);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			(from e in xElement.Attributes()
			where e.IsNamespaceDeclaration
			select e).Remove();
			Compress(ref xDocument);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
