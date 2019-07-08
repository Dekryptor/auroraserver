using AuroraServer.CLASSES;
using AuroraServer.CLASSES.CLAN;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace AuroraServer
{
    internal class SQL
    {
        public static MySqlConnection Handler = new MySqlConnection();

        public SQL()
        {
            try
            {
                SQL.Handler.ConnectionString = "Server=localhost;user=root;database=mow;password=;";
                SQL.Handler.Open();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[SQL] Успешно подключился к MySql Server!");
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[SQL] Не удалось подключиться к MySql Server!");
            }
        }

        private void Handler_InfoMessage(object sender, SqlInfoMessageEventArgs e)
        {
            Program.WriteLine(e.Message, ConsoleColor.Red);
        }

        internal static void RemoveItems(string[] Items)
        {
            List<Player> playerList = new List<Player>();
            using (MySqlDataReader mySqlDataReader = new MySqlCommand("SELECT * FROM players;", SQL.Handler).ExecuteReader())
            {
                while (mySqlDataReader.Read())
                {
                    Player player = new Player()
                    {
                        UserID = mySqlDataReader.GetInt64(0)
                    };
                    if (player.Load(false) && player.Items.RemoveAll((Predicate<Item>)(Attribute => ((IEnumerable<string>)Items).Contains<string>(Attribute.Name))) > 0)
                        playerList.Add(player);
                }
            }
            foreach (Player player in playerList)
                player.Save();
        }

        internal static void FixClans()
        {
            List<Player> playerList = new List<Player>();
            using (MySqlDataReader mySqlDataReader = new MySqlCommand("SELECT * FROM players;", SQL.Handler).ExecuteReader())
            {
                while (mySqlDataReader.Read())
                {
                    Clan clan = new Clan()
                    {
                        ID = mySqlDataReader.GetInt64(0)
                    };
                    clan.Load();
                    foreach (long profileId in clan.ProfileIds)
                    {
                        Player player = new Player()
                        {
                            UserID = profileId
                        };
                        player.Load(false);
                        player.ClanPlayer.Clan = clan;
                        playerList.Add(player);
                    }
                }
            }
        }
    }
}