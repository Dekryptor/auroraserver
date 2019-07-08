using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class BroadcastSessionResults : Stanza
	{
		internal List<XElement> SessionResults;

		public BroadcastSessionResults(Client User, XmlDocument Packet)
			: base(User, Packet)
		{

		}

		public BroadcastSessionResults(Client User, List<XElement> Results)
			: base(User, null)
		{
			SessionResults = Results;
		}

		internal override void Process()
		{
			XDocument xDocument = new XDocument();
			XElement xElement = new XElement(Gateway.JabberNS + "iq");
			xElement.Add(new XAttribute("type", "get"));
			xElement.Add(new XAttribute("from", "k01.warface"));
			xElement.Add(new XAttribute("to", User.JID));
			xElement.Add(new XAttribute("id", User.Player.Random.Next(999999, int.MaxValue)));
			XElement xElement2 = new XElement(Stanza.NameSpace + "query");
			XElement xElement3 = new XElement("broadcast_session_result");
			foreach (XElement sessionResult in SessionResults)
			{
				XElement xElement4 = sessionResult;
				xElement3.Add(SessionResults);
			}
			xElement2.Add(xElement3);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			Compress(ref xDocument);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
