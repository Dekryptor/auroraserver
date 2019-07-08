using System.Linq;
using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class GetExpiredItems : Stanza
	{
		public GetExpiredItems(Client User, XmlDocument Packet)
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
			XElement xElement3 = new XElement(Stanza.NameSpace + "get_expired_items");
			foreach (Item item in User.Player.Items)
			{
				if (item.ItemType == ItemType.CONSUMABLE)
				{
					xElement3.Add(new XElement("consumable_item", from attr in item.Serialize().Attributes()
					select new XAttribute(attr)));
				}
				if (item.ItemType == ItemType.PERMANENT)
				{
					xElement3.Add(new XElement("durability_item", from attr in item.Serialize().Attributes()
					select new XAttribute(attr)));
				}
			}
			xElement2.Add(xElement3);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
