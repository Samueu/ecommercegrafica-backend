using EcommerceGrafica.Application.Catalog.DTOs;
using EcommerceGrafica.Application.Catalog.Mappings;
using EcommerceGrafica.Domain.Catalog.Repositories;
using MediatR;

namespace EcommerceGrafica.Application.Catalog.Queries.GetProdutos;

public sealed class GetProdutosQueryHandler : IRequestHandler<GetProdutosQuery, IReadOnlyList<ProdutoDto>>
{
    private readonly IProdutoRepository _produtoRepository;

    public GetProdutosQueryHandler(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task<IReadOnlyList<ProdutoDto>> Handle(GetProdutosQuery request, CancellationToken cancellationToken)
    {
        var produtos = await _produtoRepository.ListarAtivosAsync(cancellationToken);
        return produtos.Select(p => p.ToDto()).ToList();
    }
}
