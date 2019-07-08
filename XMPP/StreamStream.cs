using System.Text.RegularExpressions;
using AuroraServer.CLASSES;

namespace AuroraServer.XMPP
{
	internal class StreamStream
	{
		internal string To;

		internal string Xmlns;

		internal string XmlnsUrl;

		internal StreamStream(Client User, string Packet)
		{
			MatchCollection matchCollection = new Regex("(?:to=')([\\s\\S]+?)'|xmlns='([\\s\\S]+?)'|xmlns:stream='([\\s\\S]+?)'").Matches(Packet);
			To = matchCollection[0].Value;
			Xmlns = matchCollection[1].Value;
			XmlnsUrl = matchCollection[1].Value;
			User.Send("<stream:features>" + ((User.SslStream == null && !User.Authorized) ? "<starttls xmlns='urn:ietf:params:xml:ns:xmpp-tls'/>" : "") + ((!User.Authorized) ? "<mechanisms xmlns='urn:ietf:params:xml:ns:xmpp-sasl'><mechanism>WARFACE</mechanism></mechanisms>" : "<bind xmlns='urn:ietf:params:xml:ns:xmpp-bind' /><session xmlns='urn:ietf:params:xml:ns:xmpp-session' />") + "</stream:features>");
		}
	}
}
