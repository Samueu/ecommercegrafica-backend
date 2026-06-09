using EcommerceGrafica.Domain.Enums;
using EcommerceGrafica.Domain.Exceptions;
using EcommerceGrafica.Domain.Interface.Repository;
using EcommerceGrafica.Domain.Interface.Service;
using EcommerceGrafica.Domain.Model;

namespace EcommerceGrafica.Application.Service
{
    public class PedidoService(
        IPedidoRepository pedidoRepository,
        IClienteRepository clienteRepository,
        IProdutoRepository produtoRepository) : IPedidoService
    {
        private readonly IPedidoRepository _pedidoRepository = pedidoRepository;
        private readonly IClienteRepository _clienteRepository = clienteRepository;
        private readonly IProdutoRepository _produtoRepository = produtoRepository;

        public async Task<IEnumerable<PedidoModel>> ListarTodos()
        {
            return await _pedidoRepository.ListarTodos();
        }

        public async Task<PedidoModel?> ObterPorId(Guid id)
        {
            if (id == Guid.Empty)
                throw new DomainException("O identificador do pedido é obrigatório.");

            return await _pedidoRepository.GetById(id);
        }

        public async Task<IEnumerable<PedidoModel>> ListarPorCliente(Guid clienteId)
        {
            if (clienteId == Guid.Empty)
                throw new DomainException("O identificador do cliente é obrigatório.");

            return await _pedidoRepository.ListarPorCliente(clienteId);
        }

        public async Task<PedidoModel> RegistrarPedido(PedidoModel pedido)
        {
            if (pedido is null)
                throw new DomainException("O pedido é obrigatório.");

            if (pedido.ClienteId == Guid.Empty)
                throw new DomainException("O cliente do pedido é obrigatório.");

            var cliente = await _clienteRepository.GetById(pedido.ClienteId);
            if (cliente is null)
                throw new DomainException("Cliente não encontrado.");

            if (pedido.Itens is null || pedido.Itens.Count == 0)
                throw new DomainException("O pedido precisa ter ao menos um item.");

            if (string.IsNullOrWhiteSpace(pedido.Logradouro) ||
                string.IsNullOrWhiteSpace(pedido.Cidade) ||
                string.IsNullOrWhiteSpace(pedido.Cep))
                throw new DomainException("O endereço de entrega é obrigatório (logradouro, cidade e CEP).");

            pedido.Id = Guid.NewGuid();
            pedido.Status = StatusPedido.AguardandoPagamento;
            pedido.CriadoEm = DateTime.UtcNow;
            pedido.Estado = pedido.Estado?.Trim().ToUpperInvariant();

            foreach (var item in pedido.Itens)
            {
                if (item.ProdutoId == Guid.Empty)
                    throw new DomainException("O produto do item é obrigatório.");

                if (item.Quantidade <= 0)
                    throw new DomainException("A quantidade deve ser maior que zero.");

                if (item.PrecoUnitario < 0)
                    throw new DomainException("O preço unitário não pode ser negativo.");

                var produto = await _produtoRepository.GetById(item.ProdutoId);
                if (produto is null)
                    throw new DomainException($"Produto {item.ProdutoId} não encontrado.");

                item.Id = Guid.NewGuid();
                item.PedidoId = pedido.Id;
                item.NomeProduto = produto.Nome;
                item.PrecoUnitario = produto.Preco;
            }

            await _pedidoRepository.RegisterPedido(pedido);
            return pedido;
        }
    }
}
