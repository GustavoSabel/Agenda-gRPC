using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgendagRPCServer
{
    public class ContatoRepository
    {
        private static int _ultimoId = 0;
        private readonly List<Contato> _contatos = new List<Contato>();

        public static readonly ContatoRepository Instancia = new ContatoRepository();

        private ContatoRepository()
        {
            InserirAsync("Gustavo", "gustavo.sabel.gs@gmail.com", new DateTime(1991, 09, 23));
            InserirAsync("Joao", "Joao@gmail.com", new DateTime(1950, 07, 01));
            InserirAsync("Pedro", "Pedro@gmail.com", new DateTime(1975, 2, 6));
        }

        public Task<Contato> ObterPorEmailAsync(string email)
        {
            return Task.FromResult(_contatos.FirstOrDefault(x => x.Email == email));
        }

        public Task<Contato> InserirAsync(string nome, string email, DateTime dataNascimento)
        {
            var contato = new Contato
            {
                Id = ++_ultimoId,
                Nome = nome,
                Email = email,
                DataNascimento = dataNascimento,
            };
            _contatos.Add(contato);
            return Task.FromResult(contato);
        }

        internal Task<Contato> ObterPeloIdAsync(uint id)
        {
            return Task.FromResult(_contatos.FirstOrDefault(x => x.Id == id));
        }

        internal Task<List<Contato>> ObterTodosAsync()
        {
            return Task.FromResult(_contatos);
        }

        public Task RemoverAsync(Contato contato) => Task.FromResult(_contatos.Remove(contato));
    }
}
