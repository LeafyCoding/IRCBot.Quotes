// -----------------------------------------------------------
// This program is private software, based on C# source code.
// To sell or change credits of this software is forbidden,
// except if someone approves it from the LeafyCoding INC. team.
// -----------------------------------------------------------
// Copyrights (c) 2016 Leafy-IRCBot.Quotes INC. All rights reserved.
// -----------------------------------------------------------

#region

using System;
using System.Linq;
using IRCBot.Quotes.Classes;
using MySql.Data.MySqlClient;

#endregion

// ReSharper disable InvertIf

namespace IRCBot.Quotes.Handlers
{
    internal static class MySQLHandler
    {
        private static string DB_ConnectionString = string.Empty;
        public static MySqlConnection DB_Connection;

        private static void InitMySQL()
        {
            DB_ConnectionString =
                $"host={Config.DB_Address};user={Config.DB_User};password={Config.DB_Password};database={Config.DB_Database};port={Config.DB_Port};";
            DB_Connection = new MySqlConnection(DB_ConnectionString);
        }

        public static bool TestMySQL()
        {
            if (DB_Connection == null)
            {
                InitMySQL();
            }
            if (OpenConnection())
            {
                try
                {
                    var MySQL_Command = new MySqlCommand("SELECT * FROM `users` WHERE `id` = 1", DB_Connection);
                    var dataReader = MySQL_Command.ExecuteReader();
                    dataReader.Read();
                    dataReader.Close();
                    CloseConnection();
                    return true;
                }
                catch (Exception ex)
                {
                    Tools.ColoredWrite(ConsoleColor.DarkRed, $"{ex.GetType().Name}: {ex.Message}");
                    Tools.ColoredWrite(ConsoleColor.Red, "An error was encountered while testing MySQL connection.");
                    return false;
                }
            }
            return false;
        }

        public static bool OpenConnection()
        {
            try
            {
                DB_Connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (ex.Number)
                {
                    case 0:
                        Tools.ColoredWrite(ConsoleColor.Red,
                            "[MySQL:Err] Cannot connect to server. Contact administrator");
                        break;

                    case 1045:
                        Tools.ColoredWrite(ConsoleColor.Red, "[MySQL:Err] Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }

        public static bool CloseConnection()
        {
            try
            {
                DB_Connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                Tools.ColoredWrite(ConsoleColor.DarkRed, $"{ex.GetType().Name}: {ex.Message}");
                return false;
            }
        }

        public static void InitUserList()
        {
            InitAdmins();
            InitUsers();
        }

        private static void InitAdmins()
        {
            if (OpenConnection())
            {
                try
                {
                    var MySQL_CommandUsers = new MySqlCommand("SELECT `name` FROM `admins`", DB_Connection);
                    var dataReaderUsers = MySQL_CommandUsers.ExecuteReader();
                    while (dataReaderUsers.Read())
                    {
                        UserHandler.Admins.Add(dataReaderUsers["name"].ToString().ToLower());
                    }
                    dataReaderUsers.Close();

                    var _s = UserHandler.Admins.Count > 1 ? "s" : string.Empty;
                    Tools.SemiColoredWrite(ConsoleColor.Cyan, "[MySQL:InitAdmins] ",
                        $"Loaded {UserHandler.Admins.Count} admin{_s}.");

                    var altCount = 0;
                    var MySQL_CommandAlts = new MySqlCommand("SELECT `alts` FROM `admins` WHERE `alts` != ''",
                        DB_Connection);
                    var dataReaderAlts = MySQL_CommandAlts.ExecuteReader();
                    while (dataReaderAlts.Read())
                    {
                        if (dataReaderAlts["alts"].ToString().Contains(','))
                        {
                            var alt = dataReaderAlts["alts"].ToString();
                            var alts = alt.Split(',');

                            foreach (var nick in alts)
                            {
                                UserHandler.Admins.Add(nick.ToLower());
                                altCount++;
                            }
                        }
                        else
                        {
                            UserHandler.Admins.Add(dataReaderUsers["alts"].ToString().ToLower());
                            altCount++;
                        }
                    }
                    dataReaderAlts.Close();

                    var _alt_s = altCount > 1 ? "s" : string.Empty;
                    Tools.SemiColoredWrite(ConsoleColor.Cyan, "[MySQL:InitAdmins] ",
                        $"Loaded {altCount} admin alt{_alt_s}.");
                    CloseConnection();
                }
                catch (Exception ex)
                {
                    Tools.ColoredWrite(ConsoleColor.DarkRed, $"{ex.GetType().Name}: {ex.Message}");
                    Tools.ColoredWrite(ConsoleColor.Red, "An error was encountered while initializing admins.");
                }
            }
        }

        private static void InitUsers()
        {
            if (OpenConnection())
            {
                try
                {
                    var MySQL_Command = new MySqlCommand("SELECT `name` FROM `users`", DB_Connection);
                    var dataReader = MySQL_Command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        UserHandler.Users.Add(dataReader["name"].ToString().ToLower());
                    }
                    dataReader.Close();

                    var _s = UserHandler.Users.Count > 1 ? "s" : string.Empty;
                    Tools.SemiColoredWrite(ConsoleColor.Cyan, "[MySQL:InitUsers] ",
                        $"Loaded {UserHandler.Users.Count} user{_s}.");

                    var altCount = 0;
                    MySQL_Command = new MySqlCommand("SELECT `alts` FROM `users` WHERE `alts` != ''", DB_Connection);
                    dataReader = MySQL_Command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        if (dataReader["alts"].ToString().Contains(','))
                        {
                            var alt = dataReader["alts"].ToString();
                            var alts = alt.Split(',');

                            foreach (var nick in alts)
                            {
                                UserHandler.Users.Add(nick.ToLower());
                                altCount++;
                            }
                        }
                        else
                        {
                            UserHandler.Users.Add(dataReader["alts"].ToString().ToLower());
                            altCount++;
                        }
                    }
                    dataReader.Close();

                    var _alt_s = altCount > 1 ? "s" : string.Empty;
                    Tools.SemiColoredWrite(ConsoleColor.Cyan, "[MySQL:InitUsers] ",
                        $"Loaded {altCount} user alt{_alt_s}.");
                    CloseConnection();
                }
                catch (Exception ex)
                {
                    Tools.ColoredWrite(ConsoleColor.DarkRed, $"{ex.GetType().Name}: {ex.Message}");
                    Tools.ColoredWrite(ConsoleColor.Red, "An error was encountered while initializing users.");
                }
            }
        }
    }
}