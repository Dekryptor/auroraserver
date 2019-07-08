using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.CLASSES.GAMEROOM;
using AuroraServer.NETWORK;
using AuroraServer.EXCEPTION;

namespace AuroraServer.XMPP.QUERY
{
    internal class GameRoom_QuickPay : Stanza
    {
        public GameRoom_QuickPay(Client User, XmlDocument Packet)
            : base(User, Packet)
        {


        }
    }
}
