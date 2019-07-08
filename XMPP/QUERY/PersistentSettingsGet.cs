using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class PersistentSettingsGet : Stanza
	{
		private string Channel;

		public PersistentSettingsGet(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
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
			XElement xElement3 = new XElement("persistent_settings_get");
			if (User.Player.Settings.ChildNodes.Count != 0)
			{
				if ((bool)App.Default["UseOldMode"])
				{
					foreach (XmlNode childNode in User.Player.Settings["settings"].ChildNodes)
					{
						xElement3.Add(XDocument.Parse((childNode.InnerXml == "") ? childNode.OuterXml : childNode.InnerXml).Root);
					}
				}
				else
				{
					foreach (XmlNode childNode2 in User.Player.Settings["settings"].ChildNodes)
					{
						xElement3.Add(XDocument.Parse(childNode2.OuterXml).Root);
					}
				}
			}
			xElement2.Add(xElement3);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			Compress(ref xDocument);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
