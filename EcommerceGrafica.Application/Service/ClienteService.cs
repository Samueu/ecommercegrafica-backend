using System.Text.RegularExpressions;
using EcommerceGrafica.Domain.Exceptions;
using EcommerceGrafica.Domain.Interface.Repository;
using EcommerceGrafica.Domain.Interface.Service;
using EcommerceGrafica.Domain.Model;

namespace EcommerceGrafica.Application.Service
{
    public partial class ClienteService(IClienteRepository clienteRepository) : IClienteService
    {
        private readonly IClienteRepository _clienteRepository = clienteRepository;

        public async Task<IEnumerable<ClienteModel>> ListarTodos()
        {
            return await _clienteRepository.ListarTodos();
        }

        public async Task<ClienteModel?> ObterPorId(int id)
        {
            if (id <= 0)
                throw new DomainException("O identificador do cliente é obrigatório.");

            return await _clienteRepository.GetById(id);
        }

        public async Task<ClienteModel> RegistrarCliente(ClienteModel cliente)
        {
            if (cliente is null)
                throw new DomainException("O cliente é obrigatório.");

            if (string.IsNullOrWhiteSpace(cliente.Nome))
                throw new DomainException("O nome do cliente é obrigatório.");

            if (string.IsNullOrWhiteSpace(cliente.Email))
                throw new DomainException("O e-mail do cliente é obrigatório.");

            var emailNormalizado = cliente.Email.Trim().ToLowerInvariant();
            if (!EmailRegex().IsMatch(emailNormalizado))
                throw new DomainException("O e-mail informado é inválido.");

            var existente = await _clienteRepository.GetByEmail(emailNormalizado);
            if (existente is not null)
                throw new DomainException("Já existe um cliente cadastrado com esse e-mail.");

            cliente.Nome = cliente.Nome.Trim();
            cliente.Email = emailNormalizado;
            cliente.Telefone = string.IsNullOrWhiteSpace(cliente.Telefone) ? null : cliente.Telefone.Trim();
            cliente.CadastradoEm = DateTime.UtcNow;

            await _clienteRepository.RegisterCliente(cliente);
            return cliente;
        }

        [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
        private static partial Regex EmailRegex();
    }
}
