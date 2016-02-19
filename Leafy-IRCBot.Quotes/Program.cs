// -----------------------------------------------------------
// This program is private software, based on C# source code.
// To sell or change credits of this software is forbidden,
// except if someone approves it from the LeafyCoding INC. team.
// -----------------------------------------------------------
// Copyrights (c) 2016 Leafy-IRCBot.Quotes INC. All rights reserved.
// -----------------------------------------------------------

using System;
using IRCBot.Quotes.Classes;

namespace IRCBot.Quotes
{
    internal static class Program
    {
        private static void Main()
        {
            if (!Config.Init())
            {
                Environment.Exit(1);
            }


        }
    }
}