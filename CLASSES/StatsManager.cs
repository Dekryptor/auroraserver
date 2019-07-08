using System.Xml;

namespace AuroraServer.CLASSES
{
	internal class StatsManager
	{
		private XmlDocument Doc;

		internal StatsManager(XmlDocument StatDoc)
		{
			Doc = StatDoc;
		}

		internal void IncrementPlayerStat(string StatName, long Value)
		{
			foreach (XmlNode childNode in Doc["stats"].ChildNodes)
			{
				if (childNode.Attributes["stat"].InnerText == StatName)
				{
					childNode.Attributes["Value"].InnerText = (long.Parse(childNode.Attributes["Value"].InnerText) + Value).ToString();
					return;
				}
			}
			XmlElement xmlElement = Doc.CreateElement("stat");
			XmlAttribute xmlAttribute = Doc.CreateAttribute("stat");
			XmlAttribute xmlAttribute2 = Doc.CreateAttribute("Value");
			xmlAttribute2.Value = Value.ToString();
			xmlAttribute.Value = StatName;
			xmlElement.Attributes.Append(xmlAttribute);
			xmlElement.Attributes.Append(xmlAttribute2);
			Doc["stats"].AppendChild(xmlElement);
		}

		internal long GetPlayerStat(string Statname)
		{
			foreach (XmlNode childNode in Doc["stats"].ChildNodes)
			{
				if (childNode.Attributes["stat"].InnerText == Statname)
				{
					return long.Parse(childNode.Attributes["Value"].InnerText);
				}
			}
			return 0L;
		}

		internal void ResetPlayerStat(string Statname, long Value)
		{
			foreach (XmlNode childNode in Doc["stats"].ChildNodes)
			{
				if (childNode.Attributes["stat"].InnerText == Statname)
				{
					childNode.Attributes["Value"].InnerText = Value.ToString();
				}
			}
		}

		internal void IncrementModePlayerStat(string Mode, string StatName, long Value)
		{
			foreach (XmlNode childNode in Doc["stats"].ChildNodes)
			{
				if (childNode.Attributes["stat"].InnerText == StatName && childNode.Attributes["mode"].InnerText == Mode)
				{
					childNode.Attributes["Value"].InnerText = (long.Parse(childNode.Attributes["Value"].InnerText) + Value).ToString();
					return;
				}
			}
			XmlElement xmlElement = Doc.CreateElement("stat");
			XmlAttribute xmlAttribute = Doc.CreateAttribute("mode");
			XmlAttribute xmlAttribute2 = Doc.CreateAttribute("stat");
			XmlAttribute xmlAttribute3 = Doc.CreateAttribute("Value");
			xmlAttribute3.Value = Value.ToString();
			xmlAttribute.Value = Mode;
			xmlAttribute2.Value = StatName;
			xmlElement.Attributes.Append(xmlAttribute);
			xmlElement.Attributes.Append(xmlAttribute2);
			xmlElement.Attributes.Append(xmlAttribute3);
			Doc["stats"].AppendChild(xmlElement);
		}

		internal void IncrementClassModePlayerStat(string Class, string Mode, string StatName, long Value)
		{
			foreach (XmlNode childNode in Doc["stats"].ChildNodes)
			{
				if (childNode.Attributes["stat"].InnerText == StatName && childNode.Attributes["class"].InnerText == Class && childNode.Attributes["mode"].InnerText == Mode)
				{
					childNode.Attributes["Value"].InnerText = (long.Parse(childNode.Attributes["Value"].InnerText) + Value).ToString();
					return;
				}
			}
			XmlElement xmlElement = Doc.CreateElement("stat");
			XmlAttribute xmlAttribute = Doc.CreateAttribute("class");
			XmlAttribute xmlAttribute2 = Doc.CreateAttribute("mode");
			XmlAttribute xmlAttribute3 = Doc.CreateAttribute("stat");
			XmlAttribute xmlAttribute4 = Doc.CreateAttribute("Value");
			xmlAttribute4.Value = Value.ToString();
			xmlAttribute.Value = Class;
			xmlAttribute2.Value = Mode;
			xmlAttribute3.Value = StatName;
			xmlElement.Attributes.Append(xmlAttribute);
			xmlElement.Attributes.Append(xmlAttribute2);
			xmlElement.Attributes.Append(xmlAttribute3);
			xmlElement.Attributes.Append(xmlAttribute4);
			Doc["stats"].AppendChild(xmlElement);
		}

		internal void IncrementDifficultyModePlayerStat(string Difficulty, string Mode, string StatName, long Value)
		{
			foreach (XmlNode childNode in Doc["stats"].ChildNodes)
			{
				if (childNode.Attributes["stat"].InnerText == StatName && childNode.Attributes["difficulty"].InnerText == Difficulty && childNode.Attributes["mode"].InnerText == Mode)
				{
					childNode.Attributes["Value"].InnerText = (long.Parse(childNode.Attributes["Value"].InnerText) + Value).ToString();
					return;
				}
			}
			XmlElement xmlElement = Doc.CreateElement("stat");
			XmlAttribute xmlAttribute = Doc.CreateAttribute("difficulty");
			XmlAttribute xmlAttribute2 = Doc.CreateAttribute("mode");
			XmlAttribute xmlAttribute3 = Doc.CreateAttribute("stat");
			XmlAttribute xmlAttribute4 = Doc.CreateAttribute("Value");
			xmlAttribute4.Value = Value.ToString();
			xmlAttribute.Value = Difficulty;
			xmlAttribute2.Value = Mode;
			xmlAttribute3.Value = StatName;
			xmlElement.Attributes.Append(xmlAttribute);
			xmlElement.Attributes.Append(xmlAttribute2);
			xmlElement.Attributes.Append(xmlAttribute3);
			xmlElement.Attributes.Append(xmlAttribute4);
			Doc["stats"].AppendChild(xmlElement);
		}

		internal void IncrementWeaponUsage(string ClassName, string Weapon, long Value)
		{
			foreach (XmlNode childNode in Doc["stats"].ChildNodes)
			{
				if (childNode.Attributes["stat"].InnerText == "player_wpn_usage" && childNode.Attributes["class"].InnerText == ClassName)
				{
					childNode.Attributes["Value"].InnerText = (long.Parse(childNode.Attributes["Value"].InnerText) + Value).ToString();
					break;
				}
			}
		}
	}
}
