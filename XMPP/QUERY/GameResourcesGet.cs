using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class GameResourcesGet : Stanza
	{
		private int From;

		private int Left;

		private XmlDocument Selected;

		public GameResourcesGet(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
			From = int.Parse((Query.Attributes["from"] == null) ? "0" : Query.Attributes["from"].InnerText);
			int index = From / 250;
			if (Query.Name == "items")
			{
				Selected = GameResources.ItemsSplited[index];
				if ((bool)App.Default["UseOldMode"])
				{
					Selected = GameResources.ItemsSplited[int.Parse(Query.Attributes["received"].InnerText) / 250];
					Left = GameResources.ItemsSplited.Count - int.Parse(Query.Attributes["received"].InnerText) / 250 - 1;
				}
			}
			if (Query.Name == "get_configs")
			{
				Selected = GameResources.ConfigsSplited[index];
				if ((bool)App.Default["UseOldMode"])
				{
					Selected = GameResources.ConfigsSplited[int.Parse(Query.Attributes["received"].InnerText) / 250];
					Left = GameResources.ConfigsSplited.Count - int.Parse(Query.Attributes["received"].InnerText);
				}
			}
			if (Query.Name == "shop_get_offers")
			{
				Selected = GameResources.ShopOffersSplited[index];
				if ((bool)App.Default["UseOldMode"])
				{
					Selected = GameResources.ShopOffersSplited[int.Parse(Query.Attributes["received"].InnerText) / 250];
					Left = GameResources.ShopOffersSplited.Count - int.Parse(Query.Attributes["received"].InnerText) / 250 - 1;
				}
			}
			if (Query.Name == "quickplay_maplist")
			{
				Selected = GameResources.QuickPlayMapListSplited[index];
				if ((bool)App.Default["UseOldMode"])
				{
					Selected = GameResources.QuickPlayMapListSplited[int.Parse(Query.Attributes["received"].InnerText) / 250];
					Left = GameResources.QuickPlayMapListSplited.Count - int.Parse(Query.Attributes["received"].InnerText);
				}
			}
			if (Query.Name == "missions_get_list")
			{
				Selected = GameResources.PvE;
			}
			Process();
		}

		internal override void Process()
		{
			XDocument xDocument = new XDocument();
			XElement xElement = new XElement(Gateway.JabberNS + "iq");
			xElement.Add(new XAttribute("type", "result"));
			xElement.Add(new XAttribute("from", ((bool)App.Default["UseOldMode"]) ? To : ("masterserver@warface/" + User.Channel.Resource)));
			xElement.Add(new XAttribute("to", User.JID));
			xElement.Add(new XAttribute("id", Id));
			XElement xElement2 = new XElement(Stanza.NameSpace + "query");
			XElement xElement3 = new XElement(Query.Name);
			if (Selected != null)
			{
				xElement3 = XElement.Parse(Selected.InnerXml);
			}
			if ((bool)App.Default["UseOldMode"])
			{
				xElement3.RemoveAttributes();
				xElement3.Add(new XAttribute("token", "-1"));
				xElement3.Add(new XAttribute("left", Left));
			}
			xElement2.Add(xElement3);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			Compress(ref xDocument);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
