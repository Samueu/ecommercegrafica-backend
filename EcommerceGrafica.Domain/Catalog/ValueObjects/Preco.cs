using EcommerceGrafica.Domain.Shared;

namespace EcommerceGrafica.Domain.Catalog.ValueObjects;

public sealed class Preco : ValueObject
{
    public decimal Valor { get; private set; }
    public string Moeda { get; private set; } = "BRL";

    private Preco()
    {
    }

    public Preco(decimal valor, string moeda = "BRL")
    {
        if (valor < 0)
            throw new DomainException("O preço não pode ser negativo.");

        if (string.IsNullOrWhiteSpace(moeda))
            throw new DomainException("A moeda é obrigatória.");

        Valor = valor;
        Moeda = moeda.ToUpperInvariant();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Valor;
        yield return Moeda;
    }
}
