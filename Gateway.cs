using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.XMPP;
using AuroraServer.XMPP.QUERY;

namespace AuroraServer.NETWORK
{
	internal class Gateway
	{
		public static string LocalIP;

		public static readonly byte[] MagicBytes = new byte[4]
		{
			173,
			222,
			237,
			254
		};
		public static readonly XNamespace JabberNS = "jabber:client";
		private const ushort BufferSize = 16999;
		internal Dictionary<string, int> Connections = new Dictionary<string, int>();
		private Socket _ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

		internal Gateway(int Port = 5222)
        { 
			LocalIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList.ToList().Find((IPAddress Attribute) => Attribute.AddressFamily == AddressFamily.InterNetwork).ToString();
			System.Timers.Timer timer = new System.Timers.Timer();
			timer.Elapsed += delegate
			{
				Connections.Clear();
			};
			timer.Interval = TimeSpan.FromMinutes(15.0).TotalMilliseconds;
			timer.Start();
			int num = Environment.ProcessorCount * 8;
			ThreadPool.SetMinThreads(2, 2);
			ThreadPool.SetMaxThreads(num, num);
			_ServerSocket.Bind(new IPEndPoint(IPAddress.Any, Port));
			_ServerSocket.Listen(0);
			Console.WriteLine();
			Console.ForegroundColor = ConsoleColor.DarkGreen;
			Console.WriteLine($"[{GetType().Name}] Сервер запустился на {Port} порту.");
			Console.ResetColor();
			_ServerSocket.BeginAccept(AcceptAsync, null);
		}

		private void AcceptAsync(IAsyncResult Result)
		{
			try
			{
				Socket socket = _ServerSocket.EndAccept(Result);
				Client client = new Client
				{
					Socket = socket,
					Buffer = new byte[16999]
				};
				//Console.WriteLine("Присоединяется! :" + client.IPAddress);
				if (Connections.ContainsKey(client.IPAddress))
				{
					Dictionary<string, int> connections = Connections;
					string iPAddress = client.IPAddress;
					connections[iPAddress]++;
					if (Connections[client.IPAddress] > 50)
					{
						File.WriteAllText("BADIP.TXT", client.IPAddress);
					}
				}
				else
				{
					Connections.Add(client.IPAddress, 1);
				}
				if (Connections[client.IPAddress] < 11 || client.IPAddress == "127.0.0.1" || File.ReadAllLines("AuthorizedIPs.txt").Contains(client.IPAddress))
				{
					try
					{
						new Thread(ReceiveAsync).Start(client);
					}
					catch
					{
                    }
				}
				//Console.WriteLine(client.IPAddress);
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
			}
			finally
			{
				_ServerSocket.BeginAccept(AcceptAsync, null);
			}
		}

		private void ReceiveAsync(object User1)
		{
			Client client = (Client)User1;
			try
			{
				client.Received = client.Socket.Receive(client.Buffer, client.Buffer.Length, SocketFlags.None);
				while (client.Received != 0 && client.Socket.Connected)
				{
					byte[] array = new byte[client.Received];
					Array.Copy(client.Buffer, array, client.Received);
					BinaryReader binaryReader = new BinaryReader(new MemoryStream(array));
					byte[] array2 = binaryReader.ReadBytes(4);
					long num2 = 0L;
					if (array2[0] == MagicBytes[0] && array2[1] == MagicBytes[1] && array2[2] == MagicBytes[2] && array2[3] == MagicBytes[3])
					{
						num2 = binaryReader.ReadInt64();
					}
					else
					{
						binaryReader.BaseStream.Position = 0L;
						num2 = binaryReader.BaseStream.Length;
					}
					string @string = Encoding.UTF8.GetString(binaryReader.ReadBytes((int)num2));
                    //Console.WriteLine(@string); //Логи пакетов
					if (@string.StartsWith("<"))
					{
						XmlDocument xmlDocument = new XmlDocument();
						if (new Regex("<stream:stream([\\s\\S]+?)>").Matches(@string).Count > 0)
							new StreamStream(client, @string);
						
						else
						{
							try
							{
								xmlDocument.LoadXml(@string);
								if (xmlDocument["starttls"] != null && xmlDocument["starttls"].NamespaceURI == "urn:ietf:params:xml:ns:xmpp-tls")
								{
									new StartTLS(client);
								}
								if (xmlDocument["auth"] != null && xmlDocument["auth"].NamespaceURI == "urn:ietf:params:xml:ns:xmpp-sasl")
								{
									new Auth(client, xmlDocument);
								}
								if (xmlDocument["iq"] != null)
								{
									if (xmlDocument["iq"]["bind"] != null && xmlDocument["iq"]["bind"].NamespaceURI == "urn:ietf:params:xml:ns:xmpp-bind")
									{
										new Bind(client, xmlDocument);
									}
									if (xmlDocument["iq"]["session"] != null && xmlDocument["iq"]["session"].NamespaceURI == "urn:ietf:params:xml:ns:xmpp-session")
									{
										new Session(client, xmlDocument); 
									}
									if (xmlDocument["iq"]["query"] != null && xmlDocument["iq"]["query"].NamespaceURI == "urn:cryonline:k01" && client.Authorized)
									{
										Activator.CreateInstance(Core.MessageFactory.Packets[xmlDocument["iq"]["query"].FirstChild.Name], client, xmlDocument);
									}
                                }
								else if (xmlDocument["message"] != null)
								{
									new Messages(client, xmlDocument);
								}
							}
							catch (KeyNotFoundException)
							{
								Console.ForegroundColor = ConsoleColor.Red;
								Console.WriteLine("[" + client.JID + "] -> [" + xmlDocument["iq"].Attributes["to"].InnerText + "] (UNKNOWN) " + xmlDocument["iq"]["query"].FirstChild.Name + " (" + xmlDocument["iq"].Attributes["type"].InnerText.ToUpper() + ")");
								Activator.CreateInstance(typeof(UnsupportedStanza), client, xmlDocument);
								Console.ResetColor();
                            }
                            catch (Exception ex2)
							{
								bool flag = client.IPAddress == "127.0.0.1";
								//Console.WriteLine(client.IPAddress + " " + ex2);
								if (new Regex("^[0-9]+$").Matches(@string).Count <= 0 && ex2 is XmlException && !client.Dedicated && @string != "protect_init")
								{
									new StreamError(client, "xml-not-well-formed");
                                    break;
                                }
							}
                        }
					}
					client.Buffer = new byte[16999];
					try
					{
						if (client.SslStream != null)
						{
							client.Received = client.SslStream.Read(client.Buffer, 0, client.Buffer.Length);
						}
						else
						{
							client.Received = client.Socket.Receive(client.Buffer);
						}
					}
					catch (Exception)
					{
                        break;
                    }
				}
			}
			catch (Exception)
			{
                Program.WriteLine($"Игрок {client.Player.Nickname} отключился от сервера", ConsoleColor.Red);
                ArrayList.OnlineUsers.Remove(client);
                Console.Title = $"AURORA SERVER (Online: {ArrayList.OnlineUsers.FindAll((Client Attribute) => !Attribute.Dedicated).Count})";
            }
			if (client.Dedicated && client.Player.RoomPlayer.Room != null)
			{
				new MissionUnload(client);
			}
			client.Socket.Dispose();
			if (client.SslStream != null)
			{
				client.SslStream.Dispose();
			}
			ArrayList.OnlineUsers.Remove(client);
			if (client.Channel != null)
			{
				if (client.Player.RoomPlayer.Room != null)
				{
					new GameRoom_Leave(client, null);
				}
				client.Channel.Users.Remove(client);
			}
		}
	}
}
