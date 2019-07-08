using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace AuroraServer.CLASSES.CLAN
{
    internal class Clan
    {
        public long ID = 1;
        public List<long> ProfileIds = new List<long>();
        private XmlDocument ClanInfo = new XmlDocument();
        public string Name;
        public string Description;
        public long CreationTime;
        public int MasterBadge;
        public int MasterMark;
        public int MasterStripe;
        public string MasterNick;
        public int Points;

        public List<Client> OnlineUsers
        {
            get
            {
                return ArrayList.OnlineUsers.FindAll((Predicate<Client>)(Attribute =>
                {
                    if (Attribute.Player.ClanPlayer.Clan != null)
                        return Attribute.Player.ClanPlayer.Clan.ID == this.ID;
                    return false;
                }));
            }
        }

        public List<ClanProperties> UserProperties
        {
            get
            {
                List<ClanProperties> source = new List<ClanProperties>();
                foreach (object childNode in this.ClanInfo["clan_members"].ChildNodes)
                {
                    XmlNode xmlNode = (XmlNode)childNode;
                    long ProfileId = long.Parse(xmlNode.Attributes["profile_id"].InnerText);
                    if (this.ProfileIds.Contains(ProfileId))
                    {
                        Client client = ArrayList.OnlineUsers.Find((Predicate<Client>)(Attribute => Attribute.Player.UserID == ProfileId));
                        if (client != null)
                        {
                            client.Player.ClanPlayer.ProfileId = client.Player.UserID;
                            client.Player.ClanPlayer.Nickname = client.Player.Nickname;
                            client.Player.ClanPlayer.Online = client;
                            client.Player.ClanPlayer.Experience = client.Player.Experience;
                            client.Player.ClanPlayer.InvitationDate = long.Parse(xmlNode.Attributes["invite_date"].InnerText);
                            source.Add(client.Player.ClanPlayer);
                        }
                        else
                        {
                            ClanProperties clanProperties = new ClanProperties()
                            {
                                Clan = this,
                                Experience = int.Parse(xmlNode.Attributes["experience"].InnerText),
                                Nickname = xmlNode.Attributes["nickname"].InnerText,
                                ProfileId = (long)int.Parse(xmlNode.Attributes["profile_id"].InnerText),
                                Online = ArrayList.OnlineUsers.Find((Predicate<Client>)(Attribute => Attribute.Player.UserID == ProfileId)),
                                InvitationDate = long.Parse(xmlNode.Attributes["invite_date"].InnerText),
                                ClanRole = (Clan.CLAN_ROLE)int.Parse(xmlNode.Attributes["clan_role"].InnerText),
                                Points = int.Parse(xmlNode.Attributes["clan_points"].InnerText)
                            };
                            source.Add(clanProperties);
                        }
                    }
                }
                if (source.Find((Predicate<ClanProperties>)(Attribute => Attribute.ClanRole == Clan.CLAN_ROLE.LEADER)) == null)
                {
                    ClanProperties e = source.OrderBy<ClanProperties, int>((Func<ClanProperties, int>)(Attribute => Attribute.Experience)).ToArray<ClanProperties>()[0];
                    source.Find((Predicate<ClanProperties>)(Attribute => Attribute == e)).ClanRole = Clan.CLAN_ROLE.LEADER;
                }
                return source;
            }
        }

        public static void Find(long ID, Player p)
        {
            Client client = ArrayList.OnlineUsers.Find((Predicate<Client>)(Attribute =>
            {
                if (Attribute.Player.ClanPlayer.Clan != null)
                    return Attribute.Player.ClanPlayer.Clan.ID == ID;
                return false;
            }));
            Clan clan;
            if (client == null)
            {
                clan = new Clan() { ID = ID };
                clan.Load();
                if (clan.Name == null)
                    return;
            }
            else
                clan = client.Player.ClanPlayer.Clan;
            ClanProperties clanProperties = clan.UserProperties.ToList<ClanProperties>().Find((Predicate<ClanProperties>)(Attribute => Attribute.ProfileId == p.UserID));
            if (clanProperties == null)
                p.ClanPlayer.Clan = (Clan)null;
            else
                p.ClanPlayer = clanProperties;
        }

        public XElement Serialize()
        {
            int num = 0;
            using (MySqlDataReader mySqlDataReader = new MySqlCommand("SELECT ID FROM clans ORDER BY Points DESC;", SQL.Handler).ExecuteReader())
            {
                while (mySqlDataReader.Read())
                {
                    ++num;
                    if (mySqlDataReader.GetInt64(0) == this.ID)
                    {
                        mySqlDataReader.Close();
                        break;
                    }
                }
                mySqlDataReader.Close();
            }
            this.Points = 0;
            XElement xelement1 = new XElement((XName)"clan");
            try
            {
                xelement1.Add((object)new XAttribute((XName)"name", (object)this.Name));
                xelement1.Add((object)new XAttribute((XName)"description", (object)Convert.ToBase64String(Encoding.UTF8.GetBytes(this.Description))));
                xelement1.Add((object)new XAttribute((XName)"clan_id", (object)this.ID));
                xelement1.Add((object)new XAttribute((XName)"creation_date", (object)this.CreationTime));
                xelement1.Add((object)new XAttribute((XName)"leaderboard_position", (object)num));
                xelement1.Add((object)new XAttribute((XName)"master_badge", (object)this.MasterBadge));
                xelement1.Add((object)new XAttribute((XName)"master_stripe", (object)this.MasterStripe));
                xelement1.Add((object)new XAttribute((XName)"master_mark", (object)this.MasterMark));
                foreach (ClanProperties userProperty in this.UserProperties)
                {
                    try
                    {
                        if (userProperty.ClanRole == Clan.CLAN_ROLE.NOT_IN_CLAN)
                        {
                            this.ProfileIds.Remove(userProperty.ProfileId);
                        }
                        else
                        {
                            this.Points += userProperty.Points;
                            XElement xelement2 = new XElement((XName)"clan_member_info");
                            xelement2.Add((object)new XAttribute((XName)"nickname", (object)userProperty.Nickname));
                            xelement2.Add((object)new XAttribute((XName)"profile_id", (object)userProperty.ProfileId));
                            xelement2.Add((object)new XAttribute((XName)"experience", (object)userProperty.Experience));
                            xelement2.Add((object)new XAttribute((XName)"jid", userProperty.Online != null ? (object)userProperty.Online.JID : (object)""));
                            xelement2.Add((object)new XAttribute((XName)"clan_points", (object)userProperty.Points));
                            xelement2.Add((object)new XAttribute((XName)"invite_date", (object)userProperty.InvitationDate));
                            xelement2.Add((object)new XAttribute((XName)"clan_role", (object)(int)userProperty.ClanRole));
                            xelement2.Add((object)new XAttribute((XName)"status", (object)(userProperty.Online != null ? userProperty.Online.Status : 0)));
                            xelement1.Add((object)xelement2);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
                xelement1.Add((object)new XAttribute((XName)"clan_points", (object)this.Points));
                this.Save();
                return xelement1;
            }
            catch (Exception ex)
            {
                return xelement1;
            }
        }

        public void Load()
        {
            using (MySqlDataReader mySqlDataReader = new MySqlCommand(string.Format("SELECT * FROM clans WHERE ID='{0}';", (object)this.ID), SQL.Handler).ExecuteReader())
            {
                mySqlDataReader.Read();
                if (!mySqlDataReader.HasRows)
                    return;
                try
                {
                    this.Name = mySqlDataReader.GetString(1);
                    this.Description = mySqlDataReader.GetString(2);
                    this.Points = mySqlDataReader.GetInt32(4);
                    XDocument xdocument = XDocument.Parse(mySqlDataReader.GetString(5));
                    this.ClanInfo.LoadXml(xdocument.ToString());
                    foreach (XmlNode childNode in this.ClanInfo["clan_members"].ChildNodes)
                        this.ProfileIds.Add(long.Parse(childNode.Attributes["profile_id"].InnerText));
                    XmlDocument xmlDocument1 = new XmlDocument();
                    XDocument.Parse(mySqlDataReader.GetString(6));
                    string xml = xdocument.ToString();
                    xmlDocument1.LoadXml(xml);
                    XmlDocument xmlDocument2 = new XmlDocument();
                    xmlDocument2.LoadXml(mySqlDataReader.GetString(6));
                    this.MasterBadge = int.Parse(xmlDocument2["master"].Attributes["master_badge"].InnerText);
                    this.MasterMark = int.Parse(xmlDocument2["master"].Attributes["master_mark"].InnerText);
                    this.MasterStripe = int.Parse(xmlDocument2["master"].Attributes["master_stripe"].InnerText);
                    this.MasterNick = xmlDocument2["master"].Attributes["nick"].InnerText;
                }
                catch
                {
                }
            }
        }

        public Clan()
        {
        }

        public void AddMember(
          long id,
          string Nickname,
          int Experience,
          int ClanPoints = 0,
          Clan.CLAN_ROLE role = Clan.CLAN_ROLE.DEFAULT)
        {
            XmlAttribute attribute1 = this.ClanInfo.CreateAttribute("profile_id");
            XmlAttribute attribute2 = this.ClanInfo.CreateAttribute("clan_points");
            XmlAttribute attribute3 = this.ClanInfo.CreateAttribute("nickname");
            XmlAttribute attribute4 = this.ClanInfo.CreateAttribute("experience");
            XmlAttribute attribute5 = this.ClanInfo.CreateAttribute("invite_date");
            XmlAttribute attribute6 = this.ClanInfo.CreateAttribute("clan_role");
            attribute1.Value = id.ToString();
            attribute2.Value = ClanPoints.ToString();
            attribute3.Value = Nickname;
            attribute4.Value = Experience.ToString();
            attribute5.Value = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            attribute6.Value = ((int)role).ToString();
            XmlElement element = this.ClanInfo.CreateElement("clan_member_info");
            element.Attributes.Append(attribute1);
            element.Attributes.Append(attribute3);
            element.Attributes.Append(attribute4);
            element.Attributes.Append(attribute2);
            element.Attributes.Append(attribute5);
            element.Attributes.Append(attribute6);
            if (this.ClanInfo["clan_members"] == null)
                this.ClanInfo.AppendChild(this.ClanInfo.ImportNode((XmlNode)this.ClanInfo.CreateElement("clan_members"), true));
            this.ClanInfo["clan_members"].AppendChild(this.ClanInfo.ImportNode((XmlNode)element, true));
            this.Save();
        }

        public void Save()
        {
            object[] objArray1 = new object[6]
            {
        (object) this.Name,
        (object) this.Description,
        (object) new XElement((XName) "clan_members", (object) this.UserProperties.Select<ClanProperties, XElement>((Func<ClanProperties, XElement>) (peer => new XElement((XName) "clan_member_info", new object[6]
        {
          (object) new XAttribute((XName) "profile_id", (object) peer.ProfileId),
          (object) new XAttribute((XName) "nickname", (object) peer.Nickname),
          (object) new XAttribute((XName) "experience", (object) peer.Experience),
          (object) new XAttribute((XName) "clan_points", (object) peer.Points),
          (object) new XAttribute((XName) "invite_date", (object) peer.InvitationDate),
          (object) new XAttribute((XName) "clan_role", (object) (int) peer.ClanRole)
        })))),
        (object) this.Points,
        null,
        null
            };
            int index = 4;
            XName name = (XName)"master";
            object[] objArray2 = new object[4]
            {
        (object) new XAttribute((XName) "nick", (object) this.UserProperties.Find((Predicate<ClanProperties>) (Attribute => Attribute.ClanRole == Clan.CLAN_ROLE.LEADER)).Nickname),
        (object) new XAttribute((XName) "master_badge", (object) this.MasterBadge),
        (object) new XAttribute((XName) "master_stripe", (object) this.MasterStripe),
        (object) new XAttribute((XName) "master_mark", (object) this.MasterMark)
            };
            objArray1[index] = (object)new XElement(name, objArray2);
            objArray1[5] = (object)this.ID;
            new MySqlCommand(string.Format("UPDATE clans SET Name='{0}', Description='{1}', Players='{2}',Points='{3}',LeaderInfo='{4}' WHERE ID='{5}';", objArray1), SQL.Handler).ExecuteNonQueryAsync();
        }

        public Clan(string Name, string Description, Client Master)
        {
            this.CreationTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            this.Name = Name;
            this.Description = Description;
            this.MasterMark = Master.Player.BannerMark;
            this.MasterStripe = Master.Player.BannerStripe;
            this.MasterBadge = Master.Player.BannerBadge;
            this.MasterNick = Master.Player.Nickname;
            this.ProfileIds.Add(Master.Player.UserID);
            this.AddMember(Master.Player.UserID, Master.Player.Nickname, Master.Player.Experience, 0, Clan.CLAN_ROLE.LEADER);
            Master.Player.ClanPlayer.Clan = this;
            Master.Player.ClanPlayer.ClanRole = Clan.CLAN_ROLE.LEADER;
            Master.Player.ClanPlayer.InvitationDate = DateTimeOffset.Now.ToUnixTimeSeconds();
            this.CreationTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            new MySqlCommand(string.Format("INSERT INTO clans (Name, Description, Players, CreationTime, LeaderInfo) VALUES ('{0}', '{1}', '{2}','{3}','{4}');", (object)this.Name, (object)Description, (object)new XElement((XName)"clan_members", (object)this.UserProperties.Select<ClanProperties, XElement>((Func<ClanProperties, XElement>)(peer => new XElement((XName)"clan_member_info", new object[6]
      {
        (object) new XAttribute((XName) "profile_id", (object) peer.ProfileId),
        (object) new XAttribute((XName) "nickname", (object) peer.Nickname),
        (object) new XAttribute((XName) "experience", (object) peer.Experience),
        (object) new XAttribute((XName) "clan_points", (object) peer.Points),
        (object) new XAttribute((XName) "invite_date", (object) peer.InvitationDate),
        (object) new XAttribute((XName) "clan_role", (object) (int) peer.ClanRole)
      })))), (object)DateTime.Now.ToString(), (object)new XElement((XName)"master", new object[4]
            {
        (object) new XAttribute((XName) "nick", (object) this.MasterNick),
        (object) new XAttribute((XName) "master_badge", (object) this.MasterBadge),
        (object) new XAttribute((XName) "master_stripe", (object) this.MasterStripe),
        (object) new XAttribute((XName) "master_mark", (object) this.MasterMark)
            })), SQL.Handler).ExecuteScalar();
            this.ID = long.Parse(new MySqlCommand("SELECT id FROM clans WHERE name='" + this.Name + "'", SQL.Handler).ExecuteScalar().ToString());
            Master.Player.Save();
        }

        public enum CLAN_ROLE
        {
            NOT_IN_CLAN,
            LEADER,
            CO_LEADER,
            DEFAULT,
        }
    }
}
