using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using System.Xml;
using AuroraServer.XMPP.QUERY;

namespace AuroraServer.CLASSES
{
	internal class GameResources
	{
		public static List<XmlDocument> ItemsSplited = new List<XmlDocument>();
		public static List<XmlDocument> Maps = new List<XmlDocument>();
		public static List<XmlDocument> ShopOffersSplited = new List<XmlDocument>();
		public static List<XmlDocument> ConfigsSplited = new List<XmlDocument>();
		public static List<XmlDocument> QuickPlayMapListSplited = new List<XmlDocument>();
		internal static Dictionary<string, List<string>> ShopItemsReged = new Dictionary<string, List<string>>();

		public static XmlDocument Items
		{
			get;
			private set;
		} = new XmlDocument();


		public static XmlDocument PvE
		{
			get;
			private set;
		} = new XmlDocument();


		public static XmlDocument QuickPlayMapList
		{
			get;
			private set;
		} = new XmlDocument();


		public static XmlDocument ShopOffers
		{
			get;
			private set;
		} = new XmlDocument();


		public static XmlDocument Configs
		{
			get;
			private set;
		} = new XmlDocument();


		public static XmlDocument OnlineVariables
		{
			get;
			private set;
		} = new XmlDocument();


		public static XmlDocument NewbieItemsXML
		{
			get;
			private set;
		} = new XmlDocument();


		public static XmlDocument NewbieItemsOldXML
		{
			get;
			private set;
		} = new XmlDocument();


		public static XmlDocument ExpCurve
		{
			get;
			private set;
		} = new XmlDocument();


		public static List<Item> NewbieItems
		{
			get;
			private set;
		} = new List<Item>();


		internal GameResources()
		{
			Timer timer = new Timer();
			timer.Interval = 42000.0;
			timer.Elapsed += delegate
			{
				ShopBuyOffer.BuyedTotally = 0;
			};
			timer.Start();
			Items.Load("Gamefiles/Items.xml");
			Configs.Load("Gamefiles/Configs.xml");
			ShopOffers.Load("Gamefiles/ShopOffers.xml");
			PvE.Load("Gamefiles/PvE.xml");
			OnlineVariables.Load("Gamefiles/OnlineVariables.xml");
			NewbieItemsXML.Load("Gamefiles/NewbieItems.xml");
			NewbieItemsOldXML.Load("Gamefiles/NewbieItemsOld.xml");
			QuickPlayMapList.Load("Gamefiles/QuickPlayMapList.xml");
			ExpCurve.Load("Gamefiles/ExpCurve.xml");
            //Program.WriteLine("Магазин параша", ConsoleColor.DarkMagenta);
			string[] files;
            files = Directory.GetFiles("Gamefiles/Maps", "*.xml", SearchOption.TopDirectoryOnly);
            foreach (string filename2 in files)
            {
                XmlDocument xmlDocument2 = new XmlDocument();
                xmlDocument2.Load(filename2);
                Maps.Add(xmlDocument2);
            }
            foreach (XmlNode childNode2 in NewbieItemsXML["items"].ChildNodes)
            {
                Item item2 = new Item();
                item2.Create((XmlElement)childNode2);
                NewbieItems.Add(item2);
            }
            files = Directory.GetFiles("Gamefiles/ShopItems", "*.xml", SearchOption.AllDirectories);
			foreach (string filename3 in files)
			{
				XmlDocument xmlDocument3 = new XmlDocument();
				xmlDocument3.Load(filename3);
				if (xmlDocument3["shop_item"] == null && xmlDocument3["GameItem"] == null)
				{
					continue;
				}
				string key = "";
				if (xmlDocument3.LastChild["mmo_stats"] != null)
				{
					foreach (XmlElement item3 in xmlDocument3.LastChild["mmo_stats"])
					{
						if (item3.Attributes["name"].InnerText == "item_category")
						{
							key = item3.Attributes["value"].InnerText;
							break;
						}
					}
					if (!ShopItemsReged.ContainsKey(key))
					{
						ShopItemsReged.Add(key, new List<string>());
					}
					ShopItemsReged[key].Add(xmlDocument3.LastChild.Attributes["name"].InnerText);
				}
			}
			SplitGamefiles(Items, ref ItemsSplited);
			SplitGamefiles(ShopOffers, ref ShopOffersSplited);
			SplitGamefiles(Configs, ref ConfigsSplited);
			SplitGamefiles(QuickPlayMapList, ref QuickPlayMapListSplited);
		}

		internal void SplitGamefiles(XmlDocument _From, ref List<XmlDocument> _To, int BlockSize = 250)
		{
			int num = 0;
			int num2 = _From["items"].ChildNodes.Count / BlockSize;
			int num3 = 0;
			for (int i = 0; i <= num2; i++)
			{
				XmlDocument xmlDocument = new XmlDocument();
				if (i != num2)
				{
					XmlElement xmlElement = xmlDocument.CreateElement("items");
					XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("code");
					XmlAttribute xmlAttribute2 = xmlDocument.CreateAttribute("from");
					XmlAttribute xmlAttribute3 = xmlDocument.CreateAttribute("to");
					XmlAttribute xmlAttribute4 = xmlDocument.CreateAttribute("hash");
					xmlAttribute.Value = "2";
					xmlAttribute2.Value = num3.ToString();
					xmlAttribute3.Value = $"{num3 + BlockSize}";
					xmlAttribute4.Value = "0";
					for (int j = 0; j < BlockSize; j++)
					{
						xmlElement.AppendChild(xmlDocument.ImportNode(_From["items"].ChildNodes[num + j], deep: true));
						num3++;
					}
					xmlElement.Attributes.Append(xmlAttribute);
					xmlElement.Attributes.Append(xmlAttribute2);
					xmlElement.Attributes.Append(xmlAttribute3);
					xmlElement.Attributes.Append(xmlAttribute4);
					xmlDocument.AppendChild(xmlElement);
					num += BlockSize;
				}
				else
				{
					BlockSize = _From["items"].ChildNodes.Count - num3;
					XmlElement xmlElement2 = xmlDocument.CreateElement("items");
					XmlAttribute xmlAttribute5 = xmlDocument.CreateAttribute("code");
					XmlAttribute xmlAttribute6 = xmlDocument.CreateAttribute("from");
					XmlAttribute xmlAttribute7 = xmlDocument.CreateAttribute("to");
					XmlAttribute xmlAttribute8 = xmlDocument.CreateAttribute("hash");
					xmlAttribute5.Value = "3";
					xmlAttribute6.Value = num3.ToString();
					xmlAttribute7.Value = $"{num3 + BlockSize}";
					xmlAttribute8.Value = "0";
					for (int k = 0; k < BlockSize; k++)
					{
						xmlElement2.AppendChild(xmlDocument.ImportNode(_From["items"].ChildNodes[num + k], deep: true));
					}
					xmlElement2.Attributes.Append(xmlAttribute5);
					xmlElement2.Attributes.Append(xmlAttribute6);
					xmlElement2.Attributes.Append(xmlAttribute7);
					xmlElement2.Attributes.Append(xmlAttribute8);
					xmlDocument.AppendChild(xmlElement2);
				}
				_To.Add(xmlDocument);
			}
		}
	}
}
