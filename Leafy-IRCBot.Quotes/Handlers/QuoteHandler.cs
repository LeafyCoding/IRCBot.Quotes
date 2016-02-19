// ----------------------------------------------------
// This program is a private software, based on c# source code.
// To sell or change credits of this software is forbidden,
// except if someone approves it from the IRCBot.Quotes team.
// ----------------------------------------------------
// Copyright (c) 2015 IRCBot.Quotes team. All rights reserved.
// ----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using IRCBot.Quotes.Classes;
using MySql.Data.MySqlClient;

// ReSharper disable InvertIf

namespace IRCBot.Quotes.Handlers
{
    internal static class QuoteHandler
    {
        public static void Read(string sendermsg, string channel)
        {
            if (MySQLHandler.OpenConnection())
            {
                try
                {
                    var number = Convert.ToInt32(sendermsg.Split(new[] { "!quote read " }, StringSplitOptions.None).Last());

                    var id = string.Empty;
                    var quote = string.Empty;
                    var name = string.Empty;
                    var time = string.Empty;

                    var MySQL_Command = new MySqlCommand($"SELECT * FROM `quotes` WHERE `id` = '{number}'", MySQLHandler.DB_Connection);
                    var dataReader = MySQL_Command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        id = dataReader["id"].ToString();
                        name = dataReader["name"].ToString();
                        quote = dataReader["quote"].ToString().Split(new[] { ") " }, StringSplitOptions.None)[1];
                        time = dataReader["time"].ToString();
                    }
                    dataReader.Close();
                    MySQLHandler.CloseConnection();

                    if (!string.IsNullOrEmpty(id))
                    {
                        var msg = $"{IRC.BOLD}Quote #{id} added by {name} at {time}:";
                        Program.client.SendMessage($"{IRC.NOCOLOR}{msg}", channel);
                        Program.client.SendMessage($"{IRC.NOCOLOR}{quote}", channel);
                        Tools.SemiColoredWrite(ConsoleColor.Green, "[QuoteRead:Success] ", number.ToString());
                    }
                    else
                    {
                        var err = $"No such quote #{number}.";
                        Program.client.SendMessage(err, channel);
                        Tools.SemiColoredWrite(ConsoleColor.Red, $"[QuoteRead:{err}] ", number.ToString());
                    }
                }
                catch (Exception ex) // Probably not a good idea to run code in an Exception catch.
                {
                    if (ex.Message == "Input string was not in a correct format.")
                    {
                        try
                        {
                            var nick = sendermsg.Split(new[] { "!quote read " }, StringSplitOptions.None).Last();

                            var full = new List<string>();

                            var MySQL_Command = new MySqlCommand($"SELECT * FROM `quotes` WHERE `name` = '{nick}'", MySQLHandler.DB_Connection);
                            var dataReader = MySQL_Command.ExecuteReader();
                            while (dataReader.Read())
                            {
                                var id = dataReader["id"].ToString();
                                var name = dataReader["name"].ToString();
                                var quote = dataReader["quote"].ToString().Split(new[] { ") " }, StringSplitOptions.None)[1];
                                var time = dataReader["time"].ToString();
                                var fullquote = $"ID={id}`NAME={name}`QUOTE={quote}`TIME={time}`";
                                full.Add(fullquote);
                            }
                            dataReader.Close();
                            MySQLHandler.CloseConnection();

                            if (full.Count != 0)
                            {
                                var readquote = full.Last();

                                var _id = readquote.Split(new[] { "ID=" }, StringSplitOptions.None).Last().Split('`').First();
                                var _name = readquote.Split(new[] { "NAME=" }, StringSplitOptions.None).Last().Split('`').First();
                                var _quote = readquote.Split(new[] { "QUOTE=" }, StringSplitOptions.None).Last().Split('`').First();
                                var _time = readquote.Split(new[] { "TIME=" }, StringSplitOptions.None).Last().Split('`').First();

                                var msg = $"{IRC.BOLD}Quote #{_id} added by {_name} at {_time}:";
                                Program.client.SendMessage($"{IRC.NOCOLOR}{msg}", channel);
                                Program.client.SendMessage($"{IRC.NOCOLOR}{_quote}", channel);
                                Tools.SemiColoredWrite(ConsoleColor.Green, "[QuoteRead:Success] ", nick);
                            }
                            else
                            {
                                var err = $"No quotes found for {nick}.";
                                Program.client.SendMessage($"{IRC.NOCOLOR}{IRC.BOLD}{IRC.RED}{err}", channel);
                                Tools.SemiColoredWrite(ConsoleColor.Red, $"[QuoteRead:{err}] ", nick);
                            }
                        }
                        catch (Exception ex2)
                        {
                            var err = $"{ex2.GetType().Name}: {ex2.Message}";
                            Program.client.SendMessage($"{IRC.BOLD}{IRC.RED}{err}", channel);
                            Tools.ColoredWrite(ConsoleColor.DarkRed, err);
                            Tools.ColoredWrite(ConsoleColor.Red, "An error was encountered while reading quote.");
                        }
                    }
                    else
                    {
                        var err = $"{ex.GetType().Name}: {ex.Message}";
                        Program.client.SendMessage($"{IRC.BOLD}{IRC.RED}{err}", channel);
                        Tools.ColoredWrite(ConsoleColor.DarkRed, err);
                        Tools.ColoredWrite(ConsoleColor.Red, "An error was encountered while reading quote.");
                    }
                }
            }
        }

