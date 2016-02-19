// -----------------------------------------------------------
// This program is private software, based on C# source code.
// To sell or change credits of this software is forbidden,
// except if someone approves it from the LeafyCoding INC. team.
// -----------------------------------------------------------
// Copyrights (c) 2016 Leafy-IRCBot.Quotes INC. All rights reserved.
// -----------------------------------------------------------

namespace IRCBot.Quotes.Classes
{
    internal class Config
    {
        //private static IniConfigSource ConfigFile;

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

        public static bool Init() => true; // TODO
    }
}