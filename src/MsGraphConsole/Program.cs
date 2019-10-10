using System;
using System.Configuration;

namespace MsGraphConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var clientId = ConfigurationManager.AppSettings["clientId"];

            Console.WriteLine("Hello World! {0}", clientId);
        }
    }
}
