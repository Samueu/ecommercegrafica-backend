using EcommerceGrafica.Domain.Model;

namespace EcommerceGrafica.Domain.Interface.Repository
{
    public interface IProdutoRepository
    {
        Task<IEnumerable<ProdutoModel>> ListarAtivos();
        Task<ProdutoModel?> GetById(Guid id);
        Task RegisterProduto(ProdutoModel produto);
    }
}
