// -----------------------------------------------------------
// This program is private software, based on C# source code.
// To sell or change credits of this software is forbidden,
// except if someone approves it from the LeafyCoding INC. team.
// -----------------------------------------------------------
// Copyrights (c) 2016 Leafy-IRCBot.Quotes INC. All rights reserved.
// -----------------------------------------------------------

#region

using System;
using System.Text.RegularExpressions;
using IRCBot.Quotes.Classes;
using MySql.Data.MySqlClient;

#endregion

// ReSharper disable InvertIf

namespace IRCBot.Quotes.Handlers
{
    internal static class QueryHandler
    {
        public static void HandleMSG(string message, string user)
        {
            if (message.Equals("whoami"))
            {
                UserHandler.WhoAmI(user, user);
            }
            if (message.StartsWith("adduser "))
            {
                if (UserHandler.isAdmin(user))
                {
                    AddUser(message, user);
                }
                else
                {
                    UserHandler.DenyAccess(user, user);
                }
            }
            if (message.StartsWith("deluser "))
            {
                if (UserHandler.isAdmin(user))
                {
                    DelUser(message, user);
                }
                else
                {
                    UserHandler.DenyAccess(user, user);
                }
            }
            if (message.StartsWith("addalt "))
            {
                if (UserHandler.isUser(user))
                {
                    AddAlt(message, user);
                }
                else
                {
                    UserHandler.DenyAccess(user, user);
                }
            }
            if (message.StartsWith("delalt "))
            {
                if (UserHandler.isUser(user))
                {
                    DelAlt(message, user);
                }
                else
                {
                    UserHandler.DenyAccess(user, user);
                }
            }
        }

        private static void AddUser(string message, string user)
        {
            var msg = message.Split(new[] {"adduser "}, StringSplitOptions.None);
            if (Regex.IsMatch(msg[1], @"([A-z]+|[0-9]|\-|\\_|\[|\]|\(\)|\/|\|)"))
            {
                if (MySQLHandler.OpenConnection())
                {
                    try
                    {
                        var userExists = false;

                        var MySQL_Command = new MySqlCommand($"SELECT * FROM `users` WHERE `name` = '{msg[1]}'",
                            MySQLHandler.DB_Connection);
                        var dataReader = MySQL_Command.ExecuteReader();
                        while (dataReader.Read())
                        {
                            if (dataReader["name"].ToString() == msg[1])
                            {
                                userExists = true;
                            }
                        }
                        dataReader.Close();

                        if (!userExists)
                        {
                            try
                            {
                                using (var cmd = MySQLHandler.DB_Connection.CreateCommand())
                                {
                                    cmd.CommandText =
                                        $"INSERT INTO `{Config.DB_Database}`.`users` (`name`, `count`) VALUES (@name, '0')";
                                    cmd.Parameters.AddWithValue("@name", msg[1]);
                                    cmd.ExecuteNonQuery();
                                }
                                Tools.SemiColoredWrite(ConsoleColor.Cyan, "[Admin:AddUser] ",
                                    $"{user} added user {msg[1]}");
                                Program.client.SendMessage(
                                    $"{IRC.BOLD}SUCCESS: added user `{IRC.ITALIC}{msg[1]}{IRC.ITALIC}` to database.",
                                    user);
                            }
                            catch (Exception ex)
                            {
                                Tools.ColoredWrite(ConsoleColor.DarkRed, $"{ex.GetType().Name}: {ex.Message}");
                                Tools.ColoredWrite(ConsoleColor.Red, "An error was encountered while adding user.");
                            }
                        }
                        else
                        {
                            Program.client.SendMessage(
                                $"{IRC.BOLD}{IRC.RED}ERROR: user `{IRC.ITALIC}{msg[1]}{IRC.ITALIC}` already exists in database.",
                                user);
                        }
                        MySQLHandler.CloseConnection();
                    }
                    catch (Exception ex)
                    {
                        Tools.ColoredWrite(ConsoleColor.DarkRed, $"{ex.GetType().Name}: {ex.Message}");
                        Tools.ColoredWrite(ConsoleColor.Red, "An error was encountered while adding user.");
                    }
                }
            }
            else
            {
                Program.client.SendMessage($"{IRC.BOLD}{IRC.RED}ERROR: user " + msg[1] + " does not seem to be valid.",
                    user);
            }
        }

