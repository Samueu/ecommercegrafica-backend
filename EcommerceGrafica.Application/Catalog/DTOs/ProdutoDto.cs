using EcommerceGrafica.Domain.Catalog.Enums;

namespace EcommerceGrafica.Application.Catalog.DTOs;

public sealed record ProdutoDto(
    Guid Id,
    string Nome,
    string Descricao,
    decimal Preco,
    string Moeda,
    TipoProduto Tipo,
    bool Ativo,
    DateTime CriadoEm);
