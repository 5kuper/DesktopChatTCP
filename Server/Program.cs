using System;

namespace Server
{
    internal class Program
    {
        private static void Main()
        {
            Server server = new Server(8888);
            server.Start();

            Console.ReadKey();
        }

        public static void Log(string text)
        {
            Console.WriteLine($"{DateTime.Now}: {text}");
        }
    }
}
