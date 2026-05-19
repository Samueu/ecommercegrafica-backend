using EcommerceGrafica.Application.Catalog.DTOs;
using MediatR;

namespace EcommerceGrafica.Application.Catalog.Queries.GetProdutoById;

public sealed record GetProdutoByIdQuery(Guid Id) : IRequest<ProdutoDto?>;
