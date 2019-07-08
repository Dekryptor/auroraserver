using AuroraServer.CLASSES;
using AuroraServer.NETWORK;
using MySql.Data.MySqlClient;
using System;
using System.Xml;
using System.Xml.Linq;

namespace AuroraServer.XMPP.QUERY
{
    internal class ChannelOperation : Stanza
    {
        private ChannelOperation.Error ErrorCode = ChannelOperation.Error.NO;
        private string Version;
        private string Resource;
        private string RegionId;
        private string BuildType;

        public ChannelOperation(Client User, XmlDocument Packet)
          : base(User, Packet)
        {
            if (User.Channel != null)
            {
                ChannelLogout channelLogout = new ChannelLogout(User, (XmlDocument)null);
            }
            if ((bool)App.Default["UseOldMode"])
                this.Resource = this.To.Split(new string[1]
                {
          "masterserver@warface/"
                }, StringSplitOptions.RemoveEmptyEntries)[0];
            else
                this.Resource = this.Query.Attributes["resource"].InnerText;
            this.BuildType = this.Query.Attributes["build_type"].InnerText;
            User.Channel = !(bool)App.Default["UseOldMode"] ? ArrayList.Channels.Find((Predicate<Channel>)(Attribute => Attribute.Resource == this.Resource)) : ArrayList.Channels.Find((Predicate<Channel>)(Attribute => Attribute.Resource == this.To.Split(new string[1]
         {
        "masterserver@warface/"
         }, StringSplitOptions.RemoveEmptyEntries)[0]));
            if ((int)User.Channel.MinRank >= (int)User.Player.Rank && (int)User.Channel.MaxRank <= (int)User.Player.Rank)
            {
                this.ErrorCode = ChannelOperation.Error.FULL_CHANNEL;
            }
            else
            {
                User.Channel.Users.Add(User);
                if (this.Query.Name == "create_profile" && !User.Player.ProfileCreated)
                {
                    string s = new MySqlCommand("SELECT id FROM tickets WHERE nickname = '" + User.Player.Nickname + "'", SQL.Handler).ExecuteScalar().ToString();
                    new MySqlCommand("INSERT INTO players (ID, Experience, Avatar, Items, Settings, Achievements, Notifications, Stats, Friends, RandomBox, LastActivity, PrivilegieId, BanType, UnbanTime ) VALUES (" + s + ", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 )", SQL.Handler).ExecuteScalar();
                    User.Player = new Player()
                    {
                        TicketId = User.Player.TicketId,
                        Nickname = User.Player.Nickname,
                        UserID = long.Parse(s),
                        Head = this.Query.Attributes["head"].InnerText.StartsWith("default_head_") ? this.Query.Attributes["head"].InnerText : "default_head_04"
                    };
                    Program.WriteLine(string.Format("Игрок {0} создал профиль с ником: {1}, ИД: {2}", (object)User.Player.Nickname, (object)User.Player.Nickname, (object)User.Player.UserID), ConsoleColor.Magenta);
                    new MySqlCommand(string.Format("UPDATE tickets SET profileid='{0}', nickname='{1}' WHERE id='{2}';", (object)User.Player.UserID, (object)User.Player.Nickname, (object)User.Player.TicketId), SQL.Handler).ExecuteScalar();
                    User.Player.Save();
                }
                this.Process();
                Program.WriteLine(string.Format("Игрок {0} присоединился к каналу {1} (На канале: {2} игроков)", (object)User.Player.Nickname, (object)User.Channel.Resource, (object)User.Channel.Online), ConsoleColor.Yellow);
                int num = User.IPAddress == "127.0.0.1" ? 1 : 0;
                new FriendList(User, (XmlDocument)null).Process();
            }
        }

