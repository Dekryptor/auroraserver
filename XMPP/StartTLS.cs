using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using AuroraServer.CLASSES;

namespace AuroraServer.XMPP
{
	internal class StartTLS
	{
		private readonly XNamespace NameSpace = "urn:ietf:params:xml:ns:xmpp-tls";

		internal StartTLS(Client User)
		{
			XDocument xDocument = new XDocument(new XElement(NameSpace + "proceed"));
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
			User.SslStream = new SslStream(new NetworkStream(User.Socket, ownsSocket: true), leaveInnerStreamOpen: true, ServicePointManager.ServerCertificateValidationCallback = ((object _003Cp0_003E, X509Certificate _003Cp1_003E, X509Chain _003Cp2_003E, SslPolicyErrors _003Cp3_003E) => true));
			User.SslStream.AuthenticateAsServer(Core.Certificate, clientCertificateRequired: true, SslProtocols.Tls, checkCertificateRevocation: false);
			//Console.ForegroundColor = ConsoleColor.Green;
			//Console.WriteLine("[" + User.IPAddress + "] -> TLS connection established");
			Console.ResetColor();
		}
	}
}
