using EcommerceGrafica.Application.Catalog.DTOs;
using MediatR;

namespace EcommerceGrafica.Application.Catalog.Queries.GetProdutos;

public sealed record GetProdutosQuery : IRequest<IReadOnlyList<ProdutoDto>>;
