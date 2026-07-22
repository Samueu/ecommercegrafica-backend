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
        [RequestSizeLimit(30 * 1024 * 1024)]
        public async Task<IActionResult> Criar([FromForm] CriarProdutoRequest request, CancellationToken cancellationToken)
        {
            var arquivos = ColetarArquivos(request);
            IReadOnlyList<string> urls = Array.Empty<string>();

            if (arquivos.Count > 0)
            {
                urls = await _storageService.UploadProdutoImagensAsync(arquivos, cancellationToken);
            }
            else if (!string.IsNullOrWhiteSpace(request.ImagemUrl))
            {
                urls = new[] { request.ImagemUrl.Trim() };
            }

            var produto = new ProdutoModel
            {
                Nome = request.Nome,
                Descricao = request.Descricao ?? string.Empty,
                Preco = request.Preco,
                Moeda = string.IsNullOrWhiteSpace(request.Moeda) ? "BRL" : request.Moeda,
                Tipo = request.Tipo,
                Imagens = urls
                    .Select((url, index) => new ProdutoImagemModel { Url = url, Ordem = index })
                    .ToList()
            };

            var registrado = await _produtoService.RegistrarProduto(produto);
            return CreatedAtAction(nameof(ObterPorId), new { id = registrado.Id }, registrado);
        }

        private static List<IFormFile> ColetarArquivos(CriarProdutoRequest request)
        {
            var arquivos = new List<IFormFile>();

            if (request.Imagens is { Count: > 0 })
            {
                arquivos.AddRange(request.Imagens.Where(f => f is { Length: > 0 }));
            }

            if (request.Imagem is { Length: > 0 })
            {
                arquivos.Add(request.Imagem);
            }

            return arquivos;
        }
    }

    public sealed class CriarProdutoRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public decimal Preco { get; set; }
        public TipoProduto Tipo { get; set; }
        public string? Moeda { get; set; }

        /// <summary>Galeria de imagens (até 8 arquivos) enviada pelo admin.</summary>
        public List<IFormFile>? Imagens { get; set; }

        /// <summary>Compatibilidade: upload de uma única imagem.</summary>
        public IFormFile? Imagem { get; set; }

        /// <summary>Fallback: URL pronta (sem upload R2).</summary>
        public string? ImagemUrl { get; set; }
    }
}
