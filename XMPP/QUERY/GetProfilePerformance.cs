using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class GetProfilePerformance : Stanza
	{
		private string Channel;

		public GetProfilePerformance(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
            try
            {
                Process();
            }
            catch
            {
            }
		}

		internal override void Process()
		{
			XDocument xDocument = new XDocument();
			XElement xElement = new XElement(Gateway.JabberNS + "iq");
			xElement.Add(new XAttribute("type", "result"));
			xElement.Add(new XAttribute("from", To));
			xElement.Add(new XAttribute("to", User.JID));
			xElement.Add(new XAttribute("id", Id));
			XElement xElement2 = new XElement(Stanza.NameSpace + "query");
			XElement xElement3 = new XElement("get_profile_performance");
			XElement xElement4 = new XElement("pvp_modes_to_complete");
			XElement content = new XElement("pve_missions_performance");
			xElement4.Add(new XElement("mode", "ctf"));
			xElement4.Add(new XElement("mode", "dst"));
			xElement4.Add(new XElement("mode", "ptb"));
			xElement4.Add(new XElement("mode", "lms"));
			xElement4.Add(new XElement("mode", "ffa"));
			xElement4.Add(new XElement("mode", "stm"));
			xElement4.Add(new XElement("mode", "tbs"));
			xElement4.Add(new XElement("mode", "dmn"));
			xElement4.Add(new XElement("mode", "hnt"));
			xElement4.Add(new XElement("mode", "tdm"));
			xElement3.Add(content);
			xElement3.Add(xElement4);
			xElement2.Add(xElement3);
			xElement.Add(xElement2);
			xDocument.Add(xElement);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
