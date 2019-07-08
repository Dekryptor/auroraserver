using Ionic.Zlib;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace AuroraServer
{
	internal class Tools
	{
		internal class DeflateTool
		{
			internal static string Decode(string base64encoded)=>
                Encoding.UTF8.GetString(ZlibStream.UncompressBuffer(Convert.FromBase64String(base64encoded)));

			internal static string Encode(string text)=>
				Convert.ToBase64String(ZlibStream.CompressBuffer(Encoding.UTF8.GetBytes(text)));
		}

		public static long GetTotalSeconds(string Value)
		{
			try
			{
				long num = long.Parse(new Regex("[0-9]*").Matches(Value)[0].ToString());
				char result = 's';
				char.TryParse(new Regex("[a-z]").Matches(Value)[0].Value, out result);
				switch (result)
				{
				case 'm':
					return (long)TimeSpan.FromMinutes(num).TotalSeconds;
				case 'h':
					return (long)TimeSpan.FromHours(num).TotalSeconds;
				case 'd':
					return (long)TimeSpan.FromDays(num).TotalSeconds;
				default:
					return num;
				}
			}
			catch
			{
				return -1;
			}
		}
	}
}
