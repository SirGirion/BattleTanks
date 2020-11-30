using ENet;
using System;

namespace BattleTanksServer
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = "localhost";
            ushort port = 7000;
			if (args.Length >= 3)
            {
                host = args[1];
                port = Convert.ToUInt16(args[2]);
            }
            using (var server = new Server(host, port))
            {
                server.StartServer();
                server.Run();
            }
		}
    }
}
