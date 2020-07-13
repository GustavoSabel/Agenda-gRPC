using AgendagRPCProto;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;
using System.Linq;
using System.Threading.Tasks;
using Proto = AgendagRPCProto;

namespace AgendagRPCServer
{
    class AgendaImpl : Proto.Agenda.AgendaBase
    {
        private readonly ContatoRepository _repository;

        public AgendaImpl()
        {
            _repository = ContatoRepository.Instancia;
        }

        public override async Task<Proto.CadastrarResponse> Cadastrar(Proto.CadastrarRequest request, ServerCallContext context)
        {
            try
            {
                var contatoExistente = await _repository.ObterPorEmailAsync(request.Email);
                if (contatoExistente != null)
                {
                    return new Proto.CadastrarResponse
                    {
                        Status = Proto.CadastrarResponse.Types.Status.JaCadastrado,
                        MensagemErro = $"Já existe um contato com o e-mail {request.Email} cadastrado",
                        Contato = ConverterParaResponse(contatoExistente)
                    };
                }

                var contato = await _repository.InserirAsync(request.Nome, request.Email, request.DataNascimento.ToDateTime());
                return new Proto.CadastrarResponse
                {
                    Status = Proto.CadastrarResponse.Types.Status.Sucesso,
                    Contato = ConverterParaResponse(contato)
                };
            }
            catch (Exception ex)
            {
                return new Proto.CadastrarResponse
                {
                    Status = Proto.CadastrarResponse.Types.Status.Erro,
                    MensagemErro = ex.Message
                };
            }
        }

        public override async Task<Proto.ConsultarResponse> Consultar(Proto.ConsultarRequest request, ServerCallContext context)
        {
            var contato = await _repository.ObterPeloIdAsync(request.Id);

            if (contato == null)
                return new Proto.ConsultarResponse { Encontrado = false };

            return new Proto.ConsultarResponse
            {
                Contato = ConverterParaResponse(contato),
                Encontrado = true,
            };
        }

        public override async Task<Proto.ConsultarTodosResponse> ConsultarTodos(Proto.ConsultarTodosRequest request, ServerCallContext context)
        {
            var contatos = await _repository.ObterTodosAsync();
            var contatosResponse = contatos.Select(x => ConverterParaResponse(x)).ToList();

            var response = new Proto.ConsultarTodosResponse
            {
                Quantidade = (uint)contatosResponse.Count,
            };

            response.Contatos.AddRange(contatosResponse);

            return response;
        }

        public override async Task<ExcluirResponse> Excluir(ExcluirRequest request, ServerCallContext context)
        {
            var contato = await _repository.ObterPeloIdAsync(request.Id);
            if (contato == null)
                return new ExcluirResponse { Sucesso = false, MensagemErro = "Contato não encontrado" };
            await _repository.RemoverAsync(contato);
            return new ExcluirResponse { Sucesso = true };
        }

        private static Proto.Contato ConverterParaResponse(Contato contato)
        {
            return new Proto.Contato
            {
                Id = (uint)contato.Id,
                Nome = contato.Nome,
                Email = contato.Email,
                DataNascimento = Timestamp.FromDateTime(contato.DataNascimento.ToUniversalTime())
            };
        }
    }
}
