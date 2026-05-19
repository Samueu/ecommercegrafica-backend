namespace EcommerceGrafica.Domain.Shared;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
