using AuroraServer.CLASSES;
using AuroraServer.NETWORK;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace AuroraServer.XMPP.QUERY
{
    internal class Messages : Stanza
    {
        private string Channel;
        public XElement MessageElement;

        public Messages(Client User, XmlDocument Packet)
          : base(User, Packet)
        {
            Messages messages = this;
            if (this.Packet["message"] != null)
                Program.WriteLine("Игрок " + User.Player.Nickname + " отправил сообщение в " + User.Channel.Resource + ": " + this.Packet["message"].InnerText, ConsoleColor.DarkYellow);
            if (!User.ChatWatcher.ContainsKey(this.To))
                User.ChatWatcher.Add(this.To, new Stopwatch());
            Stopwatch stopwatch = User.ChatWatcher[this.To];
            if (stopwatch.Elapsed.TotalMilliseconds != 0.0 && stopwatch.Elapsed.TotalSeconds < 1.0)
            {
                StreamError streamError = new StreamError(User, "policy-violation");
            }
            else if (stopwatch.Elapsed.TotalMilliseconds != 0.0 && stopwatch.Elapsed.TotalSeconds < 3.0)
            {
                this.MessageElement = new XElement((XName)"message", new object[5]
                {
          (object) new XAttribute((XName) "from", (object) this.To),
          (object) new XAttribute((XName) "to", (object) User.JID),
          (object) new XAttribute((XName) "type", (object) "error"),
          (object) new XElement((XName) "body", (object) ""),
          (object) new XElement((XName) "error", new object[3]
          {
            (object) new XAttribute((XName) "code", (object) "503"),
            (object) new XAttribute((XName) "type", (object) "cancel"),
            (object) new XElement((XNamespace) "urn:ietf:params:xml:ns:xmpp-stanzas" + "service-unavailable")
          })
                });
                User.Send(this.MessageElement.ToString(SaveOptions.DisableFormatting));
            }
            else
            {
                if (User.Player.BanType == BanType.CHAT)
                {
                    TimeSpan timeSpan = DateTimeOffset.FromUnixTimeSeconds(User.Player.UnbanTime).Subtract(DateTimeOffset.UtcNow);
                    if (timeSpan.TotalSeconds > 0.0)
                    {
                        this.MessageElement = new XElement((XName)"message", new object[4]
                        {
              (object) new XAttribute((XName) "from", (object) (this.To + "/CoCBuilder")),
              (object) new XAttribute((XName) "to", (object) User.JID),
              (object) new XAttribute((XName) "type", (object) "groupchat"),
              (object) new XElement((XName) "body", (object) string.Format(" You are muted from chat! You can send messages again through: {0} hours {1} minutes {2} seconds", (object) timeSpan.Hours, (object) timeSpan.Minutes, (object) timeSpan.Seconds))
                        });
                        User.Send(this.MessageElement.ToString(SaveOptions.DisableFormatting));
                        return;
                    }
                }
                string str1 = this.Packet["message"] != null ? this.Packet["message"].InnerText : "";
                if (User.Player.Privilegie > PrivilegieId.PLAYER && str1.StartsWith("/"))
                {
                    string[] strArray = str1.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    string str2 = "";
                    string str3 = strArray[0];
                    string Nickname;
                    if (str3 != null)
                    {
                        if (str3 != null)
                        {
                            switch (str3)
                            {
                                case "/ban":
                                    Nickname = (string)null;
                                    long totalSeconds1;
                                    try
                                    {
                                        Nickname = strArray[1];
                                        totalSeconds1 = Tools.GetTotalSeconds(strArray[2]);
                                    }
                                    catch
                                    {
                                        str2 = "Argument exception!<br>Example: /ban Nickname 1d<br> This command is execute ban 'Nickname' user on 1 day<br>Available time intervals:<br><br>h - Hour<br>m - Minute<br>d - Day<br>s - Second";
                                        break;
                                    }
                                    if (Nickname == this.User.Player.Nickname)
                                    {
                                        str2 = "Do you want to block yourself. What?";
                                        break;
                                    }
                                    if (totalSeconds1 > 0L)
                                    {
                                        Client client = ArrayList.OnlineUsers.Find((Predicate<Client>)(Attribute => Attribute.Player.Nickname == Nickname));
                                        if (client == null)
                                        {
                                            if (!new Player()
                                            {
                                                Nickname = Nickname
                                            }.Load(true))
                                            {
                                                str2 = "User by name: " + Nickname + " not found at server!";
                                                break;
                                            }
                                        }
                                        client.Player.BanType = BanType.ALL;
                                        client.Player.UnbanTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + totalSeconds1;
                                        client.Player.Save();
                                        if (client.JID != null)
                                            client.Dispose();
                                        str2 = "User by name: " + Nickname + " successfily banned at server!";
                                        break;
                                    }
                                    str2 = "You are not set bantime!";
                                    break;
                                case "/broadcast":
                                    str2 += "You are limit is reached";
                                    break;
                                case "/bsay":
                                    string Text = "";
                                    foreach (string str4 in strArray)
                                    {
                                        if (!(str4 == "/bsay"))
                                            Text = Text + str4 + " ";
                                    }
                                    using (List<Client>.Enumerator enumerator = ArrayList.OnlineUsers.GetEnumerator())
                                    {
                                        while (enumerator.MoveNext())
                                            enumerator.Current.ShowMessage(Text, true);
                                        break;
                                    }
                                case "/gc":
                                    PerformanceCounter performanceCounter = new PerformanceCounter("Memory", "Available MBytes");
                                    str2 = string.Format("Онлайн: {0}<br>Выделенных серверов: {1}<br>Активных боев: {2}<br><br>Доступно оперативной памяти: {3} МБ<br>ОС: {4}<br>База данных: {5}", (object)ArrayList.OnlineUsers.Count<Client>((Func<Client, bool>)(Attribute => !Attribute.Dedicated)), (object)ArrayList.OnlineUsers.Count<Client>((Func<Client, bool>)(Attribute => Attribute.Dedicated)), (object)ArrayList.OnlineUsers.Count<Client>((Func<Client, bool>)(Attribute =>
                                    {
                                        if (Attribute.Dedicated)
                                            return Attribute.Player.RoomPlayer.Room != null;
                                        return false;
                                    })), (object)performanceCounter.NextValue().ToString(), (object)Environment.OSVersion.ToString(), (object)new MySqlCommand("select @@VERSION;", SQL.Handler).ExecuteScalar().ToString());
                                    break;
                                case "/item":
                                    if (User.Player.Privilegie != PrivilegieId.ADMINISTRATOR)
                                    {
                                        str2 += "Данная команда Вам не доступна";
                                        break;
                                    }
                                    int num1 = 0;
                                    long num2 = 0;
                                    string str5;
                                    Messages.ItemType itemType;
                                    try
                                    {
                                        Nickname = strArray[1];
                                        str5 = strArray[3];
                                        itemType = (Messages.ItemType)byte.Parse(strArray[2]);
                                        if (itemType == Messages.ItemType.Expiration)
                                            num2 = Tools.GetTotalSeconds(strArray[4]);
                                        if (itemType == Messages.ItemType.Consumable)
                                            num2 = (long)ushort.Parse(strArray[4]);
                                        if (!Enum.IsDefined(typeof(Messages.ItemType), (object)num1))
                                        {
                                            str2 = string.Format("Этот тип элемента ({0}) не найден!<br> Available currencies :<br><br>0 - Expiration<br>1 - Consumable<br>2 - Permanent", (object)itemType);
                                            break;
                                        }
                                    }
                                    catch
                                    {
                                        str2 = "Ошибка!<br>пример : /item Nickname 2 ar12_shop 1h <br> Эта команда будет отправлять товар на игрока (Nickname, 1 hour, M16A3). Доступные валюты :<br><br>0 - CROWN<br>1 - Варбаксы[GameMoney]<br>2 - Кредиты<br><br>В интервале:<br>h - Час<br>d - День<br><br>0 - Сроком действия<br>1 - Расходный материал<br>2 - Постоянный";
                                        break;
                                    }
                                    Client User1 = ArrayList.OnlineUsers.Find((Predicate<Client>)(Attribute => Attribute.Player.Nickname == Nickname));
                                    if (User1 == null)
                                    {
                                        Player player = new Player()
                                        {
                                            Nickname = Nickname
                                        };
                                        if (!player.Load(true))
                                        {
                                            str2 = "Пользователь с именем: " + Nickname + " не найден на сервере!";
                                            break;
                                        }
                                        User1 = new Client() { Player = player };
                                    }
                                    int num3;
                                    switch (itemType)
                                    {
                                        case Messages.ItemType.Expiration:
                                            num3 = 5;
                                            break;
                                        case Messages.ItemType.Consumable:
                                            num3 = 1;
                                            break;
                                        default:
                                            num3 = 2;
                                            break;
                                    }
                                    int num4 = num3;
                                    long itemSeed = User1.Player.ItemSeed;
                                    string Name1 = str5;
                                    TimeSpan timeSpan = TimeSpan.FromSeconds((double)num2);
                                    int totalHours = (int)timeSpan.TotalHours;
                                    int Quantity = (int)num2;
                                    long DurabilityPoints = itemType == Messages.ItemType.Permanent ? 36000L : 0L;
                                    AuroraServer.CLASSES.Item obj = new AuroraServer.CLASSES.Item((AuroraServer.CLASSES.ItemType)num4, itemSeed, Name1, totalHours, Quantity, DurabilityPoints);
                                    User1.Player.Items.Add(obj);
                                    Player player1 = User1.Player;
                                    string OfferType = itemType.ToString();
                                    string Name2 = str5;
                                    int Amount1;
                                    switch (itemType)
                                    {
                                        case Messages.ItemType.Expiration:
                                            timeSpan = TimeSpan.FromSeconds((double)num2);
                                            Amount1 = (int)timeSpan.TotalHours;
                                            break;
                                        case Messages.ItemType.Consumable:
                                            Amount1 = (int)num2;
                                            break;
                                        default:
                                            Amount1 = 0;
                                            break;
                                    }
                                    player1.AddItemNotification(OfferType, Name2, Amount1, "Зачислил: " + User.Player.Nickname);
                                    User1.Player.Save();
                                    if (User1.Socket != null)
                                    {
                                        new SyncNotification(User1, (XmlDocument)null).Process();
                                        ResyncProfile resyncProfile = new ResyncProfile(User1);
                                    }
                                    str2 = "Имя пользователя: " + Nickname + " успешно отправлено " + str5 + " " + itemType.ToString() + "!";
                                    break;
                                case "/money":
                                    if (User.Player.Privilegie != PrivilegieId.ADMINISTRATOR)
                                    {
                                        str2 += "Данная команда Вам не доступна";
                                        break;
                                    }
                                    Messages.Currency currency;
                                    int Amount2;
                                    try
                                    {
                                        Nickname = strArray[1];
                                        currency = (Messages.Currency)byte.Parse(strArray[2]);
                                        Amount2 = int.Parse(strArray[3]);
                                        if (!Enum.IsDefined(typeof(Messages.Currency), (object)currency))
                                        {
                                            str2 = string.Format("Эта валюта ({0}) не найдена!<br> Available currencies :<br><br>0 - CROWN<br>1 - WARBAX (GameMoney)<br>2 - CREDIT", (object)currency);
                                            break;
                                        }
                                    }
                                    catch
                                    {
                                        str2 = "Исключение аргументов!<br>пример: /money Nickname 2 5000<br> Эта команда отправит игроку игровые ресурсы (Nickname, 5000 CREDITS). Доступные валюты :<br><br>0 - Короны<br>1 - Варбаксы (GameMoney)<br>2 - Кредиты";
                                        break;
                                    }
                                    Client User2 = ArrayList.OnlineUsers.Find((Predicate<Client>)(Attribute => Attribute.Player.Nickname == Nickname));
                                    Player player2 = (Player)null;
                                    if (User2 != null)
                                        player2 = User2.Player;
                                    if ((object)player2 == null)
                                    {
                                        Player player3 = new Player()
                                        {
                                            Nickname = Nickname
                                        };
                                        if (!player3.Load(true))
                                        {
                                            str2 = "Пользователь: " + Nickname + " не найден на сервере!";
                                            break;
                                        }
                                        player2 = player3;
                                    }
                                    switch (currency)
                                    {
                                        case Messages.Currency.crown_money:
                                            player2.CrownMoney += Amount2;
                                            break;
                                        case Messages.Currency.game_money:
                                            player2.GameMoney += Amount2;
                                            break;
                                        case Messages.Currency.cry_money:
                                            player2.CryMoney += Amount2;
                                            break;
                                    }
                                    player2.AddMoneyNotification(currency.ToString(), Amount2, User.Player.Nickname + " начислил Вам:");
                                    player2.Save();
                                    if (User2 != null)
                                    {
                                        new SyncNotification(User2, (XmlDocument)null).Process();
                                        ResyncProfile resyncProfile = new ResyncProfile(User2);
                                    }
                                    str2 = string.Format("Пользователю с именем: {0} успешно отправлено {1} {2}!", (object)Nickname, (object)Amount2, (object)currency.ToString());
                                    break;
                                case "/mute":
                                    Nickname = (string)null;
                                    long totalSeconds2;
                                    try
                                    {
                                        Nickname = strArray[1];
                                        totalSeconds2 = Tools.GetTotalSeconds(strArray[2]);
                                    }
                                    catch
                                    {
                                        str2 = "Argument exception!<br>Example: /mute Nickname 1h <br> This command will block the chat player (Nickname, 1 hour) for all channels. Include private messages<br>Available time intervals:<br><br>h - Hour<br>m - Minute<br>d - Day<br>s - Second";
                                        break;
                                    }
                                    Client client1 = ArrayList.OnlineUsers.Find((Predicate<Client>)(Attribute => Attribute.Player.Nickname == Nickname));
                                    if (client1 == null)
                                    {
                                        if (!new Player()
                                        {
                                            Nickname = Nickname
                                        }.Load(true))
                                        {
                                            str2 = "User by name: " + Nickname + " not found at server!";
                                            break;
                                        }
                                    }
                                    client1.Player.BanType = BanType.CHAT;
                                    client1.Player.UnbanTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + totalSeconds2;
                                    client1.Player.Save();
                                    str2 = "User by name: " + Nickname + " successfily muted at server!";
                                    break;
                                case "/permban":
                                    Nickname = (string)null;
                                    try
                                    {
                                        Nickname = strArray[1];
                                    }
                                    catch
                                    {
                                        str2 = "Argument exception!<br>Example: /permban Nickname <br> This command is execute permanent ban 'Nickname'";
                                        break;
                                    }
                                    Client client2 = ArrayList.OnlineUsers.Find((Predicate<Client>)(Attribute => Attribute.Player.Nickname == Nickname));
                                    if (client2 == null)
                                    {
                                        Player player3 = new Player()
                                        {
                                            Nickname = Nickname
                                        };
                                        if (!player3.Load(true))
                                        {
                                            str2 = "User by name: " + Nickname + " not found at server!";
                                            break;
                                        }
                                        client2 = new Client()
                                        {
                                            Player = player3
                                        };
                                    }
                                    client2.Player.BanType = BanType.ALL_PERMANENT;
                                    client2.Player.UnbanTime = 0L;
                                    client2.Player.Save();
                                    if (client2.JID != null)
                                        client2.Dispose();
                                    str2 = "User by name: " + Nickname + " successfily banned at server!";
                                    break;
                                case "/psave":
                                    User.Player.ClanPlayer.Clan.Save();
                                    break;
                            }
                        }
                    }
                    User.ShowMessage("MOW Games 1.0<br><br>" + str2, false);
                }
                else
                {
                    if (this.To == User.Channel.JID + "@conference.warface")
                    {
                        foreach (Client client in User.Channel.Users.ToArray())
                        {
                            this.MessageElement = new XElement((XName)"message", new object[4]
                            {
                (object) new XAttribute((XName) "from", (object) ("global." + User.Channel.Resource + "@conference.warface/" + User.Player.Nickname)),
                (object) new XAttribute((XName) "to", (object) client.JID),
                (object) new XAttribute((XName) "type", (object) "groupchat"),
                (object) new XElement((XName) "body", (object) this.Packet["message"].InnerText)
                            });
                            client.Send(this.MessageElement.ToString(SaveOptions.DisableFormatting));
                        }
                    }
                    else if (User.Player.RoomPlayer.Room != null && this.To == string.Format("room.{0}@conference.warface", (object)User.Player.RoomPlayer.Room.Core.RoomId))
                    {
                        foreach (Client client in User.Player.RoomPlayer.Room.Players.Users.ToArray())
                        {
                            this.MessageElement = new XElement((XName)"message", new object[4]
                            {
                (object) new XAttribute((XName) "from", (object) string.Format("room.{0}@conference.warface/{1}", (object) User.Player.RoomPlayer.Room.Core.RoomId, (object) User.Player.Nickname)),
                (object) new XAttribute((XName) "to", (object) client.JID),
                (object) new XAttribute((XName) "type", (object) "groupchat"),
                (object) new XElement((XName) "body", (object) this.Packet["message"].InnerText)
                            });
                            client.Send(this.MessageElement.ToString(SaveOptions.DisableFormatting));
                        }
                    }
                    else if (User.Player.RoomPlayer.Room != null && this.To == string.Format("team.room.{0}@conference.warface", (object)User.Player.RoomPlayer.Room.Core.RoomId))
                    {
                        foreach (Client client in User.Player.RoomPlayer.Room.Players.Users.ToList<Client>().FindAll((Predicate<Client>)(Attribute => Attribute.Player.RoomPlayer.TeamId == User.Player.RoomPlayer.TeamId)).ToArray())
                        {
                            this.MessageElement = new XElement((XName)"message", new object[4]
                            {
                (object) new XAttribute((XName) "from", (object) string.Format("team.room.{0}@conference.warface/{1}", (object) User.Player.RoomPlayer.Room.Core.RoomId, (object) User.Player.Nickname)),
                (object) new XAttribute((XName) "to", (object) client.JID),
                (object) new XAttribute((XName) "type", (object) "groupchat"),
                (object) new XElement((XName) "body", (object) this.Packet["message"].InnerText)
                            });
                            client.Send(this.MessageElement.ToString(SaveOptions.DisableFormatting));
                        }
                    }
                    else if (User.Player.ClanPlayer.Clan != null && this.To == string.Format("clan.{0}@conference.warface", (object)User.Player.ClanPlayer.Clan.ID))
                    {
                        foreach (Client client in ArrayList.OnlineUsers.FindAll((Predicate<Client>)(Attribute =>
                        {
                            if (Attribute.Player.ClanPlayer.Clan != null)
                                return User.Player.ClanPlayer.Clan.ID == Attribute.Player.ClanPlayer.Clan.ID;
                            return false;
                        })))
                        {
                            this.MessageElement = new XElement((XName)"message", new object[4]
                            {
                (object) new XAttribute((XName) "from", (object) string.Format("clan.{0}@conference.warface/{1}", (object) User.Player.ClanPlayer.Clan.ID, (object) User.Player.Nickname)),
                (object) new XAttribute((XName) "to", (object) client.JID),
                (object) new XAttribute((XName) "type", (object) "groupchat"),
                (object) new XElement((XName) "body", (object) this.Packet["message"].InnerText)
                            });
                            client.Send(this.MessageElement.ToString(SaveOptions.DisableFormatting));
                        }
                    }
                    else if (this.To == "wfc.warface" || this.To == "wfc.warface" || this.To == "wfc.row_emul.warface")
                    {
                        Client client = ArrayList.OnlineUsers.Find((Predicate<Client>)(Attribute => Attribute.Player.Nickname == messages.Query.Attributes["nick"].InnerText));
                        if (client == null)
                        {
                            ToOnlinePlayers toOnlinePlayers = new ToOnlinePlayers(User, Packet);
                            return;
                        }
                        XElement xelement1 = new XElement(Gateway.JabberNS + "iq");
                        xelement1.Add((object)new XAttribute((XName)"type", (object)"get"));
                        xelement1.Add((object)new XAttribute((XName)"from", (object)User.JID));
                        xelement1.Add((object)new XAttribute((XName)"to", (object)client.JID));
                        xelement1.Add((object)new XAttribute((XName)"id", (object)this.Id));
                        XElement xelement2 = new XElement(Stanza.NameSpace + "query");
                        xelement2.Add((object)new XElement((XName)"message", new object[3]
                        {
              (object) new XAttribute((XName) "from", (object) User.Player.Nickname),
              (object) new XAttribute((XName) "nick", (object) client.Player.Nickname),
              (object) new XAttribute((XName) "message", (object) this.Query.Attributes["message"].InnerText)
                        }));
                        xelement1.Add((object)xelement2);
                        client.Send(xelement1.ToString(SaveOptions.None));
                    }
                    else if (this.Type == "result")
                    {
                        ToOnlinePlayers toOnlinePlayers1 = new ToOnlinePlayers(User, Packet);
                    }
                    else
                    {
                        try
                        {
                            XElement xelement = new XElement((XName)"message", new object[5]
                            {
                (object) new XAttribute((XName) "from", (object) this.To),
                (object) new XAttribute((XName) "to", (object) User.JID),
                (object) new XAttribute((XName) "type", (object) "error"),
                (object) new XElement((XName) "body", (object) this.Packet["message"].InnerText),
                (object) new XElement((XName) "error", new object[4]
                {
                  (object) new XAttribute((XName) "code", (object) "406"),
                  (object) new XAttribute((XName) "type", (object) "modify"),
                  (object) new XElement((XNamespace) "urn:ietf:params:xml:ns:xmpp-stanzas" + "not-acceptable"),
                  (object) new XElement((XNamespace) "urn:ietf:params:xml:ns:xmpp-stanzas" + "text", (object) "Only occupants are allowed to send messages to the conference")
                })
                            });
                            User.Send(xelement.ToString(SaveOptions.DisableFormatting));
                        }
                        catch
                        {
                        }
                    }
                    stopwatch.Reset();
                    stopwatch.Start();
                }
            }
        }

        private enum Currency
        {
            crown_money,
            game_money,
            cry_money,
        }

        private enum ItemType
        {
            Expiration,
            Consumable,
            Permanent,
        }
    }
}
