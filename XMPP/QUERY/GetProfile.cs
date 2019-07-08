using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using System.Data.SqlClient;
using AuroraServer.NETWORK;
using agsXMPP;

namespace AuroraServer.XMPP.QUERY
{
    internal class GetProfile : Stanza
    {
        private string Channel;

        private long ID;
        private long session_id;

        public GetProfile(Client User, XmlDocument Packet)
            : base(User, Packet)
        {
            ID = long.Parse(Query.Attributes["id"].InnerText);
            session_id = long.Parse(Query.Attributes["session_id"].InnerText);
            Process();
        }

        internal override void Process()
        {
            Client client = ArrayList.OnlineUsers.Find((Client Attribute) => Attribute.Player.UserID == ID);
            XElement xElement = new XElement("profile");
            xElement.Add(new XAttribute("gender", client.Player.Gender));
            xElement.Add(new XAttribute("nickname", client.Player.Nickname));
            xElement.Add(new XAttribute("unlocked_classes", client.Player.UnlockedClasses));
            xElement.Add(new XAttribute("clanName", (client.Player.ClanPlayer.Clan != null) ? client.Player.ClanPlayer.Clan.Name : ""));
            xElement.Add(new XAttribute("user_id", client.Player.TicketId));
            xElement.Add(new XAttribute("preset", "DefaultPreset"));
            xElement.Add(new XAttribute("head", client.Player.Head));
            xElement.Add(new XAttribute("height", client.Player.Height));
            xElement.Add(new XAttribute("fatness", client.Player.Fatness));
            xElement.Add(new XAttribute("current_class", client.Player.CurrentClass));
            xElement.Add(new XAttribute("experience", client.Player.Experience));
            xElement.Add(new XElement("boosts", new XAttribute("xp_boost", "0"), new XAttribute("vp_boost", "0"), new XAttribute("gm_boost", "0"), new XAttribute("ic_boost", "0"), new XAttribute("is_vip", "0")));
            XElement xElement2 = new XElement("items");
            foreach (Item item in client.Player.Items)
            {
                if (item.Equipped > 0 || item.Name.Length <= 5)
                {
                    xElement2.Add(item.Serialize());
                }
            }
            xElement.Add(xElement2);
            XDocument xDocument = new XDocument();
            XElement xElement3 = new XElement(Gateway.JabberNS + "iq");
            xElement3.Add(new XAttribute("type", "result"));
            xElement3.Add(new XAttribute("from", To));
            xElement3.Add(new XAttribute("to", User.JID));
            xElement3.Add(new XAttribute("id", Id));
            XElement xElement4 = new XElement(Stanza.NameSpace + "query");
            XElement xElement5 = new XElement("getprofile");
            xElement5.Add(new XAttribute("id", ID));
            xElement5.Add(xElement);
            xElement4.Add(xElement5);
            xElement3.Add(xElement4);
            xDocument.Add(xElement3);
            Compress(ref xDocument);
            User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
        }
    }
}