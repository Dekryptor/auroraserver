using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;

namespace AuroraServer.XMPP.QUERY
{
	internal class ValidatePlayerInfo : Stanza
	{
		public ValidatePlayerInfo(Client User, XmlDocument Packet = null)
			: base(User, Packet)
		{
			Process();
		}

		internal override void Process()
		{
			if (!(Type == "result"))
			{
				XDocument xDocument = new XDocument();
				XElement xElement = new XElement("iq");
				xElement.Add(new XAttribute("type", (Type == "get") ? "result" : "get"));
				xElement.Add(new XAttribute("from", "k01.warface"));
				xElement.Add(new XAttribute("to", User.JID));
				xElement.Add(new XAttribute("id", (Type == "get") ? Id : ("uid" + User.Player.Random.Next(9999, int.MaxValue).ToString("x8"))));
				XElement xElement2 = new XElement(Stanza.NameSpace + "query");
				XElement content = new XElement("validate_player_info");
				xElement2.Add(content);
				xElement.Add(xElement2);
				xDocument.Add(xElement);
				User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
			}
		}
	}
}
