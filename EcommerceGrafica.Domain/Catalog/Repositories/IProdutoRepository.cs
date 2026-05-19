using EcommerceGrafica.Domain.Catalog.Entities;
using EcommerceGrafica.Domain.Shared;

namespace EcommerceGrafica.Domain.Catalog.Repositories;

public interface IProdutoRepository : IRepository<Produto>
{
    Task<IReadOnlyList<Produto>> ListarAtivosAsync(CancellationToken cancellationToken = default);
}
