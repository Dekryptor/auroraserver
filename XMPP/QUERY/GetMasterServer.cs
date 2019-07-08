using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using AuroraServer.CLASSES;
using AuroraServer.NETWORK;

namespace AuroraServer.XMPP.QUERY
{
	internal class GetMasterServer : Stanza
	{
		private string Channel;

		private string[] UsedResources = new string[0];

		public GetMasterServer(Client User, XmlDocument Packet)
			: base(User, Packet)
		{
			Channel = Query.Attributes["channel"].InnerText;
			if (Query.Attributes["used_resources"] != null)
			{
				UsedResources = Query.Attributes["used_resources"].InnerText.Split(new char[1]
				{
					';'
				}, StringSplitOptions.RemoveEmptyEntries);
			}
			Process();
		}

		internal override void Process()
		{
			Channel channel = null;
			bool flag = false;
			foreach (Channel channel2 in ArrayList.Channels)
			{
				if (channel2.ChannelType == Channel && !UsedResources.Contains(channel2.Resource) && channel2.MinRank <= User.Player.Rank && channel2.MaxRank >= User.Player.Rank && channel2.Load < 1.0)
				{
					flag = true;
					channel = channel2;
					break;
				}
			}
			if (!flag)
			{
				foreach (Channel channel3 in ArrayList.Channels)
				{
					if (channel3.ChannelType == "pvp_pro" && !UsedResources.Contains(channel3.Resource) && channel3.MinRank <= User.Player.Rank && channel3.MaxRank >= User.Player.Rank && channel3.Load < 1.0)
					{
						flag = true;
						channel = channel3;
						break;
					}
				}
			}
			if (!flag)
			{
				foreach (Channel channel4 in ArrayList.Channels)
				{
					if (channel4.ChannelType == "pvp_skilled" && !UsedResources.Contains(channel4.Resource) && channel4.MinRank <= User.Player.Rank && channel4.MaxRank >= User.Player.Rank && channel4.Load < 1.0)
					{
						flag = true;
						channel = channel4;
						break;
					}
				}
			}
			if (!flag)
			{
				foreach (Channel channel5 in ArrayList.Channels)
				{
					if (channel5.ChannelType == "pvp_newbie" && !UsedResources.Contains(channel5.Resource) && channel5.MinRank <= User.Player.Rank && channel5.MaxRank >= User.Player.Rank && channel5.Load < 1.0)
					{
						flag = true;
						channel = channel5;
						break;
					}
				}
			}
			if (!flag)
			{
				foreach (Channel channel6 in ArrayList.Channels)
				{
					if (channel6.ChannelType == "pve" && !UsedResources.Contains(channel6.Resource) && channel6.MinRank <= User.Player.Rank && channel6.MaxRank >= User.Player.Rank && channel6.Load < 1.0)
					{
						flag = true;
						channel = channel6;
						break;
					}
				}
			}
			User.Channel = channel;
			XDocument xDocument = new XDocument();
			XElement xElement = new XElement(Gateway.JabberNS + "iq");
			xElement.Add(new XAttribute("type", "result"));
			xElement.Add(new XAttribute("from", To));
			xElement.Add(new XAttribute("to", User.JID));
			xElement.Add(new XAttribute("id", Id));
			XElement xElement2 = new XElement(Stanza.NameSpace + "query");
			XElement xElement3 = new XElement("get_master_server");
			if (channel != null)
			{
				xElement3.Add(new XAttribute("resource", channel.Resource));
			}
			xElement3.Add(new XAttribute("load_index", "255"));
			xElement2.Add(xElement3);
			xElement.Add(xElement2);
			if (channel == null)
			{
				XElement xElement4 = new XElement((XNamespace)"urn:ietf:params:xml:ns:xmpp-stanzas" + "error", "UNKNOWN ERROR");
				xElement4.Add(new XAttribute("type", "cancel"));
				xElement4.Add(new XAttribute("code", 503));
				xElement.Add(xElement4);
			}
			xDocument.Add(xElement);
			User.Send(xDocument.ToString(SaveOptions.DisableFormatting));
		}
	}
}
