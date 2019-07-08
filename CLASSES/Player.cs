using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace AuroraServer.CLASSES
{
    internal class Player
    {
        internal ClanProperties ClanPlayer = new ClanProperties();
        internal RoomProperties RoomPlayer = new RoomProperties();
        internal long UserID = -1;
        internal string Nickname = "NoNameYet";
        internal string Gender = "male";
        internal DateTime LastSeen = DateTime.Now;
        internal XmlDocument Settings = new XmlDocument();
        internal XmlDocument Stats = new XmlDocument();
        internal XmlDocument RandomBox = new XmlDocument();
        internal XmlDocument Achievements = new XmlDocument();
        internal XmlDocument notifications = new XmlDocument();
        internal XmlDocument Clan = new XmlDocument();
        internal XmlDocument friends = new XmlDocument();
        internal double Height = 1.0;
        internal double Fatness = 1.0;
        internal string Head = "default_head_04";
        internal int GameMoney = 50000;
        internal int CryMoney = 3000;
        internal int CrownMoney = 12000;
        internal Random Random = new Random();
        internal bool SoldierSuggest = true;
        internal bool EngineerSuggest = true;
        internal bool MedicSuggest = true;
        internal string UnlockedMissions = "none,trainingmission,all,easy,normal,hard";
        internal List<Item> Items = GameResources.NewbieItems;
        internal Stopwatch BoxWatcher = new Stopwatch();
        internal BanType BanType;
        internal long UnbanTime;
        internal PrivilegieId Privilegie;
        internal long TicketId;
        internal string Nick;
        internal byte OldRank;
        internal byte CurrentClass;
        internal int Experience;
        internal int BannerMark;
        internal int BannerBadge;
        internal int BannerStripe;
        internal bool SoldierPassed;
        internal bool EngineerPassed;
        internal bool MedicPassed;
        internal StatsManager StatMgr;
        internal int BoxCount;

        internal long ItemSeed
        {
            get
            {
                return (long)(this.Items.Count + 1);
            }
        }

        internal bool ProfileCreated
        {
            get
            {
                return this.UserID > 0L;
            }
        }

        internal byte Rank
        {
            get
            {
                foreach (XmlNode childNode in GameResources.ExpCurve["exp_curve"].ChildNodes)
                {
                    if (this.Experience < int.Parse(childNode.Attributes["exp"].InnerText))
                        return byte.Parse((int.Parse(childNode.Name.Replace("level", "")) - 1).ToString());
                }
                return 90;
            }
        }

        internal XElement Friends
        {
            get
            {
                XElement xelement1 = new XElement((XName)"friend_list");
                foreach (XmlElement childNode in this.friends["friends"].ChildNodes)
                {
                    long ID = long.Parse(childNode.InnerText);
                    XElement xelement2 = new XElement((XName)"friend");
                    try
                    {
                        Client client = ArrayList.OnlineUsers.Find((Predicate<Client>)(Attribute => Attribute.Player.UserID == ID));
                        if (client != null)
                        {
                            xelement2.Add((object)new XAttribute((XName)"jid", (object)client.JID));
                            xelement2.Add((object)new XAttribute((XName)"profile_id", (object)client.Player.UserID));
                            xelement2.Add((object)new XAttribute((XName)"nickname", (object)client.Player.Nickname));
                            xelement2.Add((object)new XAttribute((XName)"status", (object)client.Status));
                            xelement2.Add((object)new XAttribute((XName)"experience", (object)client.Player.Experience));
                            xelement2.Add((object)new XAttribute((XName)"location", (object)client.Location));
                        }
                        else
                        {
                            Player player = new Player()
                            {
                                UserID = ID
                            };
                            if (player.Load(true))
                            {
                                xelement2.Add((object)new XAttribute((XName)"jid", (object)""));
                                xelement2.Add((object)new XAttribute((XName)"profile_id", (object)player.UserID));
                                xelement2.Add((object)new XAttribute((XName)"nickname", (object)player.Nickname));
                                xelement2.Add((object)new XAttribute((XName)"status", (object)0));
                                xelement2.Add((object)new XAttribute((XName)"experience", (object)player.Experience));
                                xelement2.Add((object)new XAttribute((XName)"location", (object)""));
                            }
                            else
                            {
                                xelement2.Add((object)new XAttribute((XName)"jid", (object)""));
                                xelement2.Add((object)new XAttribute((XName)"profile_id", (object)ID));
                                xelement2.Add((object)new XAttribute((XName)"nickname", (object)string.Format("без_имени_{0}", (object)ID)));
                                xelement2.Add((object)new XAttribute((XName)"status", (object)0));
                                xelement2.Add((object)new XAttribute((XName)"experience", (object)0));
                                xelement2.Add((object)new XAttribute((XName)"location", (object)""));
                            }
                        }
                    }
                    catch
                    {
                        xelement2.Add((object)new XAttribute((XName)"jid", (object)""));
                        xelement2.Add((object)new XAttribute((XName)"profile_id", (object)ID));
                        xelement2.Add((object)new XAttribute((XName)"nickname", (object)string.Format("без_имени_{0}", (object)ID)));
                        xelement2.Add((object)new XAttribute((XName)"status", (object)0));
                        xelement2.Add((object)new XAttribute((XName)"experience", (object)0));
                        xelement2.Add((object)new XAttribute((XName)"location", (object)""));
                    }
                    xelement1.Add((object)xelement2);
                }
                return xelement1;
            }
        }

        internal XmlDocument Notifications
        {
            get
            {
                foreach (XmlNode childNode in this.notifications["notifications"].ChildNodes)
                {
                    XmlNode Notification = childNode;
                    if (Notification.Attributes["type"].InnerText == "128")
                    {
                        Client client = ArrayList.OnlineUsers.Find((Predicate<Client>)(Attribute => Attribute.Player.Nickname == Notification["invite_result"].Attributes["nickname"].InnerText));
                        if (client != null)
                        {
                            Notification["invite_result"].Attributes["profile_id"].InnerText = client.Player.UserID.ToString();
                            Notification["invite_result"].Attributes["jid"].InnerText = client.JID;
                            Notification["invite_result"].Attributes["nickname"].InnerText = client.Player.Nickname;
                            Notification["invite_result"].Attributes["status"].InnerText = client.Status.ToString();
                            Notification["invite_result"].Attributes["location"].InnerText = client.Location.ToString();
                            Notification["invite_result"].Attributes["experience"].InnerText = client.Player.Experience.ToString();
                        }
                        else
                        {
                            Player player = new Player()
                            {
                                Nickname = Notification["invite_result"].Attributes["nickname"].InnerText
                            };
                            player.Load(false);
                            Notification["invite_result"].Attributes["profile_id"].InnerText = player.UserID.ToString();
                            Notification["invite_result"].Attributes["jid"].InnerText = "";
                            Notification["invite_result"].Attributes["nickname"].InnerText = player.Nickname;
                            Notification["invite_result"].Attributes["status"].InnerText = "0";
                            Notification["invite_result"].Attributes["location"].InnerText = "";
                            Notification["invite_result"].Attributes["experience"].InnerText = player.Experience.ToString();
                        }
                    }
                }
                return this.notifications;
            }
        }

        internal byte UnlockedClasses
        {
            get
            {
                byte num = 5;
                if (this.MedicPassed)
                    num += (byte)8;
                if (this.EngineerPassed)
                    num += (byte)16;
                return num;
            }
        }

        internal byte TutorialPassed
        {
            get
            {
                byte num = 0;
                if (this.SoldierPassed)
                    ++num;
                if (this.MedicPassed)
                    num += (byte)2;
                if (this.EngineerPassed)
                    num += (byte)4;
                return num;
            }
        }

        internal byte TutorialSuggest
        {
            get
            {
                byte num = 0;
                if (this.SoldierSuggest)
                    ++num;
                if (this.MedicSuggest)
                    num += (byte)2;
                if (this.EngineerSuggest)
                    num += (byte)4;
                return num;
            }
        }

        internal XElement Avatar
        {
            get
            {
                XElement xelement = new XElement((XName)"profile");
                xelement.Add((object)new XAttribute((XName)"gender", (object)this.Gender));
                xelement.Add((object)new XAttribute((XName)"height", (object)this.Height));
                xelement.Add((object)new XAttribute((XName)"fatness", (object)this.Fatness));
                xelement.Add((object)new XAttribute((XName)"head", (object)this.Head));
                xelement.Add((object)new XAttribute((XName)"current_class", (object)this.CurrentClass));
                xelement.Add((object)new XAttribute((XName)"banner_mark", (object)this.BannerMark));
                xelement.Add((object)new XAttribute((XName)"banner_stripe", (object)this.BannerStripe));
                xelement.Add((object)new XAttribute((XName)"banner_badge", (object)this.BannerBadge));
                xelement.Add((object)new XAttribute((XName)"game_money", (object)this.GameMoney));
                xelement.Add((object)new XAttribute((XName)"cry_money", (object)this.CryMoney));
                xelement.Add((object)new XAttribute((XName)"crown_money", (object)this.CrownMoney));
                xelement.Add((object)new XAttribute((XName)"medic_tutorial_passed", (object)this.MedicPassed));
                xelement.Add((object)new XAttribute((XName)"engineer_tutorial_passed", (object)this.EngineerPassed));
                xelement.Add((object)new XAttribute((XName)"soldier_tutorial_passed", (object)this.SoldierPassed));
                xelement.Add((object)new XAttribute((XName)"soldier_tutorial_suggest", (object)this.SoldierSuggest));
                xelement.Add((object)new XAttribute((XName)"engineer_tutorial_suggest", (object)this.EngineerSuggest));
                xelement.Add((object)new XAttribute((XName)"medic_tutorial_suggest", (object)this.MedicSuggest));
                xelement.Add((object)new XAttribute((XName)"unlocked_missions", (object)this.UnlockedMissions));
                xelement.Add((object)new XAttribute((XName)"clan_id", (object)(this.ClanPlayer.Clan != null ? this.ClanPlayer.Clan.ID : 0L)));
                return xelement;
            }
        }

        internal void UpdateAchievement(XmlNode Chunk)
        {
            foreach (XmlNode childNode in this.Achievements.FirstChild.ChildNodes)
            {
                if (childNode.Attributes["achievement_id"].InnerText == Chunk.Attributes["achievement_id"].InnerText)
                {
                    this.Achievements.FirstChild.RemoveChild(childNode);
                    this.Achievements.FirstChild.AppendChild(this.Achievements.ImportNode(Chunk, true));
                    return;
                }
            }
            this.Achievements.FirstChild.AppendChild(this.Achievements.ImportNode(Chunk, true));
        }

        internal void CheckAndFixItems()
        {
            List<Item> all1 = this.Items.FindAll((Predicate<Item>)(Attribute => Attribute.Slot == 1));
            List<Item> all2 = this.Items.FindAll((Predicate<Item>)(Attribute => Attribute.Slot == 32768));
            List<Item> all3 = this.Items.FindAll((Predicate<Item>)(Attribute => Attribute.Slot == 1048576));
            List<Item> all4 = this.Items.FindAll((Predicate<Item>)(Attribute => Attribute.Slot == 1024));
            if (all1.Count != 1 || all1[0].SecondsLeft < 0L)
            {
                foreach (Item obj in all1)
                {
                    obj.Slot = 0;
                    obj.Equipped = (byte)0;
                }
                Item obj1 = this.Items.Find((Predicate<Item>)(Attribute =>
                {
                    if (Attribute.ItemType != ItemType.DEFAULT || !Attribute.Name.StartsWith("ar"))
                        return Attribute.Name.StartsWith("mg");
                    return true;
                }));
                obj1.Equipped = (byte)1;
                obj1.Slot = 1;
            }
            if (all2.Count != 1 || all2[0].SecondsLeft < 0L)
            {
                foreach (Item obj in all2)
                {
                    obj.Slot = 0;
                    obj.Equipped = (byte)0;
                }
                Item obj1 = this.Items.Find((Predicate<Item>)(Attribute =>
                {
                    if (Attribute.ItemType == ItemType.DEFAULT)
                        return Attribute.Name.StartsWith("shg");
                    return false;
                }));
                obj1.Equipped = (byte)8;
                obj1.Slot = 32768;
            }
            if (all3.Count != 1 || all3[0].SecondsLeft < 0L)
            {
                foreach (Item obj in all3)
                {
                    obj.Slot = 0;
                    obj.Equipped = (byte)0;
                }
                Item obj1 = this.Items.Find((Predicate<Item>)(Attribute =>
                {
                    if (Attribute.ItemType == ItemType.DEFAULT)
                        return Attribute.Name.StartsWith("smg");
                    return false;
                }));
                obj1.Equipped = (byte)16;
                obj1.Slot = 1048576;
            }
            if (all4.Count == 1 && all4[0].SecondsLeft >= 0L)
                return;
            foreach (Item obj in all4)
            {
                obj.Slot = 0;
                obj.Equipped = (byte)0;
            }
            Item obj2 = this.Items.Find((Predicate<Item>)(Attribute =>
            {
                if (Attribute.ItemType == ItemType.DEFAULT)
                    return Attribute.Name.StartsWith("smg");
                return false;
            }));
            obj2.Equipped = (byte)4;
            obj2.Slot = 1024;
        }

        internal void AddFriend(string ID)
        {
            XmlElement element = this.friends.CreateElement("friend");
            element.InnerText = ID;
            this.friends["friends"].AppendChild((XmlNode)element);
        }

        internal void RemoveFriend(string ID)
        {
            foreach (XmlNode childNode in this.friends["friends"].ChildNodes)
            {
                if (childNode.InnerText == ID)
                    this.friends["friends"].RemoveChild(childNode);
            }
        }

        internal Player()
        {
            this.Items = new List<Item>((IEnumerable<Item>)GameResources.NewbieItems);
            this.friends.LoadXml("<friends/>");
            this.RandomBox.LoadXml("<randomboxes/>");
            this.Achievements.LoadXml("<achievements/>");
            this.Stats.LoadXml("<stats/>");
            this.notifications.LoadXml("<notifications/>");
            this.Clan.LoadXml("<clan/>");
        }

        internal void AddMoneyNotification(string Currency, int Amount = 0, string Message = "")
        {
            XElement xelement1 = new XElement((XName)"notif");
            xelement1.Add((object)new XAttribute((XName)"id", (object)this.Random.Next(999999, int.MaxValue)));
            xelement1.Add((object)new XAttribute((XName)"type", (object)2048));
            xelement1.Add((object)new XAttribute((XName)"confirmation", (object)1));
            xelement1.Add((object)new XAttribute((XName)"from_jid", (object)"aurora@server"));
            xelement1.Add((object)new XAttribute((XName)"message", (object)Message));
            XElement xelement2 = new XElement((XName)"give_money");
            xelement2.Add((object)new XAttribute((XName)"currency", (object)Currency));
            xelement2.Add((object)new XAttribute((XName)"type", (object)"1"));
            xelement2.Add((object)new XAttribute((XName)"amount", (object)Amount));
            xelement2.Add((object)new XAttribute((XName)"notify", (object)"1"));
            xelement1.Add((object)xelement2);
            XmlReader reader = xelement1.CreateReader();
            reader.Read();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(reader);
            this.Notifications.FirstChild.AppendChild(this.Notifications.ImportNode((XmlNode)xmlDocument.DocumentElement, true));
        }

        internal void AddFriendNotification(
          string Initiator,
          bool isClan = false,
          long ClanId = 0,
          string ClanName = "")
        {
            XElement xelement1 = new XElement((XName)"notif");
            xelement1.Add((object)new XAttribute((XName)"id", (object)this.Random.Next(999999, int.MaxValue)));
            xelement1.Add((object)new XAttribute((XName)"type", (object)(isClan ? 16 : 64)));
            xelement1.Add((object)new XAttribute((XName)"confirmation", (object)1));
            xelement1.Add((object)new XAttribute((XName)"from_jid", (object)"aurora@server"));
            xelement1.Add((object)new XAttribute((XName)"message", (object)""));
            XElement xelement2 = new XElement((XName)"invitation");
            xelement2.Add((object)new XAttribute((XName)"initiator", (object)Initiator));
            if (isClan)
            {
                xelement2.Add((object)new XAttribute((XName)"clan_name", (object)ClanName));
                xelement2.Add((object)new XAttribute((XName)"clan_id", (object)ClanId));
            }
            xelement1.Add((object)xelement2);
            XmlReader reader = xelement1.CreateReader();
            reader.Read();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(reader);
            this.Notifications.FirstChild.AppendChild(this.Notifications.ImportNode((XmlNode)xmlDocument.DocumentElement, true));
        }

        internal void AddClanNotification(string Initiator, bool isClan = false, long ClanId = 0, string ClanName = "")
        {
            XElement xelement1 = new XElement((XName)"notif");
            xelement1.Add((object)new XAttribute((XName)"id", (object)this.Random.Next(999999, int.MaxValue)));
            xelement1.Add((object)new XAttribute((XName)"type", (object)16));
            xelement1.Add((object)new XAttribute((XName)"confirmation", (object)1));
            xelement1.Add((object)new XAttribute((XName)"from_jid", (object)"aurora@server"));
            xelement1.Add((object)new XAttribute((XName)"message", (object)""));
            XElement xelement2 = new XElement((XName)"invitation");
            xelement2.Add((object)new XAttribute((XName)"initiator", (object)Initiator));
            xelement2.Add((object)new XAttribute((XName)"clan_name", (object)ClanName));
            xelement2.Add((object)new XAttribute((XName)"clan_id", (object)ClanId));
            xelement1.Add((object)xelement2);
            XmlReader reader = xelement1.CreateReader();
            reader.Read();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(reader);
            this.Notifications.FirstChild.AppendChild(this.Notifications.ImportNode((XmlNode)xmlDocument.DocumentElement, true));
        }

        internal void AddCustomMessage(string Message)
        {
            XElement xelement = new XElement((XName)"notif");
            xelement.Add((object)new XAttribute((XName)"id", (object)this.Random.Next(999999, int.MaxValue)));
            xelement.Add((object)new XAttribute((XName)"type", (object)8));
            xelement.Add((object)new XAttribute((XName)"confirmation", (object)1));
            xelement.Add((object)new XAttribute((XName)"from_jid", (object)"aurora@server"));
            xelement.Add((object)new XAttribute((XName)"message", (object)""));
            xelement.Add((object)new XElement((XName)"message", (object)new XAttribute((XName)"data", (object)Message)));
            XmlReader reader = xelement.CreateReader();
            reader.Read();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(reader);
            this.Notifications.FirstChild.AppendChild(this.Notifications.ImportNode((XmlNode)xmlDocument.DocumentElement, true));
        }

        internal void AddFriendResultNotification(
          long ProfileId,
          string Jid,
          string Nickname,
          int Status,
          string Location,
          int Experience,
          string Result,
          bool isClan = false)
        {
            XElement xelement1 = new XElement((XName)"notif");
            xelement1.Add((object)new XAttribute((XName)"id", (object)this.Random.Next(999999, int.MaxValue)));
            xelement1.Add((object)new XAttribute((XName)"type", (object)(isClan ? 32 : 128)));
            xelement1.Add((object)new XAttribute((XName)"confirmation", (object)1));
            xelement1.Add((object)new XAttribute((XName)"from_jid", (object)"aurora@server"));
            xelement1.Add((object)new XAttribute((XName)"message", (object)""));
            XElement xelement2 = new XElement((XName)"invite_result");
            xelement2.Add((object)new XAttribute((XName)"profile_id", (object)ProfileId));
            xelement2.Add((object)new XAttribute((XName)"jid", (object)Jid));
            xelement2.Add((object)new XAttribute((XName)"nickname", (object)Nickname));
            xelement2.Add((object)new XAttribute((XName)"status", (object)Status));
            xelement2.Add((object)new XAttribute((XName)"location", (object)Location));
            xelement2.Add((object)new XAttribute((XName)"experience", (object)Experience));
            xelement2.Add((object)new XAttribute((XName)"result", (object)Result));
            xelement2.Add((object)new XAttribute((XName)"invite_date", (object)0));
            xelement1.Add((object)xelement2);
            XmlReader reader = xelement1.CreateReader();
            reader.Read();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(reader);
            this.Notifications.FirstChild.AppendChild(this.Notifications.ImportNode((XmlNode)xmlDocument.DocumentElement, true));
        }

        internal void AddItemNotification(string OfferType, string Name, int Amount = 0, string Message = "")
        {
            XElement xelement1 = new XElement((XName)"notif");
            xelement1.Add((object)new XAttribute((XName)"id", (object)this.Random.Next(999999, int.MaxValue)));
            xelement1.Add((object)new XAttribute((XName)"type", (object)256));
            xelement1.Add((object)new XAttribute((XName)"confirmation", (object)1));
            xelement1.Add((object)new XAttribute((XName)"from_jid", (object)"aurora@server"));
            xelement1.Add((object)new XAttribute((XName)"message", (object)Message));
            XElement xelement2 = new XElement((XName)"give_item");
            xelement2.Add((object)new XAttribute((XName)"name", (object)Name));
            xelement2.Add((object)new XAttribute((XName)"offer_type", (object)OfferType));
            if (OfferType == "Expiration")
                xelement2.Add((object)new XAttribute((XName)"extended_time", (object)Amount));
            else if (OfferType == "Consumable")
                xelement2.Add((object)new XAttribute((XName)"consumables_count", (object)Amount));
            xelement2.Add((object)new XAttribute((XName)"notify", (object)"1"));
            xelement1.Add((object)xelement2);
            XmlReader reader = xelement1.CreateReader();
            reader.Read();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(reader);
            this.Notifications.FirstChild.AppendChild(this.Notifications.ImportNode((XmlNode)xmlDocument.DocumentElement, true));
        }

        internal void AddRandomBoxNotification(string Box, string Message = "")
        {
            XElement xelement1 = new XElement((XName)"notif");
            xelement1.Add((object)new XAttribute((XName)"id", (object)this.Random.Next(999999, int.MaxValue)));
            xelement1.Add((object)new XAttribute((XName)"type", (object)8192));
            xelement1.Add((object)new XAttribute((XName)"confirmation", (object)1));
            xelement1.Add((object)new XAttribute((XName)"from_jid", (object)"aurora@server"));
            xelement1.Add((object)new XAttribute((XName)"message", (object)Message));
            XElement xelement2 = new XElement((XName)"give_random_box");
            xelement2.Add((object)new XAttribute((XName)"name", (object)Box));
            xelement2.Add((object)new XAttribute((XName)"notify", (object)1));
            XElement xelement3 = new XElement((XName)"purchased_item");
            int ErrId;
            foreach (XElement priz in this.GeneratePrizes(Box, out ErrId, 0))
                xelement3.Add((object)priz);
            xelement2.Add((object)xelement3);
            xelement1.Add((object)xelement2);
            XmlReader reader = xelement1.CreateReader();
            reader.Read();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(reader);
            this.Notifications.FirstChild.AppendChild(this.Notifications.ImportNode((XmlNode)xmlDocument.DocumentElement, true));
        }

        internal void AddRankNotifierNotification(byte OldRank, byte NewRank, string Message = "")
        {
            XElement xelement1 = new XElement((XName)"notif");
            xelement1.Add((object)new XAttribute((XName)"id", (object)this.Random.Next(999999, int.MaxValue)));
            xelement1.Add((object)new XAttribute((XName)"type", (object)131072));
            xelement1.Add((object)new XAttribute((XName)"confirmation", (object)1));
            xelement1.Add((object)new XAttribute((XName)"from_jid", (object)"aurora@server"));
            xelement1.Add((object)new XAttribute((XName)"message", (object)Message));
            XElement xelement2 = new XElement((XName)"new_rank_reached");
            xelement2.Add((object)new XAttribute((XName)"old_rank", (object)OldRank));
            xelement2.Add((object)new XAttribute((XName)"new_rank", (object)NewRank));
            xelement1.Add((object)xelement2);
            XmlReader reader = xelement1.CreateReader();
            reader.Read();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(reader);
            this.Notifications.FirstChild.AppendChild(this.Notifications.ImportNode((XmlNode)xmlDocument.DocumentElement, true));
        }

        internal Item AddItem(Item Item)
        {
            foreach (Item obj in this.Items)
            {
                if (obj.ItemType == Item.ItemType && obj.Name == Item.Name)
                {
                    switch (obj.ItemType)
                    {
                        case ItemType.CONSUMABLE:
                            obj.Quantity += Item.Quantity;
                            break;
                        case ItemType.PERMANENT:
                            obj.DurabilityPoints += Item.DurabilityPoints;
                            break;
                        case ItemType.TIME:
                            obj.ExpirationTime = obj.SecondsLeft > 0L ? (obj.ExpirationTime += Item.SecondsLeft) : DateTimeOffset.UtcNow.ToUnixTimeSeconds() + Item.SecondsLeft;
                            break;
                    }
                    return obj;
                }
            }
            Item.ID = this.ItemSeed;
            this.Items.Add(Item);
            return Item;
        }

        internal void Save()
        {
            XElement xelement = new XElement((XName)"items");
            foreach (Item obj in this.Items.ToArray())
                xelement.Add((object)obj.Serialize(true));
            this.LastSeen = DateTime.Now;
            new MySqlCommand(string.Format("UPDATE players SET LastActivity='{0}',Friends='{1}',Notifications='{2}',Achievements='{3}',Stats='{4}',Settings='{5}',Experience='{6}', Avatar='{7}',Items='{8}',RandomBox='{9}',PrivilegieId='{10}',BanType='{11}',UnbanTime='{12}' WHERE  ID={13};", (object)this.LastSeen.ToString("yyyy-MM-ddTHH:mm:ss"), (object)this.friends.InnerXml, (object)this.Notifications.InnerXml, (object)this.Achievements.InnerXml, (object)this.Stats.InnerXml, (object)this.Settings.InnerXml, (object)this.Experience, (object)this.Avatar.ToString(SaveOptions.DisableFormatting), (object)xelement.ToString(SaveOptions.DisableFormatting), (object)this.RandomBox.InnerXml, (object)(byte)this.Privilegie, (object)(byte)this.BanType, (object)this.UnbanTime, (object)this.UserID), SQL.Handler).ExecuteNonQuery();
        }

        internal List<XElement> GeneratePrizes(string BoxName, out int ErrId, int OfferId = 0)
        {
            ErrId = 0;
            List<XElement> xelementList = new List<XElement>();
            if (OfferId > 0 && this.BoxWatcher != null)
                this.BoxWatcher.Stop();
            else if (this.BoxWatcher == null)
                this.BoxWatcher = new Stopwatch();
            try
            {
                int num1 = -1;
                foreach (XmlNode childNode in this.RandomBox.FirstChild.ChildNodes)
                {
                    if (childNode.Attributes["name"].InnerText == BoxName)
                    {
                        num1 = int.Parse(childNode.Attributes["opened"].InnerText);
                        break;
                    }
                }
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load("Gamefiles/ShopItems/" + BoxName + ".xml");
                byte num2 = 0;
                foreach (XmlNode childNode1 in xmlDocument["shop_item"]["random_box"].ChildNodes)
                {
                    SortedDictionary<double, XmlNode> source = new SortedDictionary<double, XmlNode>();
                    List<XmlNode> xmlNodeList1 = new List<XmlNode>();
                    List<XmlNode> xmlNodeList2 = new List<XmlNode>();
                    List<XmlNode> xmlNodeList3 = new List<XmlNode>();
                    foreach (XmlNode childNode2 in childNode1.ChildNodes)
                    {
                        if (!childNode2.Attributes["name"].InnerText.Contains("smugglers_card"))
                        {
                            if (childNode2.Attributes["name"].InnerText.Contains("gold"))
                                xmlNodeList2.Add(childNode2);
                            else if (childNode2.Attributes["amount"] == null && childNode2.Attributes["expiration"] == null)
                                xmlNodeList1.Add(childNode2);
                            xmlNodeList3.Add(childNode2);
                            if (childNode2.Attributes["weight"].InnerText.Length == 1)
                            {
                                double key = double.Parse("0." + childNode2.Attributes["weight"].InnerText, (IFormatProvider)CultureInfo.InvariantCulture);
                                while (source.ContainsKey(key))
                                    key += this.Random.NextDouble();
                                source.Add(key, childNode2);
                            }
                            if (childNode2.Attributes["weight"].InnerText.Length == 2)
                            {
                                double key = double.Parse(childNode2.Attributes["weight"].InnerText.Substring(0, 1) + "." + childNode2.Attributes["weight"].InnerText.Substring(1), (IFormatProvider)CultureInfo.InvariantCulture);
                                while (source.ContainsKey(key))
                                    key += this.Random.NextDouble();
                                source.Add(key, childNode2);
                            }
                            if (childNode2.Attributes["weight"].InnerText.Length == 3)
                            {
                                double key = double.Parse(childNode2.Attributes["weight"].InnerText.Substring(0, 2) + "." + childNode2.Attributes["weight"].InnerText.Substring(2), (IFormatProvider)CultureInfo.InvariantCulture);
                                while (source.ContainsKey(key))
                                    key += this.Random.NextDouble();
                                source.Add(key, childNode2);
                            }
                        }
                    }
                    KeyValuePair<double, XmlNode> keyValuePair = source.FirstOrDefault<KeyValuePair<double, XmlNode>>((Func<KeyValuePair<double, XmlNode>, bool>)(x => true));
                    double key1 = keyValuePair.Key;
                    keyValuePair = source.LastOrDefault<KeyValuePair<double, XmlNode>>((Func<KeyValuePair<double, XmlNode>, bool>)(x => true));
                    double num3 = this.Random.NextDouble(0.0, keyValuePair.Key);
                    List<XmlNode> xmlNodeList4 = new List<XmlNode>();
                    foreach (double key2 in source.Keys)
                    {
                        if (key2 >= num3)
                            xmlNodeList4.Add(source[key2]);
                    }
                    if (xmlNodeList4.Count == 0)
                    {
                        foreach (double key2 in source.Keys)
                        {
                            if (key2 > num3)
                                xmlNodeList4.Add(source[key2]);
                        }
                    }
                    XmlNode xmlNode = xmlNodeList4[this.Random.Next(0, xmlNodeList4.Count)];
                    Item obj1 = (Item)null;
                    if (xmlNodeList2.Count > 0 && num1 % 1000 == 0 && num2 == (byte)0)
                    {
                        Item obj2 = this.AddItem(new Item(ItemType.PERMANENT, this.ItemSeed, xmlNodeList2[this.Random.Next(0, xmlNodeList2.Count)].Attributes["name"].InnerText, 0, 0, 36000L));
                        XElement xelement = new XElement((XName)"profile_item");
                        xelement.Add((object)new XAttribute((XName)"name", (object)obj2.Name));
                        xelement.Add((object)new XAttribute((XName)"profile_item_id", (object)obj2.ID));
                        xelement.Add((object)new XAttribute((XName)"offerId", (object)OfferId));
                        xelement.Add((object)new XAttribute((XName)"added_expiration", (object)0));
                        xelement.Add((object)new XAttribute((XName)"added_quantity", (object)0));
                        xelement.Add((object)new XAttribute((XName)"error_status", (object)"0"));
                        xelement.Add((object)obj2.Serialize(false));
                        xelementList.Add(xelement);
                    }
                    else if (xmlNode.Attributes["name"].InnerText == "game_money_item_01")
                    {
                        this.GameMoney += int.Parse(xmlNode.Attributes["amount"].InnerText);
                        XElement xelement = new XElement((XName)"game_money");
                        xelement.Add((object)new XAttribute((XName)"name", (object)"game_money_item_01"));
                        xelement.Add((object)new XAttribute((XName)"added", (object)xmlNode.Attributes["amount"].InnerText));
                        xelement.Add((object)new XAttribute((XName)"total", (object)this.Experience));
                        xelement.Add((object)new XAttribute((XName)"offerId", (object)OfferId));
                        xelementList.Add(xelement);
                    }
                    else if (xmlNode.Attributes["name"].InnerText == "exp_item_01")
                    {
                        this.Experience += int.Parse(xmlNode.Attributes["amount"].InnerText);
                        XElement xelement = new XElement((XName)"exp");
                        xelement.Add((object)new XAttribute((XName)"name", (object)"exp_item_01"));
                        xelement.Add((object)new XAttribute((XName)"added", (object)xmlNode.Attributes["amount"].InnerText));
                        xelement.Add((object)new XAttribute((XName)"total", (object)this.Experience));
                        xelement.Add((object)new XAttribute((XName)"offerId", (object)OfferId));
                        xelementList.Add(xelement);
                    }
                    else
                    {
                        if (xmlNode.Attributes["expiration"] != null)
                        {
                            int totalHours = int.Parse(new Regex("[0-9]*").Match(xmlNode.Attributes["expiration"].InnerText).Value);
                            if (xmlNode.Attributes["expiration"].InnerText.Contains("d"))
                            {
                                totalHours = (int)TimeSpan.FromDays((double)totalHours).TotalHours;
                                obj1 = new Item(ItemType.TIME, this.ItemSeed, xmlNode.Attributes["name"].InnerText, totalHours, 0, 36000L);
                            }
                            if (xmlNode.Attributes["expiration"].InnerText.Contains("h"))
                                obj1 = new Item(ItemType.TIME, this.ItemSeed, xmlNode.Attributes["name"].InnerText, totalHours, 0, 36000L);
                        }
                        else if (xmlNode.Attributes["amount"] == null && xmlNode.Attributes["expiration"] == null)
                            obj1 = new Item(ItemType.PERMANENT, this.ItemSeed, xmlNode.Attributes["name"].InnerText, 0, 0, 36000L);
                        else if (xmlNode.Attributes["amount"] != null)
                            obj1 = new Item(ItemType.CONSUMABLE, this.ItemSeed, xmlNode.Attributes["name"].InnerText, 0, int.Parse(xmlNode.Attributes["amount"].InnerText), 0L);
                        if (obj1 != null)
                            obj1 = this.AddItem(obj1);
                        XElement xelement = new XElement((XName)"profile_item");
                        xelement.Add((object)new XAttribute((XName)"name", (object)obj1.Name));
                        xelement.Add((object)new XAttribute((XName)"profile_item_id", (object)obj1.ID));
                        xelement.Add((object)new XAttribute((XName)"offerId", (object)OfferId));
                        xelement.Add((object)new XAttribute((XName)"added_expiration", xmlNode.Attributes["expiration"] != null ? (object)xmlNode.Attributes["expiration"].InnerText : (object)""));
                        xelement.Add((object)new XAttribute((XName)"added_quantity", xmlNode.Attributes["amount"] != null ? (object)xmlNode.Attributes["amount"].InnerText : (object)""));
                        xelement.Add((object)new XAttribute((XName)"error_status", (object)"0"));
                        xelement.Add((object)obj1.Serialize(false));
                        xelementList.Add(xelement);
                    }
                    ++num2;
                }
                XmlAttribute attribute1 = this.RandomBox.CreateAttribute("opened");
                attribute1.Value = num1.ToString();
                if (num1 == -1)
                {
                    attribute1.Value = "1";
                    XmlAttribute attribute2 = this.RandomBox.CreateAttribute("name");
                    attribute2.Value = BoxName;
                    XmlElement element = this.RandomBox.CreateElement("box");
                    element.Attributes.Append(attribute2);
                    element.Attributes.Append(attribute1);
                    this.RandomBox.FirstChild.AppendChild((XmlNode)element);
                }
                else
                {
                    int num3 = num1 + 1;
                    foreach (XmlNode childNode in this.RandomBox.FirstChild.ChildNodes)
                    {
                        if (childNode.Attributes["name"].InnerText == BoxName)
                        {
                            childNode.Attributes["opened"].InnerText = num3.ToString();
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrId = 6;
            }
            this.BoxWatcher.Reset();
            this.BoxWatcher.Start();
            return xelementList;
        }

        internal bool Load(bool IncludeClanData = true)
        {
            MySqlCommand mySqlCommand = new MySqlCommand();
            mySqlCommand.Connection = SQL.Handler;
            if (this.UserID != -1L && this.UserID != 0L)
            {
                mySqlCommand.CommandText = string.Format("SELECT * FROM players WHERE ID='{0}';", (object)this.UserID);
            }
            else
            {
                if (this.Nickname == "")
                    return false;
                try
                {
                    long num = long.Parse(new MySqlCommand("SELECT profileid FROM tickets WHERE nick='" + this.Nickname + "';", SQL.Handler).ExecuteScalarAsync().GetAwaiter().GetResult().ToString());
                    mySqlCommand.CommandText = string.Format("SELECT * FROM players WHERE  id='{0}';", (object)num);
                }
                catch
                {
                    return false;
                }
            }
            using (MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader())
            {
                if (!mySqlDataReader.HasRows)
                {
                    mySqlDataReader.Close();
                    return false;
                }
                mySqlDataReader.Read();
                this.UserID = mySqlDataReader.GetInt64(0);
                try
                {
                    if (!(this.Nickname == "") && this.Nickname != null)
                    {
                        if (!(this.Nickname == "NoNameYet"))
                            goto label_14;
                    }
                    this.Nickname = new MySqlCommand(string.Format("SELECT nickname FROM tickets WHERE profileid='{0}';", (object)this.UserID), SQL.Handler).ExecuteScalarAsync().GetAwaiter().GetResult().ToString();
                }
                catch (Exception ex)
                {
                    return false;
                }
            label_14:
                this.Experience = mySqlDataReader.GetInt32(1);
                this.OldRank = this.Rank;
                using (XmlReader xmlReader = XmlReader.Create((TextReader)new StringReader(mySqlDataReader.GetString(2))))
                {
                    xmlReader.Read();
                    this.Gender = xmlReader["gender"].ToString();
                    this.Head = xmlReader["head"].ToString();
                    this.GameMoney = int.Parse(xmlReader["game_money"].ToString());
                    this.CrownMoney = int.Parse(xmlReader["crown_money"].ToString());
                    this.CryMoney = int.Parse(xmlReader["cry_money"].ToString());
                    this.Fatness = double.Parse(xmlReader["fatness"].ToString(), (IFormatProvider)CultureInfo.InvariantCulture);
                    this.Height = double.Parse(xmlReader["height"].ToString(), (IFormatProvider)CultureInfo.InvariantCulture);
                    this.CurrentClass = byte.Parse(xmlReader["current_class"].ToString());
                    this.BannerBadge = int.Parse(xmlReader["banner_badge"].ToString());
                    this.BannerMark = int.Parse(xmlReader["banner_mark"].ToString());
                    this.BannerStripe = int.Parse(xmlReader["banner_stripe"].ToString());
                    this.UnlockedMissions = xmlReader["unlocked_missions"].ToString();
                    this.SoldierPassed = true;
                    this.MedicPassed = true;
                    this.EngineerPassed = true;
                    this.SoldierSuggest = true;
                    this.MedicSuggest = true;
                    this.EngineerSuggest = true;
                }
                try
                {
                    using (XmlReader reader = XmlReader.Create((TextReader)new StringReader(mySqlDataReader.GetString(3))))
                    {
                        this.Items = new List<Item>();
                        XmlDocument xmlDocument = new XmlDocument();
                        reader.Read();
                        xmlDocument.Load(reader);
                        foreach (XmlElement childNode in xmlDocument["items"].ChildNodes)
                        {
                            Item obj = new Item();
                            obj.Create(childNode);
                            this.Items.Add(obj);
                        }
                    }
                }
                catch
                {
                }
                try
                {
                    using (XmlReader reader = XmlReader.Create((TextReader)new StringReader(mySqlDataReader.GetString(4))))
                    {
                        reader.Read();
                        this.Settings.Load(reader);
                    }
                }
                catch
                {
                }
                try
                {
                    using (XmlReader reader = XmlReader.Create((TextReader)new StringReader(mySqlDataReader.GetString(5))))
                    {
                        reader.Read();
                        this.Achievements.Load(reader);
                    }
                }
                catch
                {
                }
                try
                {
                    using (XmlReader reader = XmlReader.Create((TextReader)new StringReader(mySqlDataReader.GetString(6))))
                    {
                        reader.Read();
                        this.notifications.Load(reader);
                    }
                }
                catch
                {
                }
                try
                {
                    using (XmlReader reader = XmlReader.Create((TextReader)new StringReader(mySqlDataReader.GetString(7))))
                    {
                        reader.Read();
                        this.Stats.Load(reader);
                        if (this.Stats["stats"].ChildNodes.Count <= 0)
                            this.Stats.LoadXml("<stats/>");
                        this.StatMgr = new StatsManager(this.Stats);
                    }
                }
                catch
                {
                }
                try
                {
                    using (XmlReader reader = XmlReader.Create((TextReader)new StringReader(mySqlDataReader.GetString(9))))
                    {
                        reader.Read();
                        this.RandomBox.Load(reader);
                    }
                }
                catch
                {
                }
                try
                {
                    using (XmlReader reader = XmlReader.Create((TextReader)new StringReader(mySqlDataReader.GetString(8))))
                    {
                        reader.Read();
                        this.friends.Load(reader);
                    }
                }
                catch
                {
                }
                this.LastSeen = mySqlDataReader.GetDateTime(10);
                this.Privilegie = (PrivilegieId)mySqlDataReader.GetByte(11);
                this.BanType = (BanType)mySqlDataReader.GetByte(12);
                this.UnbanTime = mySqlDataReader.GetInt64(13);
                mySqlDataReader.Close();
            }
            return true;
        }
    }
}
