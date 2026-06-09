using EcommerceGrafica.Domain.Model;

namespace EcommerceGrafica.Domain.Interface.Repository
{
    public interface IProdutoRepository
    {
        Task<IEnumerable<ProdutoModel>> ListarAtivos();
        Task<ProdutoModel?> GetById(int id);
        Task RegisterProduto(ProdutoModel produto);
    }
}
