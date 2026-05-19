using EcommerceGrafica.Domain.Customers.Entities;
using EcommerceGrafica.Domain.Customers.ValueObjects;
using EcommerceGrafica.Domain.Shared;

namespace EcommerceGrafica.Domain.Customers.Repositories;

public interface IClienteRepository : IRepository<Cliente>
{
    Task<Cliente?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
}
