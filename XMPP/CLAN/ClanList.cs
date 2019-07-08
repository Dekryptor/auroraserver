
using AuroraServer.CLASSES;
using AuroraServer.NETWORK;
using MySql.Data.MySqlClient;
using System;
using System.Xml;
using System.Xml.Linq;

namespace AuroraServer.XMPP.QUERY
{
    internal class ClanList : Stanza
    {
        public ClanList(Client User, XmlDocument Packet)
          : base(User, Packet)
        {
            this.Process();
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
            XElement xelement3 = new XElement((XName)"clan_list");
            XElement xelement4 = (XElement)null;
            if (this.User.Player.ClanPlayer.Clan == null)
            {
                xelement4 = new XElement((XName)"clan_performance", (object)new XAttribute((XName)"position", (object)0));
            }
            else
            {
                using (MySqlDataReader mySqlDataReader = new MySqlCommand("SELECT ID FROM clans ORDER BY Points DESC;", SQL.Handler).ExecuteReader())
                {
                    try
                    {
                        int num = 0;
                        while (mySqlDataReader.Read())
                        {
                            ++num;
                            if (mySqlDataReader.GetInt64(0) == this.User.Player.ClanPlayer.Clan.ID)
                            {
                                xelement4 = new XElement((XName)"clan_performance", (object)new XAttribute((XName)"position", (object)num));
                                mySqlDataReader.Close();
                                break;
                            }
                        }
                        mySqlDataReader.Close();
                    }
                    catch (Exception ex)
                    {
                    }
                    finally
                    {
                        mySqlDataReader.Close();
                    }
                }
            }
            using (MySqlDataReader mySqlDataReader = new MySqlCommand("SELECT * FROM clans;", SQL.Handler).ExecuteReader())
            {
                try
                {
                    while (mySqlDataReader.Read())
                    {
                        XmlDocument xmlDocument1 = new XmlDocument();
                        XmlDocument xmlDocument2 = new XmlDocument();
                        XDocument xdocument1 = XDocument.Parse(mySqlDataReader.GetString(5));
                        XDocument xdocument2 = XDocument.Parse(mySqlDataReader.GetString(6));
                        xmlDocument2.LoadXml(xdocument1.ToString());
                        xmlDocument1.LoadXml(xdocument2.ToString());
                        xelement4.Add((object)new XElement((XName)"clan", new object[8]
                        {
              (object) new XAttribute((XName) "name", (object) mySqlDataReader.GetString(1)),
              (object) new XAttribute((XName) "clan_id", (object) mySqlDataReader.GetInt64(0)),
              (object) new XAttribute((XName) "master", (object) xmlDocument1.FirstChild.Attributes["nick"].InnerText),
              (object) new XAttribute((XName) "clan_points", (object) mySqlDataReader.GetInt32(4)),
              (object) new XAttribute((XName) "members", (object) xmlDocument2.FirstChild.ChildNodes.Count),
              (object) new XAttribute((XName) "master_badge", (object) xmlDocument1.FirstChild.Attributes["master_badge"].InnerText),
              (object) new XAttribute((XName) "master_stripe", (object) xmlDocument1.FirstChild.Attributes["master_stripe"].InnerText),
              (object) new XAttribute((XName) "master_mark", (object) xmlDocument1.FirstChild.Attributes["master_mark"].InnerText)
                        }));
                    }
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    mySqlDataReader.Close();
                }
            }
            xelement3.Add((object)xelement4);
            xelement2.Add((object)xelement3);
            xelement1.Add((object)xelement2);
            xDocument.Add((object)xelement1);
            this.Compress(ref xDocument);
            this.User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
        }
    }
}
