using System;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Threading;

namespace AuroraServer
{
	internal class Program
	{
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]

        private static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            ServicePointManager.ServerCertificateValidationCallback = ((object _003Cp0_003E, X509Certificate _003Cp1_003E, X509Chain _003Cp2_003E, SslPolicyErrors _003Cp3_003E) => true);
            new Core();
            Thread.Sleep(-1);
        }

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Exception ex = (Exception)e.ExceptionObject;
		}

        public static void WriteLine(string Text, ConsoleColor Color)
        {
            //Program.WriteLine($"Если так и есть", ConsoleColor.Yellow);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(string.Format("[{0:00}:{1:00}:{2:00}] ", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second));
            Console.ForegroundColor = Color;
            Console.WriteLine(Text);
            Console.ResetColor();
        }
    }
}
