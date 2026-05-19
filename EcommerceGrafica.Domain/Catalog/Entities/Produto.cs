using EcommerceGrafica.Domain.Catalog.Enums;
using EcommerceGrafica.Domain.Catalog.Events;
using EcommerceGrafica.Domain.Catalog.ValueObjects;
using EcommerceGrafica.Domain.Shared;

namespace EcommerceGrafica.Domain.Catalog.Entities;

public sealed class Produto : AggregateRoot
{
    public string Nome { get; private set; } = string.Empty;
    public string Descricao { get; private set; } = string.Empty;
    public Preco Preco { get; private set; } = null!;
    public TipoProduto Tipo { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime CriadoEm { get; private set; }

    private Produto()
    {
    }

    public static Produto Criar(string nome, string descricao, Preco preco, TipoProduto tipo)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome do produto é obrigatório.");

        if (preco is null)
            throw new DomainException("O preço do produto é obrigatório.");

        var produto = new Produto
        {
            Id = Guid.NewGuid(),
            Nome = nome.Trim(),
            Descricao = descricao?.Trim() ?? string.Empty,
            Preco = preco,
            Tipo = tipo,
            Ativo = true,
            CriadoEm = DateTime.UtcNow
        };

        produto.RaiseDomainEvent(new ProdutoCriadoEvent(produto.Id, produto.Nome));
        return produto;
    }

    public void Atualizar(string nome, string descricao, Preco preco, TipoProduto tipo)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome do produto é obrigatório.");

        if (preco is null)
            throw new DomainException("O preço do produto é obrigatório.");

        Nome = nome.Trim();
        Descricao = descricao?.Trim() ?? string.Empty;
        Preco = preco;
        Tipo = tipo;
    }

    public void AtualizarPreco(Preco novoPreco)
    {
        if (novoPreco is null)
            throw new DomainException("O preço do produto é obrigatório.");

        Preco = novoPreco;
    }

    public void Desativar() => Ativo = false;

    public void Ativar() => Ativo = true;
}
