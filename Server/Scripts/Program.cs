using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using SCS.System;
using SCS.Commands;

namespace ServerSide
{
    internal class Program
    {
        private static Server _chatServer;
        private static NamedPipeServerStream _pipeServer;
        private static StreamWriter _pipeWriter;

        private static void Main(string[] args)
        {
            Console.Title = "Server";
            Command.StandardPrefix = "/";
            Command.RegisterCommands<Program>();
            Command.RegisterCommands<MainCommands>();

            string pipeToken = Guid.NewGuid().ToString();
            _pipeServer = new NamedPipeServerStream(pipeToken, PipeDirection.InOut);
            Process.Start(new ProcessStartInfo("Logger.exe") { UseShellExecute = true, Arguments = pipeToken });
                        _pipeWriter = new StreamWriter(_pipeServer);
            _pipeServer.WaitForConnection();

            AdvancedConsole.ColoredWriteLine(new ColoredString("Enter "),
                new ColoredString(ConsoleColor.DarkCyan, "/"), new ColoredString(ConsoleColor.Cyan, "help "),
                new ColoredString("to get a list of commands. Don't use commas between arguments and use string arguments in quotes.\n"));

            while (true)
            {
                Console.Write(" > ");
                string message = Console.ReadLine();

                Console.WriteLine();
                Command.Execute(message);
                Console.WriteLine();
            }
        }

        private static void Log(string text)
        {
            _pipeWriter.WriteLine(text);
            _pipeWriter.Flush();
        }

        [Command(null, "start", "Starts the server on the specified port.")]
        private static void StartServer(int port)
        {
            _chatServer = new Server(port);
            _chatServer.OnLog += Log;
            _chatServer.Start();

            Console.WriteLine("Command processed, check server log.");
        }

        [Command(null, "stop", "Stops the server.")]
        private static void StopServer()
        {
            if (_chatServer != null)
            {
                _chatServer.Stop();
                Console.WriteLine("Command processed, check server log.");
            }
            else
            {
                Console.WriteLine("Server is not running!");
            }
        }
    }
}
