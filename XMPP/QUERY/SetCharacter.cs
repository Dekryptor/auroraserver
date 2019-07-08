using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class SetCharacter : Stanza
	{
		private string Channel;

		public SetCharacter(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
			try
			{
				foreach (XmlNode childNode in Query.ChildNodes)
				{
					foreach (Item item in User.Player.Items)
					{
						if (item.ID.ToString() == childNode.Attributes["id"].InnerText)
						{
							item.Slot = int.Parse(childNode.Attributes["slot"].InnerText);
							item.Equipped = Item.EquipperCalc(item.Slot);
							item.Config = childNode.Attributes["config"].InnerText;
							item.AttachedTo = childNode.Attributes["attached_to"].InnerText;
						}
					}
				}
				User.Player.Save();
			}
			catch (Exception ex)
			{
				File.WriteAllText(ex.ToString(), "SAVERROR.TXT");
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
			XElement xElement2 = new XElement(Stanza.NameSpace + "query");
			XElement content = new XElement("setcharacter");
			XElement xElement3 = new XElement((XNamespace)"urn:ietf:params:xml:ns:xmpp-stanzas" + "error");
			xElement3.Add(new XAttribute("type", "cancel"));
			xElement3.Add(new XAttribute("code", 8));
			xElement3.Add(new XAttribute("custom_code", 66));
			xElement2.Add(content);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