        private static void DelUser(string message, string user)
        {
            var msg = message.Split(new[] {"deluser "}, StringSplitOptions.None);
            if (Regex.IsMatch(msg[1], @"([A-z]+|[0-9]|\-|\\_|\[|\]|\(\)|\/|\|)"))
            {
                if (MySQLHandler.OpenConnection())
                {
                    try
                    {
                        var userExists = false;

                        var MySQL_Command = new MySqlCommand($"SELECT * FROM `users` WHERE `name` = '{msg[1]}'",
                            MySQLHandler.DB_Connection);
                        var dataReader = MySQL_Command.ExecuteReader();
                        while (dataReader.Read())
                        {
                            if (dataReader["name"].ToString() == msg[1])
                            {
                                userExists = true;
                            }
                        }
                        dataReader.Close();

                        if (userExists)
                        {
                            try
                            {
                                using (var cmd = MySQLHandler.DB_Connection.CreateCommand())
                                {
                                    cmd.CommandText =
                                        $"DELETE FROM `{Config.DB_Database}`.`users` WHERE `users`.`name` = @name";
                                    cmd.Parameters.AddWithValue("@name", msg[1]);
                                    cmd.ExecuteNonQuery();
                                }
                                Tools.SemiColoredWrite(ConsoleColor.Cyan, "[Admin:DelUser] ",
                                    $"{user} deleted user {msg[1]}");
                                Program.client.SendMessage(
                                    $"{IRC.BOLD}SUCCESS: deleted user `{IRC.ITALIC}{msg[1]}{IRC.ITALIC}` from database.",
                                    user);
                            }
                            catch (Exception ex)
                            {
                                Tools.ColoredWrite(ConsoleColor.DarkRed, $"{ex.GetType().Name}: {ex.Message}");
                                Tools.ColoredWrite(ConsoleColor.Red, "An error was encountered while deleting user.");
                            }
                        }
                        else
                        {
                            Program.client.SendMessage(
                                $"{IRC.BOLD}{IRC.RED}ERROR: user `{IRC.ITALIC}{msg[1]}{IRC.ITALIC}` does not exist in database.",
                                user);
                        }
                        MySQLHandler.CloseConnection();
                    }
                    catch (Exception ex)
                    {
                        Tools.ColoredWrite(ConsoleColor.DarkRed, $"{ex.GetType().Name}: {ex.Message}");
                        Tools.ColoredWrite(ConsoleColor.Red, "An error was encountered while deleting user.");
                    }
                }
            }
            else
            {
                Program.client.SendMessage($"{IRC.BOLD}{IRC.RED}ERROR: user {msg[1]} does not seem to be valid.", user);
            }
        }

        private static void AddAlt(string message, string user)
        {
            var msg = message.Split(new[] {"addalt "}, StringSplitOptions.None);
            if (Regex.IsMatch(msg[1], @"([A-z]+|[0-9]|\-|\\_|\[|\]|\(\)|\/|\|)"))
            {
                var alt = string.Empty;
                var nicktype = string.Empty;
                var foundUser = false;

                if (MySQLHandler.OpenConnection())
                {
                    try
                    {
                        var MySQL_Command =
                            new MySqlCommand($"SELECT `name`,`alts` FROM `users` WHERE `name` = '{user}'",
                                MySQLHandler.DB_Connection);
                        var dataReader = MySQL_Command.ExecuteReader();
                        string nickname;
                        while (dataReader.Read())
                        {
                            nickname = dataReader["name"].ToString();
                            if (string.IsNullOrEmpty(nickname))
                            {
                                foundUser = false;
                            }
                            else
                            {
                                foundUser = true;
                                alt = dataReader["alts"].ToString();
                                nicktype = "name";
                            }
                        }
                        dataReader.Close();

                        if (!foundUser)
                        {
                            MySQL_Command =
                                new MySqlCommand($"SELECT `name`,`alts` FROM `users` WHERE `alts` LIKE '%{user}%'",
                                    MySQLHandler.DB_Connection);
                            dataReader = MySQL_Command.ExecuteReader();
                            while (dataReader.Read())
                            {
                                nickname = dataReader["name"].ToString();
                                if (string.IsNullOrEmpty(nickname))
                                {
                                    Program.client.SendMessage($"No alts found for user {user}");
                                    // This should never be the case, but we can't be too sure.
                                    Tools.SemiColoredWrite(ConsoleColor.Red, $"[AddAlt:{user}] ", "💩");
                                }
                                else
                                {
                                    alt = dataReader["alts"].ToString();
                                    nicktype = "alt";
                                }
                            }
                            dataReader.Close();
                        }

                        var alt_to_add = string.IsNullOrEmpty(alt)
                            ? msg[1]
                            : (alt.EndsWith(",") ? alt + msg[1] : alt + "," + msg[1]);

                        switch (nicktype)
                        {
                            case "name":
                                using (var cmd = MySQLHandler.DB_Connection.CreateCommand())
                                {
                                    cmd.CommandText =
                                        $"UPDATE `{Config.DB_Database}`.`users` SET `alts` = @alts WHERE `name` = '{user}'";
                                    cmd.Parameters.AddWithValue("@alts", alt_to_add);
                                    cmd.ExecuteNonQuery();
                                }
                                UserHandler.Users.Add(msg[1]);

                                break;
                            case "alt":
                                using (var cmd = MySQLHandler.DB_Connection.CreateCommand())
                                {
                                    cmd.CommandText =
                                        $"UPDATE `{Config.DB_Database}`.`users` SET `alts` = @alts WHERE `alts` LIKE '%{user}%'";
                                    cmd.Parameters.AddWithValue("@alts", alt_to_add);
                                    cmd.ExecuteNonQuery();
                                }
                                UserHandler.Users.Add(msg[1]);
                                break;
                        }
                        Tools.SemiColoredWrite(ConsoleColor.Cyan, "[User:AddAlt] ", $"{user} added alt {msg[1]}");
                        Program.client.SendMessage(
                            $"{IRC.BOLD}SUCCESS: added user `{IRC.ITALIC}{msg[1]}{IRC.ITALIC}` to database.", user);
                        MySQLHandler.CloseConnection();
                    }
                    catch (Exception ex)
                    {
                        Tools.ColoredWrite(ConsoleColor.DarkRed, $"{ex.GetType().Name}: {ex.Message}");
                        Tools.ColoredWrite(ConsoleColor.Red, "An error was encountered while adding altnick.");
                    }
                }
            }

            else
            {
                Program.client.SendMessage($"{IRC.BOLD}{IRC.RED}ERROR: alt " + msg[1] + " does not seem to be valid.",
                    user);
            }
        }

