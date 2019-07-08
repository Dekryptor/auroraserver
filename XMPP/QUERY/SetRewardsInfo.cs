using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class SetRewardsInfo : Stanza
	{
		private string Channel;

		private List<XElement> Results = new List<XElement>();

		private List<Client> BcastReceivers = new List<Client>();

		public SetRewardsInfo(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
			string innerText = Query.Attributes["winning_team_id"].InnerText;
			double num = double.Parse(Query.Attributes["session_time"].InnerText, CultureInfo.InvariantCulture);
			Math.Round(num / TimeSpan.FromMinutes(1.0).TotalSeconds);
			foreach (XmlNode childNode in Query.ChildNodes)
			{
				if (childNode.Name == "team")
				{
					double num2 = 0.0;
					foreach (XmlElement PlayerEl in childNode.ChildNodes)
					{
						try
						{
							Client client = ArrayList.OnlineUsers.Find(delegate(Client Attribute)
							{
								if (Attribute.Player == null)
								{
									return false;
								}
								return Attribute.Player.UserID == long.Parse(PlayerEl.Attributes["profile_id"].InnerText);
							});
							if (client != null)
							{
								BcastReceivers.Add(client);
								if (User.Player.RoomPlayer.Room.Mission.Mode != "ffa" && User.Player.RoomPlayer.Room.Mission.Mode != "hnt")
								{
									if (childNode.Attributes["id"].InnerText == innerText)
										client.Player.StatMgr.IncrementDifficultyModePlayerStat("", "PVP", "player_sessions_won", 1L);
									
									else
										client.Player.StatMgr.IncrementDifficultyModePlayerStat("", "PVP", "player_sessions_lost", 1L);
									
								}
								int num3 = (int)num;
								if (PlayerEl.Attributes["in_session_from_start"].InnerText == "0")
								{
									num3 /= 2;
								}
								double num4 = 25.0;
								double num5 = 200.0;
								double num6 = 200.0;
								int count = ArrayList.OnlineUsers.Count;
								if (count > 300)
								{
									int num7 = count -= 300;
									num4 += (double)(50f + (float)num7);
									num5 += (double)(200f + (float)num7);
									num6 += (double)(200f + (float)num7);
								}
								double num8 = (PlayerEl.Attributes["in_session_from_start"].InnerText == "0") ? (num / (childNode.Attributes["id"].InnerText == innerText ? 4.11 : 4.75) + num2) : (num / (childNode.Attributes["id"].InnerText == innerText ? 2.61 : 3.1) + num2);
								double num9 = (PlayerEl.Attributes["in_session_from_start"].InnerText == "0") ? (num / (childNode.Attributes["id"].InnerText == innerText ? 4.81 : 5.01) + num2) : (num / (childNode.Attributes["id"].InnerText == innerText ? 3.31 : 3.68) + num2);
								double num10 = (PlayerEl.Attributes["in_session_from_start"].InnerText == "0") ? (num / (childNode.Attributes["id"].InnerText == innerText ? 4.51 : 4.94) + num2) : (num / (childNode.Attributes["id"].InnerText == innerText ? 2.97 : 3.45) + num2);
								foreach (Item item in client.Player.Items)
								{
									if (item.Name == "booster_02" && item.SecondsLeft > 0)
									{
										num5 += 1.0;
										num4 += 0.5;
										num6 += 0.75;
									}
									else if (item.Name == "booster_11" && item.SecondsLeft > 0)
									{
										num5 += 1.7999999523162842;
										num4 += 1.2999999523162842;
										num6 += 1.6499999761581421;
									}
									else if (item.Name == "booster_01" && item.SecondsLeft > 0)
									{
										num5 += 0.0;
										num4 += 0.0;
										num6 += 0.15000000596046448;
									}
									else if (item.Name == "booster_03" && item.SecondsLeft > 0)
									{
										num5 += 0.15000000596046448;
										num4 += 0.0;
										num6 += 0.0;
									}
									else if (item.Name == "booster_04" && item.SecondsLeft > 0)
									{
										num5 += 10.0;
										num4 += 0.0;
										num6 += 0.0;
									}
								}
								if (num4 != 0.0) num10 *= num4;
								if (num6 != 0.0) num9 *= num6;
								if (num5 != 0.0) num8 *= num5;
								client.Player.Experience += (int)num8;
								client.Player.GameMoney += (int)num9;
								client.Player.CryMoney += 1000;
                                client.Player.CrownMoney += 1000;
                                if (client.Player.StatMgr.GetPlayerStat("player_max_session_time") < num3 * 10)
								{
									client.Player.StatMgr.IncrementPlayerStat("player_max_session_time", num3 * 10);
								}
								client.Player.StatMgr.IncrementPlayerStat("player_online_time", num3 * 10);
								client.Player.StatMgr.IncrementPlayerStat("player_gained_money", (int)num9);
								XElement xElement = new XElement("player_result");
								xElement.Add(new XAttribute("nickname", client.Player.Nickname));
								xElement.Add(new XAttribute("experience", (int)num8));
								xElement.Add(new XAttribute("pvp_rating", 0));
								xElement.Add(new XAttribute("sponsor_points", (int)num10));
								xElement.Add(new XAttribute("money", (int)num9));
								xElement.Add(new XAttribute("bonus_money", 0));
								xElement.Add(new XAttribute("gained_crown_money", 0));
								xElement.Add(new XAttribute("bonus_experience", 0));
								xElement.Add(new XAttribute("bonus_sponsor_points", 0));
								xElement.Add(new XAttribute("completed_stages", ""));
								xElement.Add(new XAttribute("money_boost", 0));
								xElement.Add(new XAttribute("experience_boost", 0));
								xElement.Add(new XAttribute("sponsor_points_boost", 0));
								xElement.Add(new XAttribute("experience_boost_percent", num5 / 10.0));
								xElement.Add(new XAttribute("money_boost_percent", num6 / 10.0));
								xElement.Add(new XAttribute("sponsor_points_boost_percent", num4 / 10.0));
								xElement.Add(new XAttribute("is_vip", (num5 != 0.0 || num4 != 0.0 || num5 != 0.0) ? 1 : 0));
								xElement.Add(new XAttribute("score", client.Player.Nickname));
								xElement.Add(new XAttribute("no_crown_rewards", 0));
								xElement.Add(new XAttribute("dynamic_multipliers_info", 0));
                                client.Player.AddMoneyNotification("crown_money", 1000, "CrownMaster!");
                                Program.WriteLine($"{client.Player.Nickname} Выдаю награду", ConsoleColor.Yellow);
                                Results.Add(xElement);

                            }
						}
						catch
						{
						}
					}
				}
			}
			foreach (Client bcastReceiver in BcastReceivers)
			{
				new BroadcastSessionResults(bcastReceiver, Results).Process();
			}
			Process();
		}

		internal override void Process()
		{
			XDocument xDocument = new XDocument();
			XElement xElement = new XElement(Gateway.JabberNS + "iq");
			xElement.Add(new XAttribute("type", "result"));
			xElement.Add(new XAttribute("from", "masterserver@warface/aurora"));
			xElement.Add(new XAttribute("to", User.JID));
			xElement.Add(new XAttribute("id", Id));
			XElement xElement2 = new XElement(Stanza.NameSpace + "query");
			XElement content = new XElement("set_rewards_info");
			xElement2.Add(content);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
