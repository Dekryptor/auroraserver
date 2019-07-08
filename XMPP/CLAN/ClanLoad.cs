using AuroraServer.CLASSES;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace AuroraServer.XMPP.QUERY
{
    internal class ClanLoad : Stanza
    {
        public long ID = 1;
        public List<long> ProfileIds = new List<long>();
        private XmlDocument ClanInfo = new XmlDocument();
        public new string Name;
        public string Description;
        public long CreationTime;
        public int MasterBadge;
        public int MasterMark;
        public int MasterStripe;
        public string MasterNick;
        public int Points;
        public int ClanRole;
        public int Profileid;

        public ClanLoad(Client User, XmlDocument Packet = null)
          : base(User, Packet)
        {
        }

        internal override void Process()
        {
            using (MySqlDataReader mySqlDataReader = new MySqlCommand("SELECT * FROM clans WHERE ID='55';", SQL.Handler).ExecuteReader())
            {
                mySqlDataReader.Read();
                if (mySqlDataReader.HasRows)
                {
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
                        xmlDocument1.LoadXml(xdocument.ToString());
                        XmlDocument xmlDocument2 = new XmlDocument();
                        xmlDocument2.LoadXml(mySqlDataReader.GetString(6));
                        this.MasterBadge = int.Parse(xmlDocument2["master"].Attributes["master_badge"].InnerText);
                        this.MasterMark = int.Parse(xmlDocument2["master"].Attributes["master_mark"].InnerText);
                        this.MasterStripe = int.Parse(xmlDocument2["master"].Attributes["master_stripe"].InnerText);
                        this.MasterNick = xmlDocument2["master"].Attributes["nick"].InnerText;
                        this.Profileid = int.Parse(xmlDocument1["clan_member_info"].Attributes["profile_id"].InnerText);
                        this.ClanRole = int.Parse(xmlDocument1["clan_member_info"].Attributes["clan_role"].InnerText);
                    }
                    catch
                    {
                    }
                }
            }
            XDocument xdocument1 = new XDocument();
            XElement xelement1 = new XElement((XName)"iq");
            xelement1.Add((object)new XAttribute((XName)"type", this.Type == "get" ? (object)"result" : (object)"get"));
            xelement1.Add((object)new XAttribute((XName)"from", (object)"k01.warface"));
            xelement1.Add((object)new XAttribute((XName)"to", (object)this.User.JID));
            xelement1.Add((object)new XAttribute((XName)"id", this.Type == "get" ? (object)this.Id : (object)("uid" + this.User.Player.Random.Next(9999, int.MaxValue).ToString("x8"))));
            XElement xelement2 = new XElement(Stanza.NameSpace + "query");
            XElement xelement3 = new XElement((XName)"clan_info");
            XElement xelement4 = new XElement((XName)"clan");
            try
            {
                xelement4.Add((object)new XAttribute((XName)"name", (object)this.Name));
                xelement4.Add((object)new XAttribute((XName)"description", (object)this.Description));
                xelement4.Add((object)new XAttribute((XName)"clan_id", (object)"1"));
                xelement4.Add((object)new XAttribute((XName)"creation_date", (object)this.CreationTime));
                xelement4.Add((object)new XAttribute((XName)"leaderboard_position", (object)"1"));
                xelement4.Add((object)new XAttribute((XName)"master_badge", (object)this.MasterBadge));
                xelement4.Add((object)new XAttribute((XName)"master_stripe", (object)this.MasterStripe));
                xelement4.Add((object)new XAttribute((XName)"master_mark", (object)this.MasterMark));
                xelement4.Add((object)new XAttribute((XName)"clan_points", (object)"0"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            XElement xelement5 = new XElement((XName)"clan_member_info");
            xelement5.Add((object)new XAttribute((XName)"nickname", (object)this.MasterNick));
            xelement5.Add((object)new XAttribute((XName)"profile_id", (object)"1"));
            xelement5.Add((object)new XAttribute((XName)"experience", (object)"413979"));
            xelement5.Add((object)new XAttribute((XName)"jid", (object)this.User.JID));
            xelement5.Add((object)new XAttribute((XName)"clan_points", (object)"0"));
            xelement5.Add((object)new XAttribute((XName)"invite_date", (object)"1556976988"));
            xelement5.Add((object)new XAttribute((XName)"clan_role", (object)this.ClanRole));
            xelement5.Add((object)new XAttribute((XName)"status", (object)"9"));
            xelement4.Add((object)xelement5);
            xelement3.Add((object)xelement4);
            xelement2.Add((object)xelement3);
            xelement1.Add((object)xelement2);
            xdocument1.Add((object)xelement1);
            this.User.Send(xdocument1.ToString(SaveOptions.DisableFormatting));
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