        private static void DelAlt(string message, string user)
        {
            var msg = message.Split(new[] {"delalt "}, StringSplitOptions.None);
            if (Regex.IsMatch(msg[1], @"([A-z]+|[0-9]|\-|\\_|\[|\]|\(\)|\/|\|)"))
            {
                if (MySQLHandler.OpenConnection())
                {
                    string nickname;
                    var alt = string.Empty;
                    var nicktype = string.Empty;
                    var foundUser = false;

                    var MySQL_Command = new MySqlCommand($"SELECT `name`,`alts` FROM `users` WHERE `name` = '{user}'",
                        MySQLHandler.DB_Connection);
                    var dataReader = MySQL_Command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        nickname = dataReader["name"].ToString();
                        if (string.IsNullOrEmpty(nickname))
                        {
                            foundUser = false;
                        }
                        else
                        {
                            foundUser = true;
                            alt = dataReader["alts"].ToString();
                            nicktype = "name";
                        }
                    }
                    dataReader.Close();

                    if (!foundUser)
                    {
                        MySQL_Command =
                            new MySqlCommand($"SELECT `name`,`alts` FROM `users` WHERE `alts` LIKE '%{user}%'",
                                MySQLHandler.DB_Connection);
                        dataReader = MySQL_Command.ExecuteReader();
                        while (dataReader.Read())
                        {
                            nickname = dataReader["name"].ToString();
                            if (string.IsNullOrEmpty(nickname))
                            {
                                Program.client.SendMessage($"No alts found for user {user}");
                                // This should never be the case, but we can't be too sure.
                                Tools.SemiColoredWrite(ConsoleColor.Red, $"[AddAlt:{user}] ", "💩");
                            }
                            else
                            {
                                alt = dataReader["alts"].ToString();
                                nicktype = "alt";
                            }
                        }
                        dataReader.Close();
                    }

                    if (alt.Contains(msg[1] + ","))
                    {
                        alt = alt.Replace(msg[1] + ",", string.Empty);
                    }
                    else
                    {
                        if (alt.Contains(msg[1]))
                        {
                            alt = alt.Replace(msg[1], string.Empty);
                        }
                    }
                    if (alt.EndsWith(","))
                    {
                        alt = alt.Remove(alt.Length - 1);
                    }

                    switch (nicktype)
                    {
                        case "name":
                            using (var cmd = MySQLHandler.DB_Connection.CreateCommand())
                            {
                                cmd.CommandText =
                                    $"UPDATE `{Config.DB_Database}`.`users` SET `alts` = @alts WHERE `name` = '{user}'";
                                cmd.Parameters.AddWithValue("@alts", alt);
                                cmd.ExecuteNonQuery();
                            }
                            UserHandler.Users.Remove(msg[1]);
                            break;
                        case "alt":
                            using (var cmd = MySQLHandler.DB_Connection.CreateCommand())
                            {
                                cmd.CommandText =
                                    $"UPDATE `{Config.DB_Database}`.`users` SET `alts` = @alts WHERE `alts` LIKE '%{user}%'";
                                cmd.Parameters.AddWithValue("@alts", alt);
                                cmd.ExecuteNonQuery();
                            }
                            UserHandler.Users.Remove(msg[1]);
                            break;
                    }

                    Tools.SemiColoredWrite(ConsoleColor.Cyan, "[User:DelAlt] ", $"{user} deleted alt {msg[1]}");
                    Program.client.SendMessage(
                        $"{IRC.BOLD}SUCCESS: deleted alt `{IRC.ITALIC}{msg[1]}{IRC.ITALIC}` from database.", user);
                    MySQLHandler.CloseConnection();
                }
            }
            else
            {
                Program.client.SendMessage($"{IRC.BOLD}{IRC.RED}ERROR: alt " + msg[1] + " does not seem to be valid.",
                    user);
            }
        }
    }
}