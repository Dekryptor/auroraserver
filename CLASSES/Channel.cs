using System.Collections.Generic;
using System.Xml.Linq;
using AuroraServer.CLASSES.GAMEROOM;

namespace AuroraServer.CLASSES
{
	internal class Channel
	{
		internal string Resource;
		internal ushort ServerId;
		internal string ChannelType;
		internal string RankGroup = "all";
		internal byte MinRank = 1;
		internal byte MaxRank = 90;
		internal string Bootstrap = "";
		internal List<GameRoom> GameRoomList = new List<GameRoom>();
		internal List<Client> Users = new List<Client>();
		internal double Load => Users.Count / 400;
		internal int Online => Users.Count;
		internal string JID => "global." + Resource;
		internal Channel(string Resource, ushort ServerId, string ChannelType, byte MinRank, byte MaxRank)
		{
			this.Resource = Resource;
			this.ServerId = ServerId;
			this.ChannelType = ChannelType;
			this.MinRank = MinRank;
			this.MaxRank = MaxRank;
		}

		internal XElement Serialize()
		{
			XElement xElement = new XElement("server");
			xElement.Add(new XAttribute("resource", Resource));
			xElement.Add(new XAttribute("server_id", ServerId));
			xElement.Add(new XAttribute("channel", ChannelType));
			xElement.Add(new XAttribute("rank_group", RankGroup));
			xElement.Add(new XAttribute("load", Load));
			xElement.Add(new XAttribute("online", Online));
			xElement.Add(new XAttribute("min_rank", MinRank));
			xElement.Add(new XAttribute("max_rank", MaxRank));
			xElement.Add(new XAttribute("bootstrap", Bootstrap));
			XElement xElement2 = new XElement("load_stats");
			XElement xElement3 = new XElement("load_stat");
			xElement3.Add(new XAttribute("type", "quick_play"));
			xElement3.Add(new XAttribute("value", "255"));
			XElement xElement4 = new XElement("load_stat");
			xElement4.Add(new XAttribute("type", "survival"));
			xElement4.Add(new XAttribute("value", "255"));
			XElement xElement5 = new XElement("load_stat");
			xElement5.Add(new XAttribute("type", "pve"));
			xElement5.Add(new XAttribute("value", "255"));
			xElement2.Add(xElement3);
			xElement2.Add(xElement4);
			xElement2.Add(xElement5);
			xElement.Add(xElement2);
			return xElement;
		}
	}
}
