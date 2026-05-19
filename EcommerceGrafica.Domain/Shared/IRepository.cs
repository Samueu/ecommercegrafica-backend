namespace EcommerceGrafica.Domain.Shared;

public interface IRepository<TAggregate> where TAggregate : AggregateRoot
{
    Task<TAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(TAggregate aggregate, CancellationToken cancellationToken = default);
    void Update(TAggregate aggregate);
    void Remove(TAggregate aggregate);
}
