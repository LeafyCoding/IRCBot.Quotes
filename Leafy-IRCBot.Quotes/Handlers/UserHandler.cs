// -----------------------------------------------------------
// This program is private software, based on C# source code.
// To sell or change credits of this software is forbidden,
// except if someone approves it from the LeafyCoding INC. team.
// -----------------------------------------------------------
// Copyrights (c) 2016 Leafy-IRCBot.Quotes INC. All rights reserved.
// -----------------------------------------------------------

#region

using System;
using System.Collections.Generic;
using IRCBot.Quotes.Classes;
using MySql.Data.MySqlClient;

#endregion

// ReSharper disable InvertIf

namespace IRCBot.Quotes.Handlers
{
    internal static class UserHandler
    {
        public static readonly List<string> Users = new List<string>();
        public static readonly List<string> Admins = new List<string>();

        public static bool isUser(string user)
        {
            if (Users.Contains(user.ToLower()))
            {
                Tools.SemiColoredWrite(ConsoleColor.Green, "[isUser:true] ", user);
                return true;
            }
            Tools.SemiColoredWrite(ConsoleColor.Red, "[isUser:false] ", user);
            return false;
        }

        public static bool isAdmin(string user)
        {
            if (Admins.Contains(user.ToLower()))
            {
                Tools.SemiColoredWrite(ConsoleColor.Green, "[isAdmin:true] ", user);
                return true;
            }
            Tools.SemiColoredWrite(ConsoleColor.Red, "[isAdmin:false] ", user);
            return false;
        }

        public static void WhoAmI(string user, string channel)
        {
            if (isUser(user))
            {
                var _id = string.Empty;
                var _name = string.Empty;
                var _alts = string.Empty;
                var _count = string.Empty;
                var _s = string.Empty;
                string _altmsg;
                var _admin = string.Empty;

                var foundUser = false;

                if (MySQLHandler.OpenConnection())
                {
                    try
                    {
                        var MySQL_Command = new MySqlCommand(
                            $"SELECT * FROM `users` WHERE `name` = '{user.ToLower()}'", MySQLHandler.DB_Connection);
                        var dataReader = MySQL_Command.ExecuteReader();
                        while (dataReader.Read())
                        {
                            _id = dataReader["id"].ToString();
                            if (string.IsNullOrEmpty(_id))
                            {
                                foundUser = false;
                            }
                            else
                            {
                                _name = dataReader["name"].ToString();
                                _count = dataReader["count"].ToString();
                                if (!string.IsNullOrEmpty(dataReader["alts"].ToString()))
                                {
                                    _alts = dataReader["alts"].ToString();
                                }
                                foundUser = true;
                            }
                        }
                        dataReader.Close();

                        if (!foundUser)
                        {
                            var MySQL_CommandAlts =
                                new MySqlCommand($"SELECT * FROM `users` WHERE `alts` LIKE '%{user.ToLower()}%'",
                                    MySQLHandler.DB_Connection);
                            var dataReaderAlts = MySQL_CommandAlts.ExecuteReader();
                            while (dataReaderAlts.Read())
                            {
                                _id = dataReaderAlts["id"].ToString();
                                if (!string.IsNullOrEmpty(_id))
                                {
                                    _name = dataReaderAlts["name"].ToString();
                                    _count = dataReaderAlts["count"].ToString();
                                    if (!string.IsNullOrEmpty(dataReaderAlts["alts"].ToString()))
                                    {
                                        _alts = dataReaderAlts["alts"].ToString();
                                    }
                                }
                            }
                            dataReaderAlts.Close();
                        }

                        MySQLHandler.CloseConnection();
                    }
                    catch (Exception ex)
                    {
                        Tools.ColoredWrite(ConsoleColor.DarkRed, $"{ex.GetType().Name}: {ex.Message}");
                        Tools.ColoredWrite(ConsoleColor.Red, "An error was encountered while initializing admins.");
                    }
                }

                if (_alts.Contains(","))
                {
                    _alts = _alts.Replace(",", ", ");
                }

                if (Convert.ToInt32(_count) != 1)
                {
                    _s = "s";
                }

                // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                // As much as I love ?: expressions, this one would be too long.
                if (!string.IsNullOrEmpty(_alts))
                {
                    _altmsg =
                        $"I know you mainly as {IRC.BOLD}{_name}{IRC.BOLD} although I also know you as {IRC.BOLD}{_alts}{IRC.BOLD}";
                }
                else
                {
                    _altmsg = $"I only know you as {IRC.BOLD}{_name}{IRC.BOLD}";
                }

                if (isAdmin(user))
                {
                    _admin = "You are an administrator of this bot. ";
                }

                var _msg = "{0}Hi {1}{0}, you are user {0}#{2}{0}. {3}. {6}You have {0}{4}{0} quote{5} in my database.";
                var msg = string.Format(IRC.NOCOLOR + _msg, IRC.BOLD, user, _id, _altmsg, _count, _s, _admin);
                Program.client.SendMessage(msg, channel);
                Tools.SemiColoredWrite(ConsoleColor.Green, "[WhoAmI:Success] ", user);
            }
            else
            {
                Tools.ColoredWrite(ConsoleColor.Red, $"[WhoAmI:Err] I do not recognise {user}");
                Program.client.SendMessage($"{IRC.BOLD}{IRC.RED}I do not recognise you, {user}.", channel);
            }
        }

        public static void DenyAccess(string sendernick, string channel)
        {
            Tools.ColoredWrite(ConsoleColor.Red, $"[Access:Err] I do not recognise {sendernick}");
            Program.client.SendMessage($"{IRC.BOLD}{IRC.RED}I do not recognise your authority, {sendernick}.", channel);
        }
    }
}