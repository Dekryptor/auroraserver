using System;
using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP
{
	internal class Bind
	{
		internal int DedicatedId;

		private readonly XNamespace NameSpace = "urn:ietf:params:xml:ns:xmpp-bind";

		internal Bind(Client User, XmlDocument Packet)
		{
			Client client = ArrayList.OnlineUsers.Find((Client Attribute) => Attribute.JID == $"{User.Player.TicketId}@warface/GameClient");
			if (client != null)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("[" + User.JID + "] -> Bind in use! Destroying current session!");
				Console.ResetColor();
				new StreamError(client, "conflict");
			}
			if (User.Dedicated)
			{
				User.JID = $"dedicated{DedicatedId}@warface/GameDedicated";
			}
			else
			{
				User.JID = $"{User.Player.Nickname.ToLower()}@row_emul.warface/GameClient";
			}
            string AdminStatus = "";
            //if (User.Player.Privilegie == PrivilegieId.ADMINISTRATOR)
            //    AdminStatus = "[ADMIN] ";
            //else if (User.Player.Privilegie == PrivilegieId.MODERATOR)
            //    AdminStatus = "[MODER] ";
            //else AdminStatus = "";
            User.Player.Nickname = AdminStatus + User.Player.Nickname;

            /*Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("[" + User.IPAddress + "] -> Binded XMPP JID [" + User.JID + "]");
			Console.ResetColor();*/
			XElement xElement = new XElement(Gateway.JabberNS + "iq");
			xElement.Add(new XAttribute("type", "result"));
			xElement.Add(new XAttribute("id", Packet["iq"].Attributes["id"].InnerText));
			xElement.Add(new XAttribute("to", User.JID));
			XElement xElement2 = new XElement(NameSpace + "bind");
			xElement2.Add(new XElement("jid", User.JID));
			xElement.Add(xElement2);
			User.Send(xElement.ToString(SaveOptions.DisableFormatting));
		}
	}
}
