using System.Text.RegularExpressions;
using EcommerceGrafica.Domain.Shared;

namespace EcommerceGrafica.Domain.Customers.ValueObjects;

public sealed partial class Email : ValueObject
{
    public string Valor { get; private set; } = string.Empty;

    private Email()
    {
    }

    public Email(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new DomainException("O e-mail é obrigatório.");

        var emailNormalizado = valor.Trim().ToLowerInvariant();

        if (!EmailRegex().IsMatch(emailNormalizado))
            throw new DomainException("O e-mail informado é inválido.");

        Valor = emailNormalizado;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Valor;
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}