        internal override void Process()
        {
            XDocument xDocument = new XDocument();
            XElement xelement1 = new XElement(Gateway.JabberNS + "iq");
            xelement1.Add((object)new XAttribute((XName)"type", (object)"result"));
            xelement1.Add((object)new XAttribute((XName)"from", (object)this.To));
            xelement1.Add((object)new XAttribute((XName)"to", (object)this.User.JID));
            xelement1.Add((object)new XAttribute((XName)"id", (object)this.Id));
            XElement xelement2 = new XElement(Stanza.NameSpace + "query");
            XElement xelement3 = new XElement((XName)this.Query.Name);
            if (this.ErrorCode == ChannelOperation.Error.NO)
            {
                xelement3.Add((object)new XAttribute((XName)"profile_id", (object)this.User.Player.UserID));
                XElement xelement4 = new XElement((XName)"character");
                xelement4.Add((object)new XAttribute((XName)"nick", (object)this.User.Player.Nickname));
                xelement4.Add((object)new XAttribute((XName)"gender", (object)this.User.Player.Gender));
                xelement4.Add((object)new XAttribute((XName)"height", (object)this.User.Player.Height));
                xelement4.Add((object)new XAttribute((XName)"fatness", (object)this.User.Player.Fatness));
                xelement4.Add((object)new XAttribute((XName)"head", (object)this.User.Player.Head));
                xelement4.Add((object)new XAttribute((XName)"current_class", (object)this.User.Player.CurrentClass));
                xelement4.Add((object)new XAttribute((XName)"experience", (object)this.User.Player.Experience));
                xelement4.Add((object)new XAttribute((XName)"pvp_rating", (object)"0"));
                xelement4.Add((object)new XAttribute((XName)"pvp_rating_points", (object)"0"));
                xelement4.Add((object)new XAttribute((XName)"banner_badge", (object)this.User.Player.BannerBadge));
                xelement4.Add((object)new XAttribute((XName)"banner_mark", (object)this.User.Player.BannerMark));
                xelement4.Add((object)new XAttribute((XName)"banner_stripe", (object)this.User.Player.BannerStripe));
                xelement4.Add((object)new XAttribute((XName)"game_money", (object)this.User.Player.GameMoney));
                xelement4.Add((object)new XAttribute((XName)"cry_money", (object)this.User.Player.CryMoney));
                xelement4.Add((object)new XAttribute((XName)"crown_money", (object)this.User.Player.CrownMoney));
                XElement xelement5 = new XElement((XName)"sponsor_info");
                xelement5.Add((object)new XElement((XName)"sponsor", new object[3]
                {
          (object) new XAttribute((XName) "sponsor_id", (object) "0"),
          (object) new XAttribute((XName) "sponsor_points", (object) "0"),
          (object) new XAttribute((XName) "next_unlock_item", (object) "0")
                }));
                xelement5.Add((object)new XElement((XName)"sponsor", new object[3]
                {
          (object) new XAttribute((XName) "sponsor_id", (object) "1"),
          (object) new XAttribute((XName) "sponsor_points", (object) "0"),
          (object) new XAttribute((XName) "next_unlock_item", (object) "0")
                }));
                xelement5.Add((object)new XElement((XName)"sponsor", new object[3]
                {
          (object) new XAttribute((XName) "sponsor_id", (object) "2"),
          (object) new XAttribute((XName) "sponsor_points", (object) "0"),
          (object) new XAttribute((XName) "next_unlock_item", (object) "0")
                }));
                if (this.User.Player.Notifications.FirstChild.ChildNodes.Count > 0)
                {
                    foreach (XmlNode childNode in this.User.Player.Notifications.FirstChild.ChildNodes)
                        xelement4.Add((object)XDocument.Parse(childNode.OuterXml).Root);
                }
                xelement4.Add((object)new XElement((XName)"login_bonus", new object[2]
                {
          (object) new XAttribute((XName) "current_streak", (object) "1"),
          (object) new XAttribute((XName) "current_reward", (object) "0")
                }));
                XElement xelement6 = new XElement((XName)"profile_progression_state");
                xelement6.Add((object)new XAttribute((XName)"profile_id", (object)this.User.Player.UserID));
                xelement6.Add((object)new XAttribute((XName)"mission_unlocked", (object)this.User.Player.UnlockedMissions));
                xelement6.Add((object)new XAttribute((XName)"tutorial_unlocked", (object)this.User.Player.TutorialSuggest));
                xelement6.Add((object)new XAttribute((XName)"tutorial_passed", (object)this.User.Player.TutorialPassed));
                xelement6.Add((object)new XAttribute((XName)"class_unlocked", (object)this.User.Player.UnlockedClasses));
                XElement xelement7 = new XElement((XName)"chat_channels");
                XElement xelement8 = new XElement((XName)"chat");
                xelement8.Add((object)new XAttribute((XName)"channel", (object)0));
                xelement8.Add((object)new XAttribute((XName)"channel_id", (object)this.User.Channel.JID));
                xelement8.Add((object)new XAttribute((XName)"service_id", (object)"conference.warface"));
                xelement7.Add((object)xelement8);
                foreach (AuroraServer.CLASSES.Item obj in this.User.Player.Items)
                {
                    if (obj.ItemType == AuroraServer.CLASSES.ItemType.TIME && obj.SecondsLeft <= 0L && !obj.ExpiredConfirmed)
                    {
                        xelement4.Add((object)new XElement((XName)"expired_item", new object[3]
                        {
              (object) new XAttribute((XName) "id", (object) obj.ID),
              (object) new XAttribute((XName) "name", (object) obj.Name),
              (object) new XAttribute((XName) "slot_ids", (object) obj.Slot)
                        }));
                        obj.ExpiredConfirmed = true;
                    }
                    xelement4.Add((object)obj.Serialize(false));
                }
                xelement4.Add((object)xelement7);
                xelement4.Add((object)xelement5);
                xelement4.Add((object)xelement6);
                xelement4.Add((object)GameResources.OnlineVariables.ToXDocument().FirstNode);
                xelement3.Add((object)xelement4);
                xelement2.Add((object)xelement3);
                xelement1.Add((object)xelement2);
            }
            else
            {
                XElement xelement4 = new XElement((XNamespace)"urn:ietf:params:xml:ns:xmpp-stanzas" + "error");
                xelement4.Add((object)new XAttribute((XName)"type", (object)"cancel"));
                xelement4.Add((object)new XAttribute((XName)"code", (object)8));
                xelement4.Add((object)new XAttribute((XName)"custom_code", (object)(int)this.ErrorCode));
                xelement2.Add((object)xelement3);
                xelement1.Add((object)xelement2);
                xelement1.Add((object)xelement4);
            }
            xDocument.Add((object)xelement1);
            this.Compress(ref xDocument);
            this.User.Player.Save();
            this.User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
        }

        private enum Error
        {
            NO = -1,
            PROFILE_NOT_EXIST = 1,
            INVALID_GAMEVERSION = 2,
            BANNED = 3,
            FULL_CHANNEL = 5,
        }
    }
}
