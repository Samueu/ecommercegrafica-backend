using EcommerceGrafica.Domain.Shared;

namespace EcommerceGrafica.Domain.Orders.ValueObjects;

public sealed class EnderecoEntrega : ValueObject
{
    public string Logradouro { get; private set; } = string.Empty;
    public string Numero { get; private set; } = string.Empty;
    public string Bairro { get; private set; } = string.Empty;
    public string Cidade { get; private set; } = string.Empty;
    public string Estado { get; private set; } = string.Empty;
    public string Cep { get; private set; } = string.Empty;

    private EnderecoEntrega()
    {
    }

    public EnderecoEntrega(
        string logradouro,
        string numero,
        string bairro,
        string cidade,
        string estado,
        string cep)
    {
        if (string.IsNullOrWhiteSpace(logradouro))
            throw new DomainException("O logradouro é obrigatório.");

        if (string.IsNullOrWhiteSpace(cidade))
            throw new DomainException("A cidade é obrigatória.");

        if (string.IsNullOrWhiteSpace(cep))
            throw new DomainException("O CEP é obrigatório.");

        Logradouro = logradouro.Trim();
        Numero = numero?.Trim() ?? string.Empty;
        Bairro = bairro?.Trim() ?? string.Empty;
        Cidade = cidade.Trim();
        Estado = estado?.Trim().ToUpperInvariant() ?? string.Empty;
        Cep = cep.Trim();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Logradouro;
        yield return Numero;
        yield return Bairro;
        yield return Cidade;
        yield return Estado;
        yield return Cep;
    }
}
