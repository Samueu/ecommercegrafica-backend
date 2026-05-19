using EcommerceGrafica.Domain.Shared;

namespace EcommerceGrafica.Domain.Orders.Entities;

public sealed class ItemPedido : Entity
{
    public Guid ProdutoId { get; private set; }
    public string NomeProduto { get; private set; } = string.Empty;
    public int Quantidade { get; private set; }
    public decimal PrecoUnitario { get; private set; }

    private ItemPedido()
    {
    }

    internal static ItemPedido Criar(Guid produtoId, string nomeProduto, int quantidade, decimal precoUnitario)
    {
        if (produtoId == Guid.Empty)
            throw new DomainException("O produto do item é obrigatório.");

        if (string.IsNullOrWhiteSpace(nomeProduto))
            throw new DomainException("O nome do produto do item é obrigatório.");

        if (quantidade <= 0)
            throw new DomainException("A quantidade deve ser maior que zero.");

        if (precoUnitario < 0)
            throw new DomainException("O preço unitário não pode ser negativo.");

        return new ItemPedido
        {
            Id = Guid.NewGuid(),
            ProdutoId = produtoId,
            NomeProduto = nomeProduto.Trim(),
            Quantidade = quantidade,
            PrecoUnitario = precoUnitario
        };
    }

    public decimal Subtotal() => Quantidade * PrecoUnitario;
}
