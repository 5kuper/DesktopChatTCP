using System;
using System.Windows;
using System.Windows.Input;
using ClientSide.Commands;
using ClientSide.Models;
using Packets;

namespace ClientSide.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        public Client Client { get; set; }

        public string Username
        {
            get => Client.Username;
            set
            {
                Client.Username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        private string _host = "127.0.0.1";
        public string Host
        {
            get => _host;
            set
            {
                _host = value;
                OnPropertyChanged(nameof(Host));
            }
        }

        private int _port = 8888;
        public int Port
        {
            get => _port;
            set
            {
                _port = value;
                OnPropertyChanged(nameof(Port));
            } 
        }

        private string _chat;
        public string Chat
        {
            get => _chat;
            set
            {
                _chat = value;
                OnPropertyChanged(nameof(Chat));

            }
        }

        private string _message;
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));

            }
        }

        public ICommand UpdateUsernameCommand
        {
            get
            {
                return new RelayCommand(o =>
                {
                    
                });
            }
        }

        public ICommand ConnectOrDisconnectCommand
        {
            get {
                return new RelayCommand(o =>
                {
                    Client.Connect(Host, Port);
                });
            }
        }

        public ICommand SendMessageCommand
        {
            get
            {
                return new RelayCommand(o =>
                {
                    Client.SendPacket(new MessagePacket(Username, Message));
                    WriteMessage("You", Message);
                    Message = null;
                });
            }
        }

        public MainWindowViewModel()
        {
            Client = new Client();
            Client.OnLog += Log;
            Client.OnWriteMessage += WriteMessage;
        }

        public void Log(string text)
        {
            Chat += text + "\n";
        }

        public void WriteMessage(string sender, string content)
        {
            Chat += $"{DateTime.Now.Hour}:{DateTime.Now.Minute} {sender}: {content}\n";
        }
    }
}
