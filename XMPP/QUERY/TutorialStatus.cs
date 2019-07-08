using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class TutorialStatus : Stanza
	{
		private byte Event;

		private string ID;

		public TutorialStatus(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
			Event = byte.Parse(Query.Attributes["event"].InnerText);
			ID = Query.Attributes["id"].InnerText;
			if (Event == 2)
			{
				foreach (XmlDocument map in GameResources.Maps)
				{
					if (map.FirstChild.Attributes["uid"].InnerText == ID)
					{
						switch (map.FirstChild.Attributes["name"].InnerText)
						{
						case "@name_tutorial_soldier":
						case "@name_tutorial_medic":
						case "@name_tutorial_engineer":
						{
							string b = (map.FirstChild.Attributes["name"].InnerText == "@name_tutorial_soldier") ? "tutorial_1_completed" : ((map.FirstChild.Attributes["name"].InnerText == "@name_tutorial_medic") ? "tutorial_2_completed" : "tutorial_3_completed");
							if (map.FirstChild.Attributes["name"].InnerText == "@name_tutorial_soldier" && !User.Player.SoldierPassed)
							{
								User.Player.SoldierPassed = true;
							}
							else if (map.FirstChild.Attributes["name"].InnerText == "@name_tutorial_medic" && !User.Player.MedicPassed)
							{
								User.Player.MedicPassed = true;
							}
							else
							{
								if (!(map.FirstChild.Attributes["name"].InnerText == "@name_tutorial_engineer") || User.Player.EngineerPassed)
								{
									break;
								}
								User.Player.EngineerPassed = true;
							}
							foreach (XmlNode childNode in GameResources.Configs["items"]["special_reward_configuration"].ChildNodes)
							{
								if (childNode.Attributes["name"].InnerText == b)
								{
									foreach (XmlNode childNode2 in childNode.ChildNodes)
									{
										if (childNode2.Name == "money")
										{
											User.Player.AddMoneyNotification(childNode2.Attributes["currency"].InnerText, int.Parse(childNode2.Attributes["amount"].InnerText));
										}
										if (childNode2.Name == "item")
										{
											User.Player.AddItemNotification((childNode2.Attributes["expiration"] != null) ? "Expiration" : ((childNode2.Attributes["amount"] != null) ? "Consumable" : "Permanent"), childNode2.Attributes["name"].InnerText, (childNode2.Attributes["expiration"] != null) ? (int.Parse(new Regex("[0-9]*").Match(childNode2.Attributes["expiration"].InnerText).Value) * ((!childNode2.Attributes["expiration"].InnerText.Contains("d")) ? 1 : 24)) : ((childNode2.Attributes["amount"] != null) ? int.Parse(childNode2.Attributes["amount"].InnerText) : 0));
										}
										switch (childNode2.Name)
										{
										case "money":
											switch (childNode2.Attributes["currency"].InnerText)
											{
											case "game_money":
												User.Player.GameMoney += int.Parse(childNode2.Attributes["amount"].InnerText);
												break;
											case "cry_money":
												User.Player.CryMoney += int.Parse(childNode2.Attributes["amount"].InnerText);
												break;
											case "crown_money":
												User.Player.CrownMoney += int.Parse(childNode2.Attributes["amount"].InnerText);
												break;
											}
											break;
										case "item":
											User.Player.AddItem(new Item((childNode2.Attributes["expiration"] != null) ? ItemType.TIME : ((childNode2.Attributes["amount"] != null) ? ItemType.CONSUMABLE : ItemType.PERMANENT), User.Player.ItemSeed, childNode2.Attributes["name"].InnerText, (childNode2.Attributes["expiration"] != null) ? (int.Parse(new Regex("[0-9]*").Match(childNode2.Attributes["expiration"].InnerText).Value) * ((!childNode2.Attributes["expiration"].InnerText.Contains("d")) ? 1 : 24)) : 0, (childNode2.Attributes["amount"] != null) ? int.Parse(childNode2.Attributes["amount"].InnerText) : 0, 36000L));
											break;
										}
									}
								}
							}
							break;
						}
						}
					}
				}
				Process();
				new SyncNotification(User).Process();
				User.Player.Save();
			}
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
			XElement xElement3 = new XElement("tutorial_status");
			XElement xElement4 = new XElement("profile_progression_update");
			xElement4.Add(new XAttribute("profile_id", User.Player.UserID));
			xElement4.Add(new XAttribute("mission_unlocked", User.Player.UnlockedMissions));
			xElement4.Add(new XAttribute("tutorial_unlocked", User.Player.TutorialSuggest));
			xElement4.Add(new XAttribute("tutorial_passed", User.Player.TutorialPassed));
			xElement4.Add(new XAttribute("class_unlocked", User.Player.UnlockedClasses));
			xElement3.Add(xElement4);
			xElement2.Add(xElement3);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
