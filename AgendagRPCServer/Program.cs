using AgendagRPCProto;
using Grpc.Core;
using System;

namespace AgendagRPCServer
{
    class Program
    {
        const int _port = 5000;

        public static void Main(string[] args)
        {
            Server server = new Server
            {
                Services = {
                    Agenda.BindService(new AgendaImpl()),
                },
                Ports = { new ServerPort("localhost", _port, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("Servidor está em execução. Pressione qualquer tecla para fechar");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}
