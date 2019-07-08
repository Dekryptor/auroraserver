using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.EXCEPTION;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class ConsumeItem : Stanza
	{
		private string Channel;

		private Client Receiver;

		private Item PlayerItem;

		public ConsumeItem(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
			if (!User.Dedicated)
			{
				throw new StanzaException(User, Packet, 1006);
			}
			long ID = long.Parse(Query.Attributes["profile_id"].InnerText);
			long ItemId = int.Parse(Query.Attributes["item_profile_id"].InnerText);
			Receiver = ArrayList.OnlineUsers.Find((Client Attribute) => Attribute.Player.UserID == ID);
			if (Receiver != null)
			{
				PlayerItem = Receiver.Player.Items.Find((Item Attribute) => Attribute.ID == ItemId);
				PlayerItem.Quantity--;
			}
			User.Player.Save();
			Process();
		}

		internal override void Process()
		{
			XDocument xDocument = new XDocument();
			XElement xElement = new XElement(Gateway.JabberNS + "iq");
			xElement.Add(new XAttribute("type", "result"));
			xElement.Add(new XAttribute("from", "masterserver@warface/wartls"));
			xElement.Add(new XAttribute("to", User.JID));
			xElement.Add(new XAttribute("id", Id));
			XElement xElement2 = new XElement(Stanza.NameSpace + "query");
			XElement content = new XElement("consume_item", new XAttribute("profile_id", Receiver.Player.UserID), new XAttribute("item_profile_id", PlayerItem.ID), new XAttribute("items_consumed", 1), new XAttribute("items_left", PlayerItem.Quantity));
			xElement2.Add(content);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
