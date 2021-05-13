using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using Packets;
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
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.WindowHeight = 20;
            }

            Console.Title = "Server";
            Command.StandardPrefix = "/";
            Command.RegisterCommands<Program>();
            Command.RegisterCommands<MainCommands>();

            StartLogger();

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

        public static void StartLogger()
        {
            string pipeToken = Guid.NewGuid().ToString();
            _pipeServer = new NamedPipeServerStream(pipeToken, PipeDirection.InOut);
            Process.Start(new ProcessStartInfo("Logger.exe") { UseShellExecute = true, Arguments = pipeToken });
            _pipeWriter = new StreamWriter(_pipeServer);
            _pipeServer.WaitForConnection();
        }

        private static void Log(string text)
        {
            try
            {
                _pipeWriter.WriteLine($"{DateTime.Now} {text}");
                _pipeWriter.Flush();
            }
            catch
            {
                StartLogger();
                Log(text);
            }
        }

        private static void StartServer(int port, int maxConnections = -1)
        {
            if (_chatServer == null)
            {
                _chatServer = new Server(port, maxConnections);
                _chatServer.OnLog += Log;
                _chatServer.Start();

                Console.WriteLine("Command processed, check server log.");
            }
            else
            {
                Console.WriteLine("Server is already running!");
            }
        }

        [Command(null, "start", "Starts the server on the specified port.")]
        private static void StartServerCommand(int port)
        {
            StartServer(port);
        }

        [Command(null, "start", "Starts the server with the specified maximum of connections.")]
        private static void StartServerCommand(int port, int maxConnections)
        {
            StartServer(port, maxConnections);
        }

        [Command(null, "send", "Sends a message on behalf of the server.")]
        private static void SendMessageCommand(string message)
        {
            if (_chatServer != null)
            {
                _chatServer.BroadcastPacket(new MessagePacket("Server", message));
                _chatServer.Log($"Server admin said: \"{message}\"");
                Console.WriteLine("Command processed, check server log.");
            }
            else
            {
                Console.WriteLine("Server is not running!");
            }
        }

        [Command(null, "kick", "Kicks the user with the specified username.")]
        private static void KickUserCommand(string username)
        {
            if (_chatServer != null)
            {
                _chatServer.KickUser(username);
                Console.WriteLine("Command processed, check server log.");
            }
            else
            {
                Console.WriteLine("Server is not running!");
            }
        }

        [Command(null, "stop", "Stops the server.")]
        private static void StopServerCommand()
        {
            if (_chatServer != null)
            {
                _chatServer.Stop();
                _chatServer = null;
                Console.WriteLine("Command processed, check server log.");
            }
            else
            {
                Console.WriteLine("Server is not running!");
            }
        }
    }
}
