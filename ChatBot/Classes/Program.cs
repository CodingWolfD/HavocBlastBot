using System;
using TwitchLib;
using TwitchLib.Api;
using TwitchLib.Client;

namespace ChatBot
{
    class Program
    {
        static void Main(string[] args)
        {
            HavocBlastbot bot = new HavocBlastbot();
            bot.connect();

            Console.ReadLine();

            bot.disconnect();
        }
    }
}