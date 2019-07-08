using System;
using System.Security.Cryptography.X509Certificates;
using AuroraServer.CLASSES;
using AuroraServer.NETWORK;
using AuroraServer.XMPP.QUERY;


namespace AuroraServer
{
	internal class Core
	{
		internal static X509Certificate2 Certificate;
		internal static MessageFactory MessageFactory;
		internal Core()
		{
			Console.WriteLine();
			Certificate = new X509Certificate2("Cert/Server.pfx");
			Console.ForegroundColor = ConsoleColor.DarkMagenta;
			Console.WriteLine("[CertificateManager] Certificate X509 is loaded");
			MessageFactory = new MessageFactory();
			new ArrayList();
			new GameResources();
			new SQL();
			ArrayList.Channels.Add(new Channel("pve_001", 1, "pve", 1, 90));
			ArrayList.Channels.Add(new Channel("pvp_pro_3", 301, "pvp_pro", 1, 90));
            new Gateway();
		}
	}
}
