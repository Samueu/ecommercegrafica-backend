using EcommerceGrafica.Domain.Model;

namespace EcommerceGrafica.Domain.Interface.Repository
{
    public interface IProdutoImagemRepository
    {
        Task<IReadOnlyList<ProdutoImagemModel>> ListarPorProduto(int produtoId);
        Task RegistrarImagens(int produtoId, IReadOnlyList<ProdutoImagemModel> imagens);
    }
}
