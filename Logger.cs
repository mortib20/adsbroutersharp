using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADSBRouterSharp
{

    internal class Logger
    {
        readonly string name;
        public Logger(string name)
        {
            this.name = name;
        }

        public void Info(string msg)
        {
            Console.WriteLine($"{DateTime.Now:u} | INFO | {name} | {msg}");
        }

        public void Error(string msg)
        {
            Console.Write($"{DateTime.Now:u} | ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("ERROR");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($" | {name} | {msg}");
        }
    }
}
