using System;
using System.Xml;
using System.Xml.Linq;

namespace AuroraServer.CLASSES
{
	internal class Item
	{
		internal string BundleTime = "0";
		internal string BundleQuantity = "0";
		internal ItemType ItemType;
		internal long ID;
		internal string Name;
		internal string Config;
		internal string AttachedTo;
		internal byte Equipped;
		internal int Slot;
		internal long BuyTime;
		internal long ExpirationTime;
		internal bool ExpiredConfirmed;
		internal long TotalDurabilityPoints;
		internal long DurabilityPoints;
		internal int Quantity;
		internal long SecondsLeft
		{
			get
			{
				if (ItemType != ItemType.CONSUMABLE && ItemType != ItemType.NO_REPAIR)
				{
					return ExpirationTime - DateTimeOffset.UtcNow.ToUnixTimeSeconds();
				}
				return 0L;
			}
		}

		internal long RepairCost => 0L;

		internal static byte EquipperCalc(int Slot)
		{
			switch (Slot)
			{
			case 0:
				return 0;
			case 1:
				return 1;
			case 32768:
				return 8;
			case 1048576:
				return 16;
			case 1024:
				return 4;
			case 12:
				return 1;
			case 393216:
				return 8;
			case 12582912:
				return 16;
			case 12288:
				return 4;
			case 23:
				return 1;
			case 753664:
				return 8;
			case 24117248:
				return 16;
			case 23552:
				return 4;
			case 2:
				return 1;
			case 163840:
				return 8;
			case 2097152:
				return 16;
			default:
				switch (Slot)
				{
				case 23552:
					return 4;
				case 26:
					return 1;
				case 851968:
					return 8;
				case 27262976:
					return 16;
				case 26624:
					return 4;
				case 3247107:
					return 29;
				case 3:
					return 1;
				case 98304:
					return 8;
				case 3145728:
					return 16;
				case 3072:
					return 4;
				case 98307:
					return 9;
				case 3244032:
					return 24;
				case 3148800:
					return 20;
				case 3145731:
					return 17;
				case 3075:
					return 5;
				case 101376:
					return 12;
				default:
					switch (Slot)
					{
					case 3148800:
						return 1;
					case 3247104:
						return 28;
					case 3148803:
						return 21;
					case 101379:
						return 13;
					case 3244035:
						return 25;
					case 4329476:
						return 29;
					case 4:
						return 1;
					case 131072:
						return 8;
					case 4194304:
						return 16;
					case 4096:
						return 4;
					case 131076:
						return 9;
					case 4325376:
						return 24;
					case 4198400:
						return 20;
					case 4194308:
						return 17;
					case 4100:
						return 5;
					case 135168:
						return 12;
					default:
						switch (Slot)
						{
						case 4198400:
							return 1;
						case 4329472:
							return 28;
						case 4198404:
							return 21;
						case 135172:
							return 13;
						case 4325380:
							return 25;
						case 5411845:
							return 29;
						case 163845:
							return 9;
						case 5406720:
							return 24;
						default:
							switch (Slot)
							{
							case 4198400:
								return 20;
							case 4194308:
								return 17;
							case 5125:
								return 5;
							case 135168:
								return 12;
							case 5248000:
								return 1;
							case 5411840:
								return 28;
							case 5248005:
								return 21;
							case 168965:
								return 13;
							case 5406725:
								return 25;
							case 23812118:
								return 29;
							case 22:
								return 1;
							case 720896:
								return 8;
							case 23068672:
								return 16;
							case 22528:
								return 4;
							case 720918:
								return 9;
							case 23789568:
								return 24;
							case 23091200:
								return 20;
							case 23068694:
								return 17;
							case 22550:
								return 5;
							case 743424:
								return 12;
							case 4329472:
								return 28;
							case 23091222:
								return 21;
							case 743446:
								return 13;
							case 23789590:
								return 25;
							case 29223963:
								return 29;
							case 27:
								return 1;
							case 884736:
								return 8;
							case 28311552:
								return 16;
							case 27648:
								return 4;
							case 884763:
								return 9;
							case 29196288:
								return 24;
							case 28339200:
								return 20;
							case 28311579:
								return 17;
							case 27675:
								return 5;
							case 912384:
								return 12;
							case 29223936:
								return 28;
							case 28339227:
								return 21;
							case 912411:
								return 13;
							case 29196315:
								return 25;
							case 18400273:
								return 29;
							case 17:
								return 1;
							case 557056:
								return 8;
							case 17825792:
								return 16;
							case 17408:
								return 4;
							case 557073:
								return 9;
							case 18382848:
								return 24;
							case 17843200:
								return 20;
							case 17825809:
								return 17;
							case 17425:
								return 5;
							case 574464:
								return 12;
							case 18400256:
								return 28;
							case 17843217:
								return 21;
							case 574481:
								return 13;
							case 18382865:
								return 25;
							case 7576583:
								return 29;
							case 7:
								return 1;
							case 229376:
								return 8;
							case 7340032:
								return 16;
							case 7168:
								return 4;
							case 229383:
								return 9;
							case 7569408:
								return 24;
							case 7347200:
								return 20;
							case 7340039:
								return 17;
							case 7175:
								return 5;
							case 236544:
								return 12;
							case 7576576:
								return 28;
							case 7347207:
								return 21;
							case 236551:
								return 13;
							case 7569415:
								return 25;
							case 17317904:
								return 29;
							case 16:
								return 1;
							case 524288:
								return 8;
							case 16777216:
								return 16;
							case 16384:
								return 4;
							case 524304:
								return 9;
							case 17301504:
								return 24;
							case 16793600:
								return 20;
							case 16777232:
								return 17;
							case 16400:
								return 5;
							case 540672:
								return 12;
							case 17317888:
								return 28;
							case 16793616:
								return 21;
							case 540688:
								return 13;
							case 17301520:
								return 25;
							case 12988428:
								return 29;
							case 393228:
								return 9;
							case 12976128:
								return 24;
							case 12595200:
								return 20;
							case 12582924:
								return 17;
							case 12300:
								return 5;
							case 405504:
								return 12;
							case 12988416:
								return 28;
							case 12595212:
								return 21;
							case 405516:
								return 13;
							case 12976140:
								return 25;
							case 65536:
								return 8;
							default:
								//Console.WriteLine($"SLOT: {Slot} NOT FOUND!");
								return 0;
							}
						}
					}
				}
			}
		}

