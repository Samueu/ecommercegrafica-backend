using EcommerceGrafica.Domain.Customers.ValueObjects;
using EcommerceGrafica.Domain.Shared;

namespace EcommerceGrafica.Domain.Customers.Entities;

public sealed class Cliente : AggregateRoot
{
    public string Nome { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public string? Telefone { get; private set; }
    public DateTime CadastradoEm { get; private set; }

    private Cliente()
    {
    }

    public static Cliente Criar(string nome, Email email, string? telefone = null)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome do cliente é obrigatório.");

        if (email is null)
            throw new DomainException("O e-mail do cliente é obrigatório.");

        return new Cliente
        {
            Id = Guid.NewGuid(),
            Nome = nome.Trim(),
            Email = email,
            Telefone = string.IsNullOrWhiteSpace(telefone) ? null : telefone.Trim(),
            CadastradoEm = DateTime.UtcNow
        };
    }
}
