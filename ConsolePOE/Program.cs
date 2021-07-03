using System;

namespace ConsolePOE
{
    class Program
    {
        static void Main(string[] args)
        {
            Bot bot = new Bot();
            bot.Start();
            Console.WriteLine("Bot is running..");
            Console.ReadLine();
            bot.Stop();
        }
    }
}
