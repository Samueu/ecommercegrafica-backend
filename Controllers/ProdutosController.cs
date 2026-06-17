using EcommerceGrafica.Domain.Enums;
using EcommerceGrafica.Domain.Interface.Service;
using EcommerceGrafica.Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ecommercegrafica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController(IProdutoService produtoService) : ControllerBase
    {
        private readonly IProdutoService _produtoService = produtoService;

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Listar()
        {
            var produtos = await _produtoService.ListarAtivos();
            return Ok(produtos);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var produto = await _produtoService.ObterPorId(id);
            return produto is null ? NotFound() : Ok(produto);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Criar([FromBody] CriarProdutoRequest request)
        {
            var produto = new ProdutoModel
            {
                Nome = request.Nome,
                Descricao = request.Descricao,
                Preco = request.Preco,
                Moeda = string.IsNullOrWhiteSpace(request.Moeda) ? "BRL" : request.Moeda,
                Tipo = request.Tipo,
                ImagemUrl = request.ImagemUrl
            };

            var registrado = await _produtoService.RegistrarProduto(produto);
            return CreatedAtAction(nameof(ObterPorId), new { id = registrado.Id }, registrado);
        }
    }

    public sealed record CriarProdutoRequest(
        string Nome,
        string Descricao,
        decimal Preco,
        TipoProduto Tipo,
        string? Moeda = null,
        string? ImagemUrl = null);
}
