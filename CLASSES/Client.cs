using AuroraServer.NETWORK;
using AuroraServer.XMPP;
using AuroraServer.XMPP.QUERY;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace AuroraServer.CLASSES
{
    internal class Client : IDisposable
    {
        internal bool UnhandledAllowed = true;
        internal Dictionary<string, Stopwatch> ChatWatcher = new Dictionary<string, Stopwatch>();
        internal List<AuroraServer.CLASSES.InvitationTicket> InvitationTicket = new List<AuroraServer.CLASSES.InvitationTicket>();
        internal string Location = "";
        private ConcurrentQueue<byte[]> writePendingData = new ConcurrentQueue<byte[]>();
        internal Dictionary<string, XElement> DedicatedTelemetryes;
        internal Socket Socket;
        internal Player Player;
        internal SslStream SslStream;
        internal Channel Channel;
        internal byte[] Buffer;
        internal int Status;
        internal bool Dedicated;
        internal bool Authorized;
        internal string JID;
        internal int Received;
        internal ushort DedicatedPort;
        private bool sendingData;

        internal string IPAddress
        {
            get
            {
                if (!this.Socket.Connected)
                    return "0.0.0.0";
                return ((IPEndPoint)this.Socket.RemoteEndPoint).Address.ToString();
            }
        }

        internal static string ResolveNickname(long ID)
        {
            try
            {
                return new MySqlCommand(string.Format("SELECT nickname FROM tickets WHERE profileid='{0}';", (object)ID), SQL.Handler).ExecuteReader().ToString();
            }
            catch
            {
                return (string)null;
            }
        }

        internal XElement ToElement(bool isWarface = true)
        {
            XElement xelement = new XElement((XName)"player");
            xelement.Add((object)new XAttribute((XName)"profile_id", (object)this.Player.UserID));
            xelement.Add((object)new XAttribute((XName)"team_id", (object)(byte)this.Player.RoomPlayer.TeamId));
            xelement.Add((object)new XAttribute((XName)"status", (object)(byte)this.Player.RoomPlayer.Status));
            xelement.Add((object)new XAttribute((XName)"observer", (object)"0"));
            xelement.Add((object)new XAttribute((XName)"skill", (object)"0.000"));
            xelement.Add((object)new XAttribute((XName)"nickname", (object)this.Player.Nickname));
            xelement.Add((object)new XAttribute((XName)"clanName", this.Player.ClanPlayer.Clan != null ? (object)this.Player.ClanPlayer.Clan.Name : (object)""));
            xelement.Add((object)new XAttribute((XName)"class_id", (object)this.Player.CurrentClass));
            xelement.Add((object)new XAttribute((XName)"online_id", (object)this.JID));
            xelement.Add((object)new XAttribute((XName)"group_id", (object)this.Player.RoomPlayer.GroupId));
            xelement.Add((object)new XAttribute((XName)"presence", (object)this.Status));
            xelement.Add((object)new XAttribute((XName)"experience", (object)this.Player.Experience));
            xelement.Add((object)new XAttribute((XName)"banner_badge", (object)this.Player.BannerBadge));
            xelement.Add((object)new XAttribute((XName)"banner_mark", (object)this.Player.BannerMark));
            xelement.Add((object)new XAttribute((XName)"banner_stripe", (object)this.Player.BannerStripe));
            return xelement;
        }

        internal void CheckExperience()
        {
            int num = (int)this.Player.Rank - (int)this.Player.OldRank;
            for (byte index = 0; (int)index < num; ++index)
            {
                this.Player.AddRankNotifierNotification(this.Player.OldRank, this.Player.Rank, "");
                ++this.Player.OldRank;
                this.Player.AddRandomBoxNotification("random_box_rank_05", "");
            }
            if (num <= 0)
                return;
            new SyncNotification(this, (XmlDocument)null).Process();
        }

        internal void ShowMessage(string Text, bool Green = false)
        {
            XElement FastNotify = new XElement((XName)"notif");
            FastNotify.Add((object)new XAttribute((XName)"id", (object)301));
            FastNotify.Add((object)new XAttribute((XName)"type", (object)(Green ? 512 : 8)));
            FastNotify.Add((object)new XAttribute((XName)"confirmation", (object)0));
            FastNotify.Add((object)new XAttribute((XName)"from_jid", (object)"wartls@server"));
            FastNotify.Add((object)new XAttribute((XName)"message", (object)""));
            if (Green)
            {
                XElement xelement = new XElement((XName)"announcement", new object[8]
                {
          (object) new XAttribute((XName) "id", (object) this.Player.Random.Next(99, 9999999)),
          (object) new XAttribute((XName) "is_system", (object) 1),
          (object) new XAttribute((XName) "frequency", (object) 300),
          (object) new XAttribute((XName) "repeat_time", (object) 0),
          (object) new XAttribute((XName) "message", (object) Text),
          (object) new XAttribute((XName) "server", (object) "wartls"),
          (object) new XAttribute((XName) "channel", (object) "wartls"),
          (object) new XAttribute((XName) "place", (object) 1)
                });
                FastNotify.Add((object)xelement);
            }
            else
            {
                XElement xelement = new XElement((XName)"message", (object)new XAttribute((XName)"data", (object)Text));
                FastNotify.Add((object)xelement);
            }
            XElement xelement1 = new XElement((XName)"message", (object)new XAttribute((XName)"data", (object)Text));
            new SyncNotification(this, FastNotify).Process();
        }

        internal async void Send(string Message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(Message);
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter((Stream)memoryStream);
            binaryWriter.Write(Gateway.MagicBytes);
            binaryWriter.Write((long)bytes.Length);
            binaryWriter.Write(bytes);
            try
            {
                if (this.SslStream == null || !this.SslStream.IsAuthenticated)
                {
                    int num = await this.Socket.SendAsync(new ArraySegment<byte>(memoryStream.ToArray()), SocketFlags.None);
                }
                else
                    this.EnqueueDataForWrite(this.SslStream, memoryStream.ToArray());
            }
            catch (Exception ex)
            {
            }
        }

        private void EnqueueDataForWrite(SslStream sslStream, byte[] buffer)
        {
            if (buffer == null)
                return;
            this.writePendingData.Enqueue(buffer);
            lock (this.writePendingData)
            {
                if (this.sendingData)
                    return;
                this.sendingData = true;
            }
            this.Write(sslStream);
        }

        private void Write(SslStream sslStream)
        {
            byte[] result = (byte[])null;
            try
            {
                if (this.writePendingData.Count > 0 && this.writePendingData.TryDequeue(out result))
                {
                    sslStream.BeginWrite(result, 0, result.Length, new AsyncCallback(this.WriteCallback), (object)sslStream);
                }
                else
                {
                    lock (this.writePendingData)
                        this.sendingData = false;
                }
            }
            catch (Exception ex)
            {
                lock (this.writePendingData)
                    this.sendingData = false;
            }
        }

        private void WriteCallback(IAsyncResult ar)
        {
            SslStream asyncState = (SslStream)ar.AsyncState;
            try
            {
                asyncState.EndWrite(ar);
            }
            catch (Exception ex)
            {
                return;
            }
            this.Write(asyncState);
        }

        private void SocketWrite(IAsyncResult Result)
        {
            this.Socket.EndSend(Result);
        }

        public void Dispose()
        {
            StreamError streamError = new StreamError(this, "resource-constraint");
            if (this.Socket != null)
                this.Socket.Close();
            if (this.SslStream == null)
                return;
            this.SslStream.Close();
        }
    }
}
