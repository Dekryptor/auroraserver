using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class ShopBuyOffer : Stanza
	{
		internal static int BuyedTotally;

		private Item Buyed;

		private List<XElement> profileItemElements = new List<XElement>();

		private List<Item> PurchasedItems = new List<Item>();

		private int OfferId;

		private int ErrId;

		public ShopBuyOffer(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
			List<int> list = new List<int>();
			if (Query.Name == "shop_buy_multiple_offer")
			{
				foreach (XmlElement childNode in Query.ChildNodes)
				{
					OfferId = int.Parse(childNode.Attributes["id"].InnerText);
					list.Add(int.Parse(childNode.Attributes["id"].InnerText));
				}
			}
			else
			{
				OfferId = int.Parse(Query.Attributes["offer_id"].InnerText);
				list.Add(OfferId);
			}
			foreach (int item2 in list)
			{
				foreach (XmlNode Offer in GameResources.ShopOffers["items"].ChildNodes)
				{
					if (int.Parse(Offer.Attributes["id"].InnerText) == item2)
					{
						int num = int.Parse(Offer.Attributes["game_price"].InnerText);
						int num2 = int.Parse(Offer.Attributes["cry_price"].InnerText);
						int num3 = int.Parse(Offer.Attributes["crown_price"].InnerText);
						User.Player.CrownMoney -= num3;
						User.Player.GameMoney -= num;
						User.Player.CryMoney -= num2;
						if (Offer.Attributes["name"].InnerText.Contains("game_money_item_01"))
						{
							User.Player.GameMoney += int.Parse(Offer.Attributes["quantity"].InnerText);
						}
						else if (Offer.Attributes["name"].InnerText.Contains("box"))
						{
							profileItemElements.AddRange(User.Player.GeneratePrizes(Offer.Attributes["name"].InnerText, out ErrId, item2));
						}
						else if (Offer.Attributes["name"].InnerText.Contains("bundle_item"))
						{
							XmlDocument xmlDocument = new XmlDocument();
							xmlDocument.Load("Gamefiles/ShopItems/" + Offer.Attributes["name"].InnerText + ".xml");
							foreach (XmlNode childNode2 in xmlDocument["shop_item"]["bundle"].ChildNodes)
							{
								if (childNode2.Attributes["expiration"] != null)
								{
									if (childNode2.Attributes["expiration"].InnerText.Contains("d"))
									{
										int hours = (int)TimeSpan.FromDays(int.Parse(new Regex("[0-9]*").Match(childNode2.Attributes["expiration"].InnerText).Value)).TotalHours;
										PurchasedItems.Add(new Item(ItemType.TIME, User.Player.ItemSeed, childNode2.Attributes["name"].InnerText, hours, 0, 36000L)
										{
											BundleTime = childNode2.Attributes["expiration"].InnerText
										});
									}
									else if (childNode2.Attributes["expiration"].InnerText.Contains("h"))
									{
										int hours2 = int.Parse(new Regex("[0-9]*").Match(childNode2.Attributes["expiration"].InnerText).Value);
										PurchasedItems.Add(new Item(ItemType.TIME, User.Player.ItemSeed, childNode2.Attributes["name"].InnerText, hours2, 0, 36000L)
										{
											BundleTime = childNode2.Attributes["expiration"].InnerText
										});
									}
								}
								else if (childNode2.Attributes["regular"] != null)
								{
									PurchasedItems.Add(new Item(ItemType.NO_REPAIR, User.Player.ItemSeed, childNode2.Attributes["name"].InnerText, 0, 0, 36000L));
								}
								else if (childNode2.Attributes["amount"] != null)
								{
									PurchasedItems.Add(new Item(ItemType.CONSUMABLE, User.Player.ItemSeed, childNode2.Attributes["name"].InnerText, 0, int.Parse(childNode2.Attributes["amount"].InnerText), 36000L)
									{
										BundleQuantity = childNode2.Attributes["amount"].InnerText
									});
								}
								else
								{
									PurchasedItems.Add(new Item(ItemType.PERMANENT, User.Player.ItemSeed, childNode2.Attributes["name"].InnerText, 0, 0, 36000L));
								}
							}
						}
						else if (Offer.Attributes["expirationTime"].InnerText != "0")
						{
							int num4 = int.Parse(new Regex("[0-9]*").Match(Offer.Attributes["expirationTime"].InnerText).Value);
							if (Offer.Attributes["expirationTime"].InnerText.Contains("d"))
							{
								num4 = (int)TimeSpan.FromDays(num4).TotalHours;
								PurchasedItems.Add(new Item(ItemType.TIME, User.Player.ItemSeed, Offer.Attributes["name"].InnerText, num4, 0, 36000L));
							}
							if (Offer.Attributes["expirationTime"].InnerText.Contains("h"))
							{
								PurchasedItems.Add(new Item(ItemType.TIME, User.Player.ItemSeed, Offer.Attributes["name"].InnerText, num4, 0, 36000L));
							}
						}
						else if (Offer.Attributes["durabilityPoints"].InnerText != "0")
						{
							PurchasedItems.Add(new Item(ItemType.PERMANENT, User.Player.ItemSeed, Offer.Attributes["name"].InnerText, 0, 0, long.Parse(Offer.Attributes["durabilityPoints"].InnerText)));
						}
						else if (Offer.Attributes["quantity"].InnerText != "0")
						{
							PurchasedItems.Add(new Item(ItemType.CONSUMABLE, User.Player.ItemSeed, Offer.Attributes["name"].InnerText, 0, int.Parse(Offer.Attributes["quantity"].InnerText), 0L));
						}
						else if (User.Player.Items.Find(delegate(Item Attribute)
						{
							if (Attribute.Name == Offer.Attributes["name"].InnerText)
							{
								return Attribute.ItemType == ItemType.NO_REPAIR;
							}
							return false;
						}) == null)
						{
							PurchasedItems.Add(new Item(ItemType.NO_REPAIR, User.Player.ItemSeed, Offer.Attributes["name"].InnerText, 0, 0, 0L));
						}
						else
						{
							ErrId = 4;
						}
						if (profileItemElements.Count == 0)
						{
							foreach (Item purchasedItem in PurchasedItems)
							{
								Item item = purchasedItem;
								XElement xElement = new XElement("profile_item");
								if (purchasedItem.Name != "game_money_item_01")
								{
									item = User.Player.AddItem(purchasedItem);
									xElement.Add(new XAttribute("name", item.Name));
									xElement.Add(new XAttribute("profile_item_id", item.ID));
									xElement.Add(new XAttribute("offerId", item2));
									xElement.Add(new XAttribute("added_expiration", (!Offer.Attributes["name"].InnerText.Contains("bundle_item") && !Offer.Attributes["name"].InnerText.Contains("random_box")) ? Offer.Attributes["expirationTime"].InnerText : purchasedItem.BundleTime));
									xElement.Add(new XAttribute("added_quantity", (!Offer.Attributes["name"].InnerText.Contains("bundle_item") && !Offer.Attributes["name"].InnerText.Contains("random_box")) ? Offer.Attributes["quantity"].InnerText : purchasedItem.BundleQuantity));
									xElement.Add(new XAttribute("error_status", ErrId));
									xElement.Add(item.Serialize());
									profileItemElements.Add(xElement);
								}
								Buyed = item;
							}
						}
						if (ErrId > 0)
						{
							User.Player.CrownMoney += num3;
							User.Player.GameMoney += num;
							User.Player.CryMoney += num2;
						}
						break;
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
			XElement xElement3 = new XElement(Query.Name);
			xElement3.Add(new XAttribute("error_status", ErrId));
			if (Query.Name != "extend_item")
			{
				xElement3.Add(new XAttribute("offer_id", OfferId));
				XElement xElement4 = new XElement("purchased_item");
				foreach (XElement profileItemElement in profileItemElements)
				{
					xElement4.Add(profileItemElement);
				}
				XElement xElement5 = new XElement("money");
				xElement5.Add(new XAttribute("cry_money", User.Player.CryMoney));
				xElement5.Add(new XAttribute("crown_money", User.Player.CrownMoney));
				xElement5.Add(new XAttribute("game_money", User.Player.GameMoney));
				xElement3.Add(xElement4);
				xElement3.Add(xElement5);
			}
			else
			{
				xElement3.Add(new XAttribute("durability", Buyed.DurabilityPoints));
				xElement3.Add(new XAttribute("total_durability", Buyed.TotalDurabilityPoints));
				xElement3.Add(new XAttribute("expiration_time_utc", Buyed.ExpirationTime));
				xElement3.Add(new XAttribute("seconds_left", Buyed.SecondsLeft));
				xElement3.Add(new XAttribute("cry_money", User.Player.CryMoney));
				xElement3.Add(new XAttribute("game_money", User.Player.GameMoney));
				xElement3.Add(new XAttribute("crown_money", User.Player.CrownMoney));
			}
			xElement2.Add(xElement3);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
			User.CheckExperience();
		}
	}
}