        public static void ReadRandom(string channel)
        {
            if (MySQLHandler.OpenConnection())
            {
                try
                {
                    var full = new List<string>();

                    var MySQL_Command = new MySqlCommand($"SELECT * FROM `quotes`", MySQLHandler.DB_Connection);
                    var dataReader = MySQL_Command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        var id = dataReader["id"].ToString();
                        var name = dataReader["name"].ToString();
                        var quote = dataReader["quote"].ToString().Split(new[] { ") " }, StringSplitOptions.None)[1];
                        var time = dataReader["time"].ToString();
                        var fullquote = $"ID={id}`NAME={name}`QUOTE={quote}`TIME={time}`";
                        full.Add(fullquote);
                    }
                    dataReader.Close();
                    MySQLHandler.CloseConnection();

                    var rnd = new Random().Next(full.Count);
                    var readquote = full[rnd];

                    var _id = readquote.Split(new[] { "ID=" }, StringSplitOptions.None).Last().Split('`').First();
                    var _name = readquote.Split(new[] { "NAME=" }, StringSplitOptions.None).Last().Split('`').First();
                    var _quote = $@"{readquote.Split(new[] { "QUOTE=" }, StringSplitOptions.None).Last().Split('`').First()}";
                    var _time = readquote.Split(new[] { "TIME=" }, StringSplitOptions.None).Last().Split('`').First();

                    var msg = $"{IRC.BOLD}Quote #{_id} added by {_name} at {_time}:";
                    Program.client.SendMessage($"{IRC.NOCOLOR}{msg}", channel);
                    Program.client.SendMessage($"{IRC.NOCOLOR}{_quote}", channel);
                    Tools.SemiColoredWrite(ConsoleColor.Green, "[QuoteRandom:Success] ", _id);
                }
                catch (Exception ex)
                {
                    var err = $"{ex.GetType().Name}: {ex.Message}";
                    Program.client.SendMessage($"{IRC.BOLD}{IRC.RED}{err}", channel);
                    Tools.ColoredWrite(ConsoleColor.DarkRed, err);
                    Tools.ColoredWrite(ConsoleColor.Red, "An error was encountered while reading quote.");
                }
            }
        }

        public static void Add(string sendermsg, string sendernick, string channel)
        {
            var msg = sendermsg.Split(new[] { "!quote add " }, StringSplitOptions.None).Last();
            msg = $"({sendernick}) {Regex.Replace(msg, @"[\x02\x1F\x0F\x16]|\x03(\d\d?(,\d\d?)?)?", string.Empty)}";

            if (msg.Length > 140)
            {
                var err = $"{IRC.BOLD}{IRC.RED}Quote too long.";
                Program.client.SendMessage(err, channel);
                Tools.SemiColoredWrite(ConsoleColor.Red, $"[QuoteAdd:{err}] ", sendernick);
            }
            else
            {
                TwitterHandler.SendTweet(msg);

                if (TwitterHandler.TweetSuccess)
                {
                    var count = 0;
                    var foundUser = false;
                    var id = string.Empty;

                    if (MySQLHandler.OpenConnection())
                    {
                        try
                        {
                            var MySQL_Command = new MySqlCommand($"SELECT `id`,`name`,`count` FROM `users` WHERE `name` = '{sendernick}'",
                                MySQLHandler.DB_Connection);
                            var dataReader = MySQL_Command.ExecuteReader();
                            while (dataReader.Read())
                            {
                                if (string.IsNullOrEmpty(dataReader["name"].ToString()))
                                {
                                    foundUser = false;
                                }
                                else
                                {
                                    foundUser = true;
                                    id = dataReader["name"].ToString();
                                    count = Convert.ToInt32(dataReader["count"]);
                                }
                            }
                            dataReader.Close();

                            if (!foundUser)
                            {
                                MySQL_Command = new MySqlCommand($"SELECT `name`,`count` FROM `users` WHERE `alts` LIKE '%{sendernick}%'",
                                    MySQLHandler.DB_Connection);
                                dataReader = MySQL_Command.ExecuteReader();
                                while (dataReader.Read())
                                {
                                    id = dataReader["name"].ToString();
                                    count = Convert.ToInt32(dataReader["count"]);
                                }
                                dataReader.Close();
                            }

                            count = count + 1;

                            try
                            {
                                MySQL_Command = new MySqlCommand($"UPDATE `{Config.DB_Database}`.`users` SET `count` = '{count}' WHERE `users`.`id` = '{id}'",
                                    MySQLHandler.DB_Connection);
                                dataReader = MySQL_Command.ExecuteReader();
                                dataReader.Read();
                                dataReader.Close();
                            }
                            catch (Exception ex)
                            {
                                Tools.ColoredWrite(ConsoleColor.DarkRed, $"{ex.GetType().Name}: {ex.Message}");
                                Tools.ColoredWrite(ConsoleColor.Red, "An error was encountered while updating MySQL quote count.");
                            }

                            try
                            {
                                using (var cmd = MySQLHandler.DB_Connection.CreateCommand())
                                {
                                    cmd.CommandText = $"INSERT INTO `{Config.DB_Database}`.`quotes` (`name`, `quote`, `time`) VALUES (@name,@quote,@time)";
                                    cmd.Parameters.AddWithValue("@name", sendernick);
                                    cmd.Parameters.AddWithValue("@quote", msg);
                                    cmd.Parameters.AddWithValue("@time", DateTime.Now.ToString("h:mm:ss tt dd/MM/yyyy"));
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            catch (Exception ex)
                            {
                                Tools.ColoredWrite(ConsoleColor.DarkRed, $"{ex.GetType().Name}: {ex.Message}");
                                Tools.ColoredWrite(ConsoleColor.Red, "An error was encountered while adding MySQL quote.");
                            }

                            MySQLHandler.CloseConnection();

                            var sendmsg = $"{IRC.BOLD}Quote added.";
                            Program.client.SendMessage(sendmsg, channel);
                            Tools.SemiColoredWrite(ConsoleColor.Green, "[QuoteAdd:Success] ", sendernick);
                        }
                        catch (Exception ex)
                        {
                            Tools.ColoredWrite(ConsoleColor.DarkRed, $"{ex.GetType().Name}: {ex.Message}");
                            Tools.ColoredWrite(ConsoleColor.Red, "An error was encountered while testing MySQL connection.");
                        }
                    }
                }
                else
                {
                    var err = "Error adding quote: " + TwitterHandler.TweetErr;
                    Program.client.SendMessage($"{IRC.BOLD}{IRC.RED}{err}", channel);
                    Tools.ColoredWrite(ConsoleColor.DarkRed, err);
                    Tools.ColoredWrite(ConsoleColor.Red, "An error was encountered while adding quote.");
                }
            }
        }
    }
}