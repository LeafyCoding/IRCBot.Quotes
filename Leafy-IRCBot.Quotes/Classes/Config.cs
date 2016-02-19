// -----------------------------------------------------------
// This program is private software, based on C# source code.
// To sell or change credits of this software is forbidden,
// except if someone approves it from the LeafyCoding INC. team.
// -----------------------------------------------------------
// Copyrights (c) 2016 Leafy-IRCBot.Quotes INC. All rights reserved.
// -----------------------------------------------------------

using System;
using System.IO;
using IRCBot.Quotes.Handlers;
using Nini.Config;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable NotAccessedField.Global

namespace IRCBot.Quotes.Classes
{
    internal static class Config
    {
        private static IniConfigSource ConfigFile;

        public static string IRC_BotName = string.Empty;
        public static string IRC_ChannelName = string.Empty;
        public static string IRC_Server = string.Empty;
        public static string IRC_Password = string.Empty;
        public static string IRC_NSPassword = string.Empty;
        public static bool IRC_SSL;

        public static string DB_User = string.Empty;
        public static string DB_Address = string.Empty;
        public static string DB_Password = string.Empty;
        public static string DB_Database = string.Empty;
        public static string DB_Port = string.Empty;

        public static string Twitter_Consumer = string.Empty;
        public static string Twitter_ConsumerSecret = string.Empty;
        public static string Twitter_Token = string.Empty;
        public static string Twitter_TokenSecret = string.Empty;

        public static bool Init()
        {
            try
            {
                ConfigFile = new IniConfigSource("config.ini");
                return true;
            }
            catch (Exception ex)
            {
                Tools.ColoredWrite(ConsoleColor.Red, $"{ex.GetType().Name}: {ex.Message}");

                if (CreateConfig())
                {
                    Tools.ColoredWrite(ConsoleColor.Red,
                        "A new config file has been created, please edit it and re-run the program.");
                    Console.ReadKey();
                }
                return false;
            }
        }

        private static bool CreateConfig()
        {
            try
            {
                var NewConfig = ";config.ini" + Environment.NewLine +
                    "[DB_Options]" + Environment.NewLine +
                    "User = " + Environment.NewLine +
                    "Address = " + Environment.NewLine +
                    "Password = " + Environment.NewLine +
                    "Database = " + Environment.NewLine +
                    "Port = " + Environment.NewLine +
                    "SSL =  " + Environment.NewLine +
                    "[IRC_Options]" + Environment.NewLine +
                    "BotName = " + Environment.NewLine +
                    "ChannelName = " + Environment.NewLine +
                    "Server = " + Environment.NewLine +
                    "Password = " + Environment.NewLine +
                    "NickServPassword = " + Environment.NewLine +
                    "[Twitter_Options]" + Environment.NewLine +
                    "Consumer = " + Environment.NewLine +
                    "ConsumerSecret = " + Environment.NewLine +
                    "Token = " + Environment.NewLine +
                    "TokenSecret = ";

                File.WriteAllText(@"config.ini", NewConfig);
                return true;
            }
            catch (Exception ex)
            {
                Tools.ColoredWrite(ConsoleColor.DarkRed, $"{ex.GetType().Name}: {ex.Message}");
                Tools.ColoredWrite(ConsoleColor.Red, "A config file could not be written, check current directory permissions.");
                Console.ReadKey();
                return false;
            }
        }

        public static void Populate()
        {
            try
            {
                IRC_BotName = ConfigFile.Configs["IRC_Options"].Get("BotName");
                IRC_ChannelName = ConfigFile.Configs["IRC_Options"].Get("ChannelName");
                IRC_Server = ConfigFile.Configs["IRC_Options"].Get("Server");
                IRC_Password = ConfigFile.Configs["IRC_Options"].Get("Password");
                IRC_NSPassword = ConfigFile.Configs["IRC_Options"].Get("NSPassword");
                IRC_SSL = ConfigFile.Configs["IRC_Options"].GetBoolean("SSL");

                DB_User = ConfigFile.Configs["DB_Options"].Get("User");
                DB_Address = ConfigFile.Configs["DB_Options"].Get("Address");
                DB_Password = ConfigFile.Configs["DB_Options"].Get("Password");
                DB_Database = ConfigFile.Configs["DB_Options"].Get("Database");
                DB_Port = ConfigFile.Configs["DB_Options"].Get("Port");

                Twitter_Consumer = ConfigFile.Configs["Twitter_Options"].Get("Consumer");
                Twitter_ConsumerSecret = ConfigFile.Configs["Twitter_Options"].Get("ConsumerSecret");
                Twitter_Token = ConfigFile.Configs["Twitter_Options"].Get("Token");
                Twitter_TokenSecret = ConfigFile.Configs["Twitter_Options"].Get("TokenSecret");
            }
            catch (Exception ex)
            {
                Tools.ColoredWrite(ConsoleColor.DarkRed, $"{ex.GetType().Name}: {ex.Message}");
                Tools.ColoredWrite(ConsoleColor.Red, "An error was encountered while parsing the config file.");
                Console.ReadKey();
                Environment.Exit(1);
            }

            if (MySQLHandler.TestMySQL())
            {
                Tools.SemiColoredWrite(ConsoleColor.Cyan, "[MySQL:Test] ", "Success");
                MySQLHandler.InitUserList();
            }
        }
    }
}