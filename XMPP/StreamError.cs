using AuroraServer.CLASSES;

namespace AuroraServer.XMPP
{
	internal class StreamError
	{
		internal string To;

		internal string Xmlns;

		internal string XmlnsUrl;

		internal StreamError(Client User, string Error)
		{
			User.Send("<stream:error><" + Error + " xmlns='urn:ietf:params:xml:ns:xmpp-streams'/></stream:error></stream:stream>");
			User.Socket.Dispose();
			if (User.SslStream != null)
			{
				User.SslStream.Dispose();
			}
		}
	}
}
