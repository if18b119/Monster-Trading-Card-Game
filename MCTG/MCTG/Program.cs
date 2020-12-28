using System;
using MCTGclass;

using RestAPIServerLib;
using System.Threading;

namespace MCTG
{
    class Program
    {
        static void Main(string[] args)

        {            
            MyServer server = new MyServer();
            server.StartListening();
            //Thread thread = new Thread(new ThreadStart(server.StartListening));
            //thread.Start();

        }
    }
}
