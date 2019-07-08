using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class TelemetryStream : Stanza
	{
		private string Channel;

		private static Regex Regex = new Regex("{\\[*.*\\]");

		public TelemetryStream(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
			if (Query.Attributes["finalize"].InnerText == "0")
			{
				string text = WebUtility.HtmlDecode(Query.InnerText);
				foreach (Match item in Regex.Matches(text))
				{
					string value = item.Value;
					int startIndex = text.IndexOf(value) + value.Length;
					if (text.Length != 0)
					{
						string text2 = text.Substring(startIndex);
						int num = text2.IndexOf('{');
						if (num > -1)
						{
							text2 = text2.Substring(0, num);
						}
						text = text.Replace(value + text2, "");
						if (text2.Length > 5)
						{
							if (!User.DedicatedTelemetryes.ContainsKey(value))
							{
								User.DedicatedTelemetryes.Add(value, null);
							}
							if (User.DedicatedTelemetryes[value] == null)
							{
								User.DedicatedTelemetryes[value] = XElement.Parse(text2);
							}
							else
							{
								User.DedicatedTelemetryes[value].Add(XElement.Parse(text2));
							}
						}
					}
				}
			}
			else
			{
				List<Client> list = new List<Client>();
				foreach (XElement value2 in User.DedicatedTelemetryes.Values)
				{
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.Load(value2.CreateReader());
					try
					{
						string name = xmlDocument.FirstChild.Name;
						if (name != null)
						{
							long ProfileId;
							if (!(name == "player"))
							{
								if (name == "stats_session" && xmlDocument["stats_session"].Attributes["winner"] != null)
								{
									foreach (XmlElement childNode in xmlDocument["stats_session"]["timelines"].ChildNodes)
									{
										string innerText = childNode.Attributes["name"].InnerText;
										if (innerText != null && innerText == "disconnect")
										{
											foreach (XmlElement childNode2 in childNode.ChildNodes)
											{
												ProfileId = long.Parse(childNode2["param"].Attributes["profile_id"].InnerText);
												string innerText2 = childNode2["param"].Attributes["cause"].InnerText;
												Client client = ArrayList.OnlineUsers.Find((Client Attribute) => Attribute.Player.UserID == ProfileId);
												if (client != null)
												{
													if (!list.Contains(client))
													{
														list.Add(client);
													}
													if (innerText2 == "left")
													{
														client.Player.StatMgr.IncrementModePlayerStat("PVP", "player_sessions_left", 1L);
													}
												}
											}
										}
									}
								}
							}
							else
							{
								ProfileId = long.Parse(xmlDocument["player"].Attributes["profile_id"].InnerText);
								string innerText3 = xmlDocument["player"].Attributes["character_class"].InnerText;
								uint num2 = 0u;
								if (xmlDocument["player"].Attributes["lifetime_end"] != null)
								{
									num2 = uint.Parse(xmlDocument["player"].Attributes["lifetime_end"].InnerText) - uint.Parse(xmlDocument["player"].Attributes["lifetime_begin"].InnerText);
								}
								else
								{
									num2 = ((xmlDocument["player"]["player"] == null) ? (uint.Parse(xmlDocument["player"]["timelines"].ChildNodes[xmlDocument["player"]["timelines"].ChildNodes.Count - 1]["val"].Attributes["time"].InnerText) - uint.Parse(xmlDocument["player"].Attributes["lifetime_begin"].InnerText)) : (uint.Parse(xmlDocument["player"]["player"]["timelines"].ChildNodes[xmlDocument["player"]["player"]["timelines"].ChildNodes.Count - 1]["val"].Attributes["time"].InnerText) - uint.Parse(xmlDocument["player"].Attributes["lifetime_begin"].InnerText)));
									num2 /= 10u;
								}
								Client client = ArrayList.OnlineUsers.Find((Client Attribute) => Attribute.Player.UserID == ProfileId);
								if (client != null)
								{
									if (!list.Contains(client))
									{
										list.Add(client);
									}
									Player player = client.Player;
									player.StatMgr.IncrementClassModePlayerStat(innerText3, "PVP", "player_playtime", num2);
									foreach (XmlElement childNode3 in xmlDocument["player"].ChildNodes)
									{
										object obj = null;
										if (childNode3.Name == "player" || childNode3.Name == "timelines")
										{
											obj = ((!(childNode3.Name == "player")) ? childNode3.ChildNodes : childNode3["timelines"].ChildNodes);
											foreach (XmlElement item2 in (XmlNodeList)obj)
											{
												string innerText = item2.Attributes["name"].InnerText;
												if (innerText != null)
												{
													switch (innerText)
													{
													case "resurrent":
														player.StatMgr.IncrementPlayerStat("player_resurrected_by_medic", 1L);
														break;
													case "kill":
														player.StatMgr.IncrementModePlayerStat("PVP", "player_kills_player", item2.ChildNodes.Count);
														foreach (XmlElement childNode4 in item2.ChildNodes)
														{
															if (childNode4.FirstChild.Attributes["hit_type"] != null)
															{
																if (childNode4.FirstChild.Attributes["hit_type"].InnerText == "melee")
																{
																	player.StatMgr.IncrementModePlayerStat("PVP", "player_kills_melee", 1L);
																}
																else if (childNode4.FirstChild.Attributes["hit_type"].InnerText == "clay_more")
																{
																	player.StatMgr.IncrementModePlayerStat("PVP", "player_kills_claymore", 1L);
																}
															}
														}
														break;
													case "score":
														foreach (XmlElement childNode5 in item2.ChildNodes)
														{
															string innerText4 = childNode5["param"].Attributes["event"].InnerText;
															if (innerText4 != null)
															{
																switch (innerText4)
																{
																case "headshot_kill":
																	player.StatMgr.IncrementClassModePlayerStat(innerText3, "PVP", "player_headshots", 1L);
																	break;
																case "teammate_restore":
																	if (innerText3 == "Medic")
																	{
																		player.StatMgr.IncrementPlayerStat("player_heal", int.Parse(childNode5["param"].Attributes["score"].InnerText));
																	}
																	else if (innerText3 == "Engineer")
																	{
																		player.StatMgr.IncrementPlayerStat("player_repair", int.Parse(childNode5["param"].Attributes["score"].InnerText));
																	}
																	break;
																case "teammate_resurrect":
																	player.StatMgr.IncrementPlayerStat("player_resurrect_made", 1L);
																	break;
																case "teammate_kill":
																	player.StatMgr.IncrementModePlayerStat("PVP", "player_kills_player_friendly", 1L);
																	break;
																case "sm_coop_assist":
																	player.StatMgr.IncrementPlayerStat("player_climb_assists", 1L);
																	break;
																case "sm_coop_climb":
																	player.StatMgr.IncrementPlayerStat("player_climb_coops", 1L);
																	break;
																case "teammate_give_ammo":
																	player.StatMgr.IncrementPlayerStat("player_ammo_restored", 1L);
																	break;
																case "claymore_kill":
																	player.StatMgr.IncrementPlayerStat("player_kills_claymore", 1L);
																	break;
																}
															}
														}
														break;
													case "hit":
														player.StatMgr.IncrementClassModePlayerStat(innerText3, "PVP", "player_hits", item2.ChildNodes.Count);
														foreach (XmlElement childNode6 in item2.ChildNodes)
														{
															int num4 = int.Parse(childNode6.FirstChild.Attributes["damage"].InnerText);
															if (player.StatMgr.GetPlayerStat("player_max_damage") < num4)
															{
																player.StatMgr.ResetPlayerStat("player_max_damage", num4);
															}
															player.StatMgr.IncrementPlayerStat("player_damage", num4);
														}
														break;
													case "shot":
														player.StatMgr.IncrementClassModePlayerStat(innerText3, "PVP", "player_shots", item2.ChildNodes.Count);
														break;
													case "death":
														player.StatMgr.IncrementModePlayerStat("PVP", "player_deaths", item2.ChildNodes.Count);
														break;
													case "kill_streak":
														foreach (XmlElement childNode7 in item2.ChildNodes)
														{
															int num3 = int.Parse(childNode7.Attributes["prm"].InnerText);
															player.StatMgr.IncrementModePlayerStat("PVP", "player_kill_streak", num3);
														}
														break;
													}
												}
											}
										}
									}
								}
							}
						}
					}
					catch (Exception)
					{
					}
				}
				foreach (Client item3 in list)
				{
					new GetPlayerStats(item3, null);
				}
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
			XElement content = new XElement("telemetry_stream");
			xElement2.Add(content);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
