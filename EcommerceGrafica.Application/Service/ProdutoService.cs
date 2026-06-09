using EcommerceGrafica.Domain.Exceptions;
using EcommerceGrafica.Domain.Interface.Repository;
using EcommerceGrafica.Domain.Interface.Service;
using EcommerceGrafica.Domain.Model;

namespace EcommerceGrafica.Application.Service
{
    public class ProdutoService(IProdutoRepository produtoRepository) : IProdutoService
    {
        private readonly IProdutoRepository _produtoRepository = produtoRepository;

        public async Task<IEnumerable<ProdutoModel>> ListarAtivos()
        {
            return await _produtoRepository.ListarAtivos();
        }

        public async Task<ProdutoModel?> ObterPorId(int id)
        {
            if (id <= 0)
                throw new DomainException("O identificador do produto é obrigatório.");

            return await _produtoRepository.GetById(id);
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
            produto.ImagemUrl = string.IsNullOrWhiteSpace(produto.ImagemUrl) ? null : produto.ImagemUrl.Trim();

            await _produtoRepository.RegisterProduto(produto);
            return produto;
        }
    }
}
