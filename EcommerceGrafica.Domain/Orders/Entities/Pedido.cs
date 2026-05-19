using EcommerceGrafica.Domain.Orders.Enums;
using EcommerceGrafica.Domain.Orders.ValueObjects;
using EcommerceGrafica.Domain.Shared;

namespace EcommerceGrafica.Domain.Orders.Entities;

public sealed class Pedido : AggregateRoot
{
    private readonly List<ItemPedido> _itens = [];

    public Guid ClienteId { get; private set; }
    public StatusPedido Status { get; private set; }
    public EnderecoEntrega? EnderecoEntrega { get; private set; }
    public IReadOnlyCollection<ItemPedido> Itens => _itens.AsReadOnly();
    public DateTime CriadoEm { get; private set; }

    private Pedido()
    {
    }

    public static Pedido Criar(Guid clienteId)
    {
        if (clienteId == Guid.Empty)
            throw new DomainException("O cliente do pedido é obrigatório.");

        return new Pedido
        {
            Id = Guid.NewGuid(),
            ClienteId = clienteId,
            Status = StatusPedido.Rascunho,
            CriadoEm = DateTime.UtcNow
        };
    }

    public void AdicionarItem(Guid produtoId, string nomeProduto, int quantidade, decimal precoUnitario)
    {
        if (Status != StatusPedido.Rascunho)
            throw new DomainException("Somente pedidos em rascunho podem receber itens.");

        _itens.Add(ItemPedido.Criar(produtoId, nomeProduto, quantidade, precoUnitario));
    }

    public void DefinirEnderecoEntrega(EnderecoEntrega endereco)
    {
        if (endereco is null)
            throw new DomainException("O endereço de entrega é obrigatório.");

        EnderecoEntrega = endereco;
    }

    public void Confirmar()
    {
        if (!_itens.Any())
            throw new DomainException("O pedido precisa ter ao menos um item.");

        if (EnderecoEntrega is null)
            throw new DomainException("O pedido precisa de um endereço de entrega.");

        Status = StatusPedido.AguardandoPagamento;
    }

    public decimal Total() => _itens.Sum(i => i.Subtotal());
}
