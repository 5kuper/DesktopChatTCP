using System;
using System.Globalization;
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

        #region Field values

        private string _username = string.Empty;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
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

        private string _port = "8888";

        public string Port
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

        #endregion

        #region Field options

        private bool _usernameFieldEnabled = true;

        public bool UsernameFieldEnabled
        {
            get => _usernameFieldEnabled;
            set
            {
                _usernameFieldEnabled = value;
                OnPropertyChanged(nameof(UsernameFieldEnabled));
            }
        }

        private bool _hostAndPortFieldsEnabled = true;

        public bool HostAndPortFieldsEnabled
        {
            get => _hostAndPortFieldsEnabled;
            set
            {
                _hostAndPortFieldsEnabled = value;
                OnPropertyChanged(nameof(HostAndPortFieldsEnabled));
            }
        }

        private bool _renameButtonEnabled;

        public bool RenameButtonEnabled
        {
            get => _renameButtonEnabled;
            set
            {
                _renameButtonEnabled = value;
                OnPropertyChanged(nameof(RenameButtonEnabled));
            }
        }

        private bool _connectOrDisconnectButtonEnabled = true;

        public bool ConnectOrDisconnectButtonEnabled
        {
            get => _connectOrDisconnectButtonEnabled;
            set
            {
                _connectOrDisconnectButtonEnabled = value;
                OnPropertyChanged(nameof(ConnectOrDisconnectButtonEnabled));
            }
        }

        private string _renameButtonContent = "Rename";

        public string RenameButtonContent
        {
            get => _renameButtonContent;
            set
            {
                _renameButtonContent = value;
                OnPropertyChanged(nameof(RenameButtonContent));
            }
        }

        private string _connectOrDisconnectButtonContent = "Connect";

        public string ConnectOrDisconnectButtonContent
        {
            get => _connectOrDisconnectButtonContent;
            set
            {
                _connectOrDisconnectButtonContent = value;
                OnPropertyChanged(nameof(ConnectOrDisconnectButtonContent));
            }
        }

        #endregion

        #region Button commands

        public ICommand ConnectOrDisconnectCommand
        {
            get
            {
                return new RelayCommand(o =>
                {
                    if (!Client.Connected)
                    {
                        // Connect button
                        if (Username.Trim(' ') == string.Empty)
                        {
                            Log("Enter your username!");
                        }
                        else if (Host.Trim(' ') == string.Empty)
                        {
                            Log("Enter server host!");
                        }
                        else if (!int.TryParse(Port, out var port))
                        {
                            Log("Enter server port!");
                        }
                        else
                        {
                            Client.Username = Username;
                            Client.Connect(Host, port);
                        }
                    }
                    else
                    {
                        // Disconnect button
                        Client.Disconnect();
                    }
                });
            }
        }

        public ICommand SendMessageCommand
        {
            get
            {
                return new RelayCommand(o =>
                {
                    Client.SendPacket(new MessagePacket(Message));
                    DisplayMessage("You", Message);
                    Message = null;
                });
            }
        }

        public ICommand UpdateUsernameCommand
        {
            get
            {
                return new RelayCommand(o =>
                {
                    if (RenameButtonContent == "Rename")
                    {
                        // Rename button
                        RenameButtonContent = "Send Request";
                        UsernameFieldEnabled = true;
                    }
                    else
                    {
                        // Send Request button
                        if (Username?.Trim(' ') == string.Empty)
                        {
                            Log("Username can't be empty!");
                            return;
                        }
                        else if (Username == Client.Username)
                        {
                            Log("Renaming canceled.");
                        }
                        else
                        {
                            // TODO: Send renaming request
                            Log("Renaming request sent.");
                        }

                        RenameButtonContent = "Rename";
                        UsernameFieldEnabled = false;
                    }

                    Client.Username = Username;
                    ConnectOrDisconnectButtonEnabled = true;
                });
            }
        }

        #endregion

        public MainWindowViewModel()
        {
            Client = new Client();
            Client.OnLog += Log;
            Client.OnDisplayMessage += DisplayMessage;
            Client.OnConnectionStatusChanged += HandleConnectionStatus;
        }

        private void Log(string text)
        {
            Chat += $"{DateTime.Now.Hour}:{DateTime.Now.Minute} {text}\n";
        }

        private void DisplayMessage(string sender, string content)
        {
            Chat += $"{DateTime.Now.Hour}:{DateTime.Now.Minute} {sender}: {content}\n";
        }

        private void HandleConnectionStatus()
        {
            if (Client.Connected)
            {
                SetConnectedState();
            }
            else
            {
                SetDisconnectedState();
            }
        }

        private void SetConnectedState()
        {
            UsernameFieldEnabled = false;
            HostAndPortFieldsEnabled = false;

            ConnectOrDisconnectButtonContent = "Disconnect";

            RenameButtonContent = "Rename";
            RenameButtonEnabled = true;

        }

        private void SetDisconnectedState()
        {
            UsernameFieldEnabled = true;
            HostAndPortFieldsEnabled = true;

            ConnectOrDisconnectButtonContent = "Connect";

            RenameButtonContent = "Rename";
            RenameButtonEnabled = false;
        }
    }
}
