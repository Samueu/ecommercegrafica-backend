using EcommerceGrafica.Application.Catalog.DTOs;
using EcommerceGrafica.Domain.Catalog.Entities;

namespace EcommerceGrafica.Application.Catalog.Mappings;

public static class ProdutoMapper
{
    public static ProdutoDto ToDto(this Produto produto) =>
        new(
            produto.Id,
            produto.Nome,
            produto.Descricao,
            produto.Preco.Valor,
            produto.Preco.Moeda,
            produto.Tipo,
            produto.Ativo,
            produto.CriadoEm);
}
