using System.Xml;
using System.Xml.Linq;

namespace AuroraServer.CLASSES
{
	public static class DocumentExtensions
	{
		public static XmlDocument ToXmlDocument(this XDocument xDocument)
		{
			XmlDocument xmlDocument = new XmlDocument();
			using (XmlReader reader = xDocument.CreateReader())
			{
				xmlDocument.Load(reader);
				return xmlDocument;
			}
		}

		public static XDocument ToXDocument(this XmlDocument xmlDocument)
		{
			using (XmlNodeReader xmlNodeReader = new XmlNodeReader(xmlDocument))
			{
				xmlNodeReader.MoveToContent();
				return XDocument.Load(xmlNodeReader);
			}
		}
	}
}
