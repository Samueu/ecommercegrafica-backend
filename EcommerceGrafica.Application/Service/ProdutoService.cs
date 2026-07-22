using EcommerceGrafica.Domain.Exceptions;
using EcommerceGrafica.Domain.Interface.Repository;
using EcommerceGrafica.Domain.Interface.Service;
using EcommerceGrafica.Domain.Model;

namespace EcommerceGrafica.Application.Service
{
    public class ProdutoService(
        IProdutoRepository produtoRepository,
        IProdutoImagemRepository produtoImagemRepository) : IProdutoService
    {
        private readonly IProdutoRepository _produtoRepository = produtoRepository;
        private readonly IProdutoImagemRepository _produtoImagemRepository = produtoImagemRepository;

        public async Task<IEnumerable<ProdutoModel>> ListarAtivos()
        {
            var produtos = (await _produtoRepository.ListarAtivos()).ToList();

            foreach (var produto in produtos)
            {
                await AnexarImagens(produto);
            }

            return produtos;
        }

        public async Task<ProdutoModel?> ObterPorId(int id)
        {
            if (id <= 0)
                throw new DomainException("O identificador do produto é obrigatório.");

            var produto = await _produtoRepository.GetById(id);
            if (produto is null)
                return null;

            await AnexarImagens(produto);
            return produto;
        }

        public async Task<ProdutoModel> RegistrarProduto(ProdutoModel produto)
        {
            if (produto is null)
                throw new DomainException("O produto é obrigatório.");

            if (string.IsNullOrWhiteSpace(produto.Nome))
                throw new DomainException("O nome do produto é obrigatório.");

            if (produto.Preco < 0)
                throw new DomainException("O preço não pode ser negativo.");

            if (string.IsNullOrWhiteSpace(produto.Moeda))
                throw new DomainException("A moeda é obrigatória.");

            produto.Nome = produto.Nome.Trim();
            produto.Descricao = produto.Descricao?.Trim() ?? string.Empty;
            produto.Moeda = produto.Moeda.ToUpperInvariant();
            produto.Ativo = true;
            produto.CriadoEm = DateTime.UtcNow;

            NormalizarImagens(produto);

            await _produtoRepository.RegisterProduto(produto);

            if (produto.Imagens.Count > 0)
            {
                await _produtoImagemRepository.RegistrarImagens(produto.Id, produto.Imagens);
            }

            return produto;
        }

        private async Task AnexarImagens(ProdutoModel produto)
        {
            var imagens = await _produtoImagemRepository.ListarPorProduto(produto.Id);
            produto.Imagens = imagens.ToList();

            if (produto.Imagens.Count > 0)
            {
                produto.ImagemUrl ??= produto.Imagens[0].Url;
            }
            else if (!string.IsNullOrWhiteSpace(produto.ImagemUrl))
            {
                produto.Imagens.Add(new ProdutoImagemModel
                {
                    ProdutoId = produto.Id,
                    Url = produto.ImagemUrl,
                    Ordem = 0
                });
            }
        }

        private static void NormalizarImagens(ProdutoModel produto)
        {
            produto.Imagens = produto.Imagens
                .Where(i => !string.IsNullOrWhiteSpace(i.Url))
                .Select((img, index) => new ProdutoImagemModel
                {
                    Url = img.Url.Trim(),
                    Ordem = img.Ordem >= 0 ? img.Ordem : index
                })
                .OrderBy(i => i.Ordem)
                .ToList();

            if (produto.Imagens.Count > 0)
            {
                produto.ImagemUrl = produto.Imagens[0].Url;
            }
            else
            {
                produto.ImagemUrl = string.IsNullOrWhiteSpace(produto.ImagemUrl)
                    ? null
                    : produto.ImagemUrl.Trim();
            }
        }
    }
}
