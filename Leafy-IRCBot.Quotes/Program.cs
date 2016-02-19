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
using ChatSharp;
using IRCBot.Quotes.Classes;

#endregion

namespace IRCBot.Quotes
{
    internal static class Program
    {
        public static IrcClient client;

        public static bool MenuEnabled;

        private static void Main()
        {
            if (!Config.Init())
            {
                Environment.Exit(1);
            }
            else
            {
                Config.Populate();
            }

            Console.Title = $"{Config.IRC_BotName} on {Config.IRC_Server} in channel {Config.IRC_ChannelName}.";
            Tools.ColoredWrite(ConsoleColor.Green, "*** Initialising " + Console.Title);

            Tools.SemiColoredWrite(ConsoleColor.Yellow, "[IRC] ", "Initialising IRC client.");
            client = new IrcClient(Config.IRC_Server,
                new IrcUser(Config.IRC_BotName, Config.IRC_BotName, Config.IRC_Password), Config.IRC_SSL);

            client.ConnectAsync();
            Tools.SemiColoredWrite(ConsoleColor.Yellow, "[IRC] ", "Connected to IRC server.");

            Tools.SetupIRCClient();

            do
            {
                Thread.Sleep(100);
            } while (!MenuEnabled);

            Start();
        }

        private static void Start()
        {
            string command;
            do
            {
                Console.ReadKey(true);
                Console.Write("> ");
                command = Console.ReadLine();
                switch (command)
                {
                    case "exit":
                        break;
                    case "say":
                        Tools.ColoredWrite(ConsoleColor.DarkGray, "--- say");
                        Tools.ColoredWrite(ConsoleColor.Cyan, "Enter message to send:");
                        var msg = Console.ReadLine();
                        Tools.ColoredWrite(ConsoleColor.Cyan, "Enter channel to send to:");
                        var channel = Console.ReadLine();
                        client.Channels[channel].SendMessage(msg);
                        Tools.ColoredWrite(ConsoleColor.DarkGray, "--- end_say");
                        continue;
                    case "join":
                        Tools.ColoredWrite(ConsoleColor.DarkGray, "--- join");
                        Tools.ColoredWrite(ConsoleColor.Cyan, "Enter channel to join:");
                        client.JoinChannel(Console.ReadLine());
                        Tools.ColoredWrite(ConsoleColor.DarkGray, "--- end_join");
                        continue;
                    case "part":
                        Tools.ColoredWrite(ConsoleColor.DarkGray, "--- part");
                        Tools.ColoredWrite(ConsoleColor.Cyan, "Enter channel to part:");
                        client.PartChannel(Console.ReadLine());
                        Tools.ColoredWrite(ConsoleColor.DarkGray, "--- end_part");
                        continue;
                    default:
                        Tools.ColoredWrite(ConsoleColor.DarkGray, "--- err");
                        Tools.ColoredWrite(ConsoleColor.Red, "Invalid command.");
                        Tools.ColoredWrite(ConsoleColor.DarkGray, "--- end_err");
                        continue;
                        // TODO Uptime, Stats.
                }
            } while (command != "exit");
        }
    }
}