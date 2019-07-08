using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace AuroraServer.CLASSES
{
	internal class Stanza
	{
		public static XNamespace NameSpace = "urn:cryonline:k01";
		protected Client User;
		protected XmlDocument Packet;
		protected XmlNode Query;
		internal string To;
		internal string Id;
		internal string Type;
		internal string Name;
		public Stanza(Client User, XmlDocument Packet)
		{
			this.User = User;
			if (Packet != null)
			{
				if (Packet["message"] == null)
				{
					Name = Packet["iq"]["query"].FirstChild.Name;
				}
				try
				{
					To = Packet[Packet.FirstChild.Name].Attributes["to"].InnerText;
				}
				catch
				{
				}
				if (Packet[Packet.FirstChild.Name].Attributes["id"] != null)
				{
					Id = Packet[Packet.FirstChild.Name].Attributes["id"].InnerText;
				}
				if (Packet[Packet.FirstChild.Name].Attributes["type"] != null)
				{
					Type = Packet[Packet.FirstChild.Name].Attributes["type"].InnerText;
				}
				if (Packet["iq"] != null && Packet["iq"]["query"].FirstChild != null)
				{
					Query = Packet["iq"]["query"].FirstChild;
				}
				this.Packet = Packet;
			}
		}

		internal virtual void Process()
		{
		}

		internal void Uncompress(ref XmlDocument Packet)
		{
			XmlDocument xmlDocument = new XmlDocument();
			string xml = Tools.DeflateTool.Decode(Packet.LastChild.LastChild.LastChild.Attributes["compressedData"].InnerText);
			Packet.FirstChild.FirstChild.RemoveAll();
			xmlDocument.LoadXml(xml);
			XmlNode xmlNode = Packet.ImportNode(xmlDocument.DocumentElement, deep: true);
			Packet.FirstChild.FirstChild.AppendChild(xmlNode);
			Packet.LastChild.LastChild.ReplaceChild(Packet.LastChild.LastChild.LastChild, xmlNode);
		}

		internal void Compress(ref XDocument xDocument)
		{
			XmlDocument xmlDocument = new XmlDocument();
			using (XmlReader reader = xDocument.CreateReader())
			{
				xmlDocument.Load(reader);
			}
			XmlDocument xmlDocument2 = new XmlDocument();
			XmlDocument xmlDocument3 = xmlDocument;
			XmlNode firstChild = xmlDocument3.FirstChild["query"].FirstChild;
			string name = firstChild.Name;
			string arg = Tools.DeflateTool.Encode(firstChild.OuterXml);
			int byteCount = Encoding.UTF8.GetByteCount(firstChild.OuterXml);
			xmlDocument2.LoadXml($"<data query_name='{name}' compressedData='{arg}' originalSize='{byteCount}'/>");
			foreach (XmlAttribute attribute in firstChild.Attributes)
			{
				XmlAttribute xmlAttribute2 = xmlDocument2.CreateAttribute(attribute.Name);
				xmlAttribute2.InnerText = attribute.InnerText;
				xmlDocument2.FirstChild.Attributes.Prepend(xmlAttribute2);
			}
			XmlNode newChild = xmlDocument3.ImportNode(xmlDocument2.DocumentElement, deep: true);
			xmlDocument3.FirstChild.FirstChild.RemoveAll();
			xmlDocument3.FirstChild.FirstChild.PrependChild(newChild);
			using (XmlNodeReader xmlNodeReader = new XmlNodeReader(xmlDocument))
			{
				xmlNodeReader.MoveToContent();
				xDocument = XDocument.Load(xmlNodeReader);
			}
		}
	}
}
