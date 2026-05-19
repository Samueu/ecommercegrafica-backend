using EcommerceGrafica.Application.Catalog.Commands.CreateProduto;
using EcommerceGrafica.Application.Catalog.Queries.GetProdutoById;
using EcommerceGrafica.Application.Catalog.Queries.GetProdutos;
using EcommerceGrafica.Domain.Catalog.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ecommercegrafica.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProdutosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        var produtos = await _mediator.Send(new GetProdutosQuery(), cancellationToken);
        return Ok(produtos);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken cancellationToken)
    {
        var produto = await _mediator.Send(new GetProdutoByIdQuery(id), cancellationToken);
        return produto is null ? NotFound() : Ok(produto);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarProdutoRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreateProdutoCommand(request.Nome, request.Descricao, request.Preco, request.Tipo),
            cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { erro = result.Error });

        return CreatedAtAction(nameof(ObterPorId), new { id = result.Value!.Id }, result.Value);
    }
}

public sealed record CriarProdutoRequest(
    string Nome,
    string Descricao,
    decimal Preco,
    TipoProduto Tipo);
