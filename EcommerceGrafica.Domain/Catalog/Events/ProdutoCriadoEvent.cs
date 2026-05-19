using EcommerceGrafica.Domain.Shared;

namespace EcommerceGrafica.Domain.Catalog.Events;

public sealed record ProdutoCriadoEvent(Guid ProdutoId, string Nome) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
