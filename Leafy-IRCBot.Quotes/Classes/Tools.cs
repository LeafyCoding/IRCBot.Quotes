// -----------------------------------------------------------
// This program is private software, based on C# source code.
// To sell or change credits of this software is forbidden,
// except if someone approves it from the LeafyCoding INC. team.
// -----------------------------------------------------------
// Copyrights (c) 2016 Leafy-IRCBot.Quotes INC. All rights reserved.
// -----------------------------------------------------------

#region

using System;
using System.Threading;
using IRCBot.Quotes.Handlers;

// ReSharper disable UnusedMember.Global

#endregion

// ReSharper disable SwitchStatementMissingSomeCases

namespace IRCBot.Quotes.Classes
{
    internal static class Tools
    {
        public static void ColoredWrite(ConsoleColor color, string text)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(text + Environment.NewLine);
            Console.ForegroundColor = originalColor;
        }

        public static void SemiColoredWrite(ConsoleColor color, string coloredText, string noColorText)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(coloredText);
            Console.ForegroundColor = originalColor;
            Console.Write(noColorText + Environment.NewLine);
        }

        private static string BuildCTCPTime()
        {
            var time = DateTime.Now;
            var message = $"{IRC.NOCOLOR}{Clock(time.ToString("HH"), time.ToString("mm"))}  "
                          + $"{IRC.CYAN}{time.ToString("HH:mm:ss tt")}{IRC.NOCOLOR} {SunMoon(time.ToString("HH"))} "
                          + $"(📅{IRC.CYAN}{time.ToString("dd/MM/yyyy")}{IRC.NOCOLOR})";
            return message;
        }

        private static string Clock(string hours, string minutes)
        {
            var clock = string.Empty;
            if (Convert.ToInt32(minutes) > 30)
            {
                switch (hours)
                {
                    case "1":
                    case "13":
                        clock = "🕜";
                        break;
                    case "2":
                    case "14":
                        clock = "🕝";
                        break;
                    case "3":
                    case "15":
                        clock = "🕞";
                        break;
                    case "4":
                    case "16":
                        clock = "🕟";
                        break;
                    case "5":
                    case "17":
                        clock = "🕠";
                        break;
                    case "6":
                    case "18":
                        clock = "🕡";
                        break;
                    case "7":
                    case "19":
                        clock = "🕢";
                        break;
                    case "8":
                    case "20":
                        clock = "🕣";
                        break;
                    case "9":
                    case "21":
                        clock = "🕤";
                        break;
                    case "10":
                    case "22":
                        clock = "🕥";
                        break;
                    case "11":
                    case "23":
                        clock = "🕦";
                        break;
                    case "0":
                    case "00":
                    case "24":
                        clock = "🕧";
                        break;
                }
            }
            else
            {
                switch (hours)
                {
                    case "1":
                    case "13":
                        clock = "🕐";
                        break;
                    case "2":
                    case "14":
                        clock = "🕑";
                        break;
                    case "3":
                    case "15":
                        clock = "🕒";
                        break;
                    case "4":
                    case "16":
                        clock = "🕓";
                        break;
                    case "5":
                    case "17":
                        clock = "🕔";
                        break;
                    case "6":
                    case "18":
                        clock = "🕕";
                        break;
                    case "7":
                    case "19":
                        clock = "🕖";
                        break;
                    case "8":
                    case "20":
                        clock = "🕗";
                        break;
                    case "9":
                    case "21":
                        clock = "🕘";
                        break;
                    case "10":
                    case "22":
                        clock = "🕙";
                        break;
                    case "11":
                    case "23":
                        clock = "🕚";
                        break;
                    case "0":
                    case "00":
                    case "24":
                        clock = "🕛";
                        break;
                }
            }
            return clock;
        }

        private static string SunMoon(string time) => Convert.ToInt32(time) >= 18 || (Convert.ToInt32(time) <= 8) ? "🌙" : "☀️";

        public static void SetupIRCClient()
        {
            Program.client.ConnectionComplete += (s, e) =>
            {
                SemiColoredWrite(ConsoleColor.Yellow, "[IRC] ", "Identifying to NickServ.");
                Program.client.SendMessage("identify " + Config.IRC_NSPassword, "NickServ");
                Thread.Sleep(200);
                SemiColoredWrite(ConsoleColor.Yellow, "[IRC] ", "Enabling vHost.");
                Program.client.SendMessage("hs on", "HostServ");
                Thread.Sleep(200);
                SemiColoredWrite(ConsoleColor.Yellow, "[IRC] ", "Joining default channel.");
                Program.client.JoinChannel(Config.IRC_ChannelName);
                SemiColoredWrite(ConsoleColor.Yellow, "[IRC] ", "Joined channel, enabling menu.");
                Thread.Sleep(100);
                Program.MenuEnabled = true;
            };

            Program.client.ChannelMessageRecieved += (s, e) =>
            {
                if (e.PrivateMessage.Message.Equals("!whoami"))
                {
                    UserHandler.WhoAmI(e.PrivateMessage.User.Nick, e.PrivateMessage.Source);
                }
                if (e.PrivateMessage.Message.StartsWith("!quote read "))
                {
                    QuoteHandler.Read(e.PrivateMessage.Message, e.PrivateMessage.Source);
                }
                if (e.PrivateMessage.Message.StartsWith("!quote add "))
                {
                    if (UserHandler.isUser(e.PrivateMessage.User.Nick))
                    {
                        QuoteHandler.Add(e.PrivateMessage.Message, e.PrivateMessage.User.Nick, e.PrivateMessage.Source);
                    }
                    else
                    {
                        UserHandler.DenyAccess(e.PrivateMessage.User.Nick, e.PrivateMessage.Source);
                    }
                }
                if (e.PrivateMessage.Message.Equals("!quote random"))
                {
                    QuoteHandler.ReadRandom(e.PrivateMessage.Source);
                }
            };

            Program.client.PrivateMessageRecieved += (s, e) =>
            {
                if (!e.PrivateMessage.Source.StartsWith("#"))
                {
                    QueryHandler.HandleMSG(e.PrivateMessage.Message, e.PrivateMessage.User.Nick);
                }
                if (e.PrivateMessage.Message.Equals("\x01VERSION\x01"))
                {
                    var msg = $"\x01VERSION {IRC.NOCOLOR}💩 IRCBot.Quotes 💩\x01";
                    Program.client.SendNotice(msg, e.PrivateMessage.User.Nick);
                    SemiColoredWrite(ConsoleColor.Magenta, "[CTCP:VERSION] ", $"Responded to request from {e.PrivateMessage.User.Nick}");
                }
                if (e.PrivateMessage.Message.Equals("\x01TIME\x01"))
                {
                    var msg = $"\x01TIME  {BuildCTCPTime()} \x01";
                    Program.client.SendNotice(msg, e.PrivateMessage.User.Nick);
                    SemiColoredWrite(ConsoleColor.Magenta, "[CTCP:TIME] ", $"Responded to request from {e.PrivateMessage.User.Nick}");
                }
            };
        }
    }
}