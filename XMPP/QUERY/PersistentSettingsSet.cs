using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class PersistentSettingsSet : Stanza
	{
		private string Channel;

		public PersistentSettingsSet(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
			XmlNode xmlNode = Query["settings"];
			foreach (XmlNode childNode in xmlNode.ChildNodes)
			{
				if (User.Player.Settings["settings"] == null)
				{
					User.Player.Settings.AppendChild(User.Player.Settings.ImportNode(xmlNode, deep: true));
				}
				if (User.Player.Settings["settings"][childNode.Name] == null)
				{
					User.Player.Settings["settings"].AppendChild(User.Player.Settings.ImportNode(xmlNode.FirstChild, deep: true));
				}
				else
				{
					foreach (XmlAttribute attribute in Query["settings"][childNode.Name].Attributes)
					{
						if (User.Player.Settings["settings"][childNode.Name].Attributes[attribute.Name] == null)
						{
							User.Player.Settings["settings"][childNode.Name].SetAttribute(attribute.Name, attribute.Value);
						}
						else
						{
							User.Player.Settings["settings"][childNode.Name].Attributes[attribute.Name].Value = attribute.Value;
						}
					}
				}
			}
			User.Player.Save();
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
			XElement content = new XElement("persistent_settings_set");
			xElement2.Add(content);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
