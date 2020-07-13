using AgendagRPCProto;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;
using System.Linq;

namespace AgendagRPCClient
{
    class Program
    {
        private static Agenda.AgendaClient client;

        static void Main(string[] args)
        {
            try
            {
                var channel = new Channel("127.0.0.1:5000", ChannelCredentials.Insecure);
                client = new Agenda.AgendaClient(channel);

                while (true)
                {
                    Console.WriteLine("CAD:  cadastrar um novo");
                    Console.WriteLine("CON:  consultar um contato");
                    Console.WriteLine("EX:   excluir");
                    Console.WriteLine("L:    listar");
                    Console.WriteLine("Q:    sair");

                    Console.Write("Digite um comando: ");
                    var comando = Console.ReadLine().ToUpper();

                    if (comando == "CAD")
                        Cadastrar();
                    else if (comando == "CON")
                        Consultar();
                    else if (comando == "EX")
                        Excluir();
                    else if (comando == "L")
                        Listar();
                    else if (comando == "Q")
                        break;
                    else
                        Console.WriteLine("Comando inválido");

                    Console.WriteLine("-------------------------------------------------");
                }

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.ToString());
                Console.ResetColor();
                Console.ReadKey();
            }
        }

        private static void Cadastrar()
        {
            Console.Write("Nome: ");
            var nomeContato = Console.ReadLine();
            Console.Write("Email: ");
            var emailContato = Console.ReadLine();
            var dataContato = LerData("Data Nascimento (DD/MM/YYYY): ");

            var response = client.Cadastrar(new CadastrarRequest
            {
                Nome = nomeContato,
                Email = emailContato,
                DataNascimento = Timestamp.FromDateTime(dataContato.ToUniversalTime())
            });
            if (response.Status == CadastrarResponse.Types.Status.Sucesso)
                Console.WriteLine($"{response.Contato.Email} foi cadastrado com o id {response.Contato.Id}");
            else if (response.Status == CadastrarResponse.Types.Status.JaCadastrado)
                Console.WriteLine($"{response.Contato.Email} já está cadastrado com o id {response.Contato.Id}");
            else
                throw new Exception(response.MensagemErro);
        }

        private static void Consultar()
        {
            var id = LerUInt("Digite o Id do contato: ");
            if (id <= 0)
                return;

            var response = client.Consultar(new ConsultarRequest { Id = id });
            if (response.Encontrado)
            {
                Console.WriteLine($"{response.Contato.Nome} foi encontrada com sucesso");
            }
            else
            {
                Console.WriteLine("Contato não foi encontrado");
            }
        }

        private static void Excluir()
        {
            var id = LerUInt("Digite o Id do contato que deseja excluir: ");
            if (id <= 0)
                return;

            var response = client.Excluir(new ExcluirRequest { Id = id });
            if (response.Sucesso)
                Console.WriteLine("Excluido com sucesso");
            else
                Console.WriteLine(response.MensagemErro);
        }

        private static void Listar()
        {
            var response = client.ConsultarTodos(new ConsultarTodosRequest());
            foreach (var c in response.Contatos)
            {
                Console.WriteLine($"{c.Id,-3} {c.DataNascimento.ToDateTime():d} {c.Nome,-25} {c.Email}");
            }
        }

        private static DateTime LerData(string mensagem)
        {
            while (true)
            {
                try
                {
                    Console.Write(mensagem);
                    var data = Console.ReadLine().Split('/').Select(x => int.Parse(x)).ToArray();
                    return new DateTime(data[2], data[1], data[0]);
                }
                catch
                {
                    Console.WriteLine("Data inválida");
                }
            }
        }

        private static uint LerUInt(string mensagem)
        {
            while (true)
            {
                try
                {
                    Console.Write(mensagem);
                    return uint.Parse(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine("Número inválido");
                }
            }
        }
    }
}
