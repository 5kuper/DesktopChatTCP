using System;

namespace Client.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        public string Username { get; set; }

        public string Host { get; set; }
        public int Port { get; set; }
    }
}
