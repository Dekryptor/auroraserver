using AuroraServer.CLASSES;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace AuroraServer.XMPP
{
    internal class Auth
    {
        public static WebClient WClient = new WebClient();
        private readonly XNamespace NameSpace = (XNamespace)"urn:ietf:params:xml:ns:xmpp-sasl";

        internal Auth(Client User, XmlDocument Packet)
        {
            try
            {
                string[] strArray = Encoding.UTF8.GetString(Convert.FromBase64String(Packet["auth"].InnerText)).Split(new char[1], StringSplitOptions.RemoveEmptyEntries);
                if (strArray[0] != "dedicated" && strArray[1] != "dedicated")
                {
                    Player player = new Player();
                    using (MySqlDataReader mySqlDataReader = new MySqlCommand("SELECT id,token,nickname,profileid FROM tickets WHERE nickname='" + strArray[1] + "';", AuroraServer.SQL.Handler).ExecuteReader())
                    {
                        try
                        {
                            if (mySqlDataReader.HasRows)
                            {
                                mySqlDataReader.Read();
                                string str = mySqlDataReader.GetString(1);
                                player.TicketId = mySqlDataReader.GetInt64(0);
                                player.Nickname = mySqlDataReader.GetString(2);
                                player.UserID = mySqlDataReader.GetInt64(3);
                                mySqlDataReader.Close();
                                if (player.Nickname == "")
                                    player.Nickname = string.Format("user{0}", (object)player.UserID);
                                if (strArray[0].Replace("{:B:}row_emul", "").ToLower() == str.ToLower())
                                {
                                    if (player.UserID != 0L && !player.Load(true))
                                    {
                                        Program.WriteLine("Игрок " + strArray[1] + " не может авторизироваться!", ConsoleColor.Red);
                                        User.Send(new XDocument(new object[1]
                                        {
                      (object) new XElement(this.NameSpace + "failure")
                                        }).ToString(SaveOptions.DisableFormatting));
                                    }
                                    else
                                    {
                                        User.Authorized = true;
                                        if (player.BanType == BanType.ALL_PERMANENT)
                                        {
                                            Program.WriteLine("Игрок " + strArray[1] + " забанен навсегда!", ConsoleColor.Red);
                                            User.Send(new XDocument(new object[1]
                                            {
                        (object) new XElement(this.NameSpace + "failure")
                                            }).ToString(SaveOptions.DisableFormatting));
                                        }
                                        else
                                        {
                                            if (player.BanType == BanType.ALL)
                                            {
                                                TimeSpan timeSpan = DateTimeOffset.FromUnixTimeSeconds(player.UnbanTime).Subtract(DateTimeOffset.UtcNow);
                                                if (timeSpan.TotalSeconds <= 0.0)
                                                {
                                                    player.BanType = BanType.NO;
                                                }
                                                else
                                                {
                                                    Program.WriteLine("Игрок " + strArray[1] + " забанен на " + timeSpan.ToString() + "!", ConsoleColor.Red);
                                                    User.Send(new XDocument(new object[1]
                                                    {
                            (object) new XElement(this.NameSpace + "failure")
                                                    }).ToString(SaveOptions.DisableFormatting));
                                                    return;
                                                }
                                            }
                                            if (player != null)
                                            {
                                                if (User.Player == null)
                                                    User.Player = player;
                                                ArrayList.OnlineUsers.Add(User);
                                            }
                                            Program.WriteLine("Игрок " + strArray[1] + " присоединился к серверу!", ConsoleColor.Green);
                                            User.Send(new XDocument(new object[1]
                                            {
                        (object) new XElement(this.NameSpace + "success")
                                            }).ToString(SaveOptions.DisableFormatting));
                                            Console.Title = string.Format("AURORA SERVER (Online: {0})", (object)ArrayList.OnlineUsers.FindAll((Predicate<Client>)(Attribute => !Attribute.Dedicated)).Count);
                                            MySqlCommand mySqlCommand = new MySqlCommand("UPDATE system SET value=@value WHERE system_cvar=@value2", AuroraServer.SQL.Handler);
                                            mySqlCommand.Parameters.AddWithValue("value2", (object)"online");
                                            mySqlCommand.Parameters.AddWithValue("value", (object)ArrayList.OnlineUsers.FindAll((Predicate<Client>)(Attribute => !Attribute.Dedicated)).Count);
                                            mySqlCommand.ExecuteNonQuery();
                                            User.Player = player;
                                        }
                                    }
                                }
                                else
                                {
                                    Program.WriteLine("Игрок " + strArray[1] + " не может авторизироваться. Неверный пароль!", ConsoleColor.Red);
                                    User.Send(new XDocument(new object[1]
                                    {
                    (object) new XElement(this.NameSpace + "failure")
                                    }).ToString(SaveOptions.DisableFormatting));
                                }
                            }
                            else
                                User.Send(new XDocument(new object[1]
                                {
                  (object) new XElement(this.NameSpace + "failure")
                                }).ToString(SaveOptions.DisableFormatting));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine((object)ex);
                        }
                    }
                }
                else if (!((IEnumerable<string>)System.IO.File.ReadAllLines("AuthorizedIPs.txt")).Contains<string>(User.IPAddress))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[" + User.IPAddress + "] Нельзя запустить выделенный сервер с данного IP!");
                    Console.ResetColor();
                    User.Dispose();
                }
                else
                {
                    Player player = new Player();
                    User.Dedicated = true;
                    User.Player = player;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[" + User.IPAddress + "] Выделенный сервер подключен!");
                    Console.ResetColor();
                    XDocument xdocument = new XDocument(new object[1]
                    {
            (object) new XElement(this.NameSpace + "success")
                    });
                    User.Authorized = true;
                    ArrayList.OnlineUsers.Add(User);
                    User.Send(xdocument.ToString(SaveOptions.DisableFormatting));
                }
            }
            catch (SqlException ex)
            {
                if (!ex.Message.Contains("Закрыто"))
                    return;
                AuroraServer.SQL.Handler.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine((object)ex);
            }
        }
    }
}