		internal Item()
		{
		}

		internal Item(ItemType ItemType, long Id, string Name, int Hours = 0, int Quantity = 0, long DurabilityPoints = 36000L)
		{
			this.ItemType = ItemType;
			AttachedTo = "";
			Config = "";
			this.Name = Name;
			ID = Id;
			BuyTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
			switch (ItemType)
			{
			case ItemType.DEFAULT:
			case ItemType.NO_REPAIR:
				break;
			case ItemType.CONSUMABLE:
				this.Quantity = Quantity;
				break;
			case ItemType.PERMANENT:
				this.DurabilityPoints = DurabilityPoints;
				TotalDurabilityPoints = DurabilityPoints;
				break;
			case ItemType.TIME:
				ExpirationTime = BuyTime + (int)TimeSpan.FromHours(Hours).TotalSeconds;
				break;
			}
		}

		internal XElement Serialize(bool IncludeType = false)
		{
			XElement xElement = new XElement("item");
			if (IncludeType)
			{
				xElement.Add(new XAttribute("type", (int)ItemType));
			}
			xElement.Add(new XAttribute("id", ID));
			xElement.Add(new XAttribute("name", Name));
			xElement.Add(new XAttribute("attached_to", AttachedTo));
			xElement.Add(new XAttribute("config", Config));
			xElement.Add(new XAttribute("slot", Slot));
			xElement.Add(new XAttribute("equipped", Equipped));
			xElement.Add(new XAttribute("default", (ItemType == ItemType.DEFAULT) ? 1 : 0));
			xElement.Add(new XAttribute("permanent", (ItemType == ItemType.PERMANENT) ? 1 : 0));
			xElement.Add(new XAttribute("expired_confirmed", ExpiredConfirmed ? 1 : 0));
			xElement.Add(new XAttribute("buy_time_utc", BuyTime));
			if (Name.StartsWith("f_") || Name.StartsWith("fbs_"))
			{
				ItemType = ItemType.NO_REPAIR;
			}
			else if (ItemType == ItemType.CONSUMABLE)
			{
				xElement.Add(new XAttribute("quantity", Quantity));
			}
			else if (ItemType == ItemType.DEFAULT)
			{
				xElement.Add(new XAttribute("seconds_left", 0));
			}
			else if (ItemType == ItemType.TIME)
			{
				xElement.Add(new XAttribute("expiration_time_utc", ExpirationTime));
                xElement.Add(new XAttribute("seconds_left", SecondsLeft));
            }
			else if (ItemType == ItemType.PERMANENT)
			{
				xElement.Add(new XAttribute("total_durability_points", TotalDurabilityPoints));
				xElement.Add(new XAttribute("durability_points", DurabilityPoints));
			}
			return xElement;
		}

		internal void Create(XmlElement Item)
		{
			ItemType = (ItemType)int.Parse(Item.Attributes["type"].InnerText);
			ID = long.Parse(Item.Attributes["id"].InnerText);
			Name = Item.Attributes["name"].InnerText;
			AttachedTo = Item.Attributes["attached_to"].InnerText;
			Config = Item.Attributes["config"].InnerText;
			Slot = int.Parse(Item.Attributes["slot"].InnerText);
			Equipped = byte.Parse(Item.Attributes["equipped"].InnerText);
			ExpiredConfirmed = (Item.Attributes["expired_confirmed"].InnerText == "1");
			BuyTime = long.Parse(Item.Attributes["buy_time_utc"].InnerText);
			if (Item.Attributes["name"].InnerText.StartsWith("f_") || Item.Attributes["name"].InnerText.StartsWith("fbs_"))
			{
				ItemType = ItemType.NO_REPAIR;
			}
			else if (ItemType == ItemType.CONSUMABLE)
			{
				Quantity = int.Parse(Item.Attributes["quantity"].InnerText);
			}
			else if (ItemType == ItemType.TIME)
			{
				ExpirationTime = long.Parse(Item.Attributes["expiration_time_utc"].InnerText);
			}
			else if (ItemType == ItemType.PERMANENT)
			{
				TotalDurabilityPoints = long.Parse(Item.Attributes["total_durability_points"].InnerText);
				DurabilityPoints = long.Parse(Item.Attributes["durability_points"].InnerText);
			}
		}
	}
}
