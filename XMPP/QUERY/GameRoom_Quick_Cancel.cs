using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.CLASSES.GAMEROOM;
using AuroraServer.NETWORK;
using AuroraServer.EXCEPTION;

namespace AuroraServer.XMPP.QUERY
{
    internal class GameRoom_QuickPay_Cancel : Stanza
    {
        public GameRoom_QuickPay_Cancel(Client _User, XmlDocument _Packet)
            : base(_User, _Packet)
        {
            if (base.To == "result") return;
            XDocument Packet = new XDocument();
            XElement iqElement = new XElement(Gateway.JabberNS + "iq");
            iqElement.Add(new XAttribute("type", base.Type));
            iqElement.Add(new XAttribute("from", To));
            iqElement.Add(new XAttribute("to", User.JID));
            iqElement.Add(new XAttribute("id", Id));
            XElement Query = new XElement(Stanza.NameSpace + "query");
            XElement Quick_cencel = new XElement("gameroom_quickplay_cancel");
            Query.Add(Quick_cencel);
            iqElement.Add(Query);
            Packet.Add(iqElement);
            User.Send(Packet.ToString(SaveOptions.DisableFormatting));
        }

    }
}