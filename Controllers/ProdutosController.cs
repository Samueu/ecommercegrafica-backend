using EcommerceGrafica.Domain.Enums;
using EcommerceGrafica.Domain.Interface.Service;
using EcommerceGrafica.Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ecommercegrafica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController(IProdutoService produtoService, IStorageService storageService) : ControllerBase
    {
        private readonly IProdutoService _produtoService = produtoService;
        private readonly IStorageService _storageService = storageService;

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
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> Criar([FromForm] CriarProdutoRequest request, CancellationToken cancellationToken)
        {
            string? imagemUrl = null;
            if (request.Imagem is { Length: > 0 })
            {
                imagemUrl = await _storageService.UploadProdutoImagemAsync(request.Imagem, cancellationToken);
            }
            else if (!string.IsNullOrWhiteSpace(request.ImagemUrl))
            {
                imagemUrl = request.ImagemUrl;
            }

            var produto = new ProdutoModel
            {
                Nome = request.Nome,
                Descricao = request.Descricao ?? string.Empty,
                Preco = request.Preco,
                Moeda = string.IsNullOrWhiteSpace(request.Moeda) ? "BRL" : request.Moeda,
                Tipo = request.Tipo,
                ImagemUrl = imagemUrl
            };

            var registrado = await _produtoService.RegistrarProduto(produto);
            return CreatedAtAction(nameof(ObterPorId), new { id = registrado.Id }, registrado);
        }
    }

    public sealed class CriarProdutoRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public decimal Preco { get; set; }
        public TipoProduto Tipo { get; set; }
        public string? Moeda { get; set; }
        public IFormFile? Imagem { get; set; }

        // Fallback opcional: aceita URL pronta caso o cliente ainda envie assim
        // (mantém compatibilidade com quem já usa o endpoint sem arquivo).
        public string? ImagemUrl { get; set; }
    }
}
