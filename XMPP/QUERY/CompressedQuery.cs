using System;
using System.Xml;
using AuroraServer.CLASSES;

namespace AuroraServer.XMPP.QUERY
{
	internal class CompressedQuery : Stanza
	{
		public CompressedQuery(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
			Uncompress(ref Packet);
			string key = Packet.FirstChild.FirstChild.FirstChild.Name.Replace(":", "_");
			Type type = Core.MessageFactory.Packets[key];
			if (type != null)
			{
				Activator.CreateInstance(type, User, Packet);
			}
		}
	}
}
