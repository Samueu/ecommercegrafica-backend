using EcommerceGrafica.Application.Catalog.DTOs;
using EcommerceGrafica.Application.Common.Models;
using EcommerceGrafica.Domain.Catalog.Enums;
using MediatR;

namespace EcommerceGrafica.Application.Catalog.Commands.CreateProduto;

public sealed record CreateProdutoCommand(
    string Nome,
    string Descricao,
    decimal Preco,
    TipoProduto Tipo) : IRequest<Result<ProdutoDto>>;
