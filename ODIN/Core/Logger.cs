using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace IslaBot.Discord
{
    class Logger : ILogger
    {
        public void Log(string message)
        {
            //MainWindow.ConsoleLog(message);
            Console.WriteLine(message);
        }
    }
}
