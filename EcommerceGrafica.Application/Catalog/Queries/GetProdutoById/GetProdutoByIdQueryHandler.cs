using EcommerceGrafica.Application.Catalog.DTOs;
using EcommerceGrafica.Application.Catalog.Mappings;
using EcommerceGrafica.Domain.Catalog.Repositories;
using MediatR;

namespace EcommerceGrafica.Application.Catalog.Queries.GetProdutoById;

public sealed class GetProdutoByIdQueryHandler : IRequestHandler<GetProdutoByIdQuery, ProdutoDto?>
{
    private readonly IProdutoRepository _produtoRepository;

    public GetProdutoByIdQueryHandler(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task<ProdutoDto?> Handle(GetProdutoByIdQuery request, CancellationToken cancellationToken)
    {
        var produto = await _produtoRepository.GetByIdAsync(request.Id, cancellationToken);
        return produto?.ToDto();
    }
}
