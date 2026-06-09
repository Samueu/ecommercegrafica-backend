using EcommerceGrafica.Domain.Model;

namespace EcommerceGrafica.Domain.Interface.Service
{
    public interface IProdutoService
    {
        Task<IEnumerable<ProdutoModel>> ListarAtivos();
        Task<ProdutoModel?> ObterPorId(Guid id);
        Task<ProdutoModel> RegistrarProduto(ProdutoModel produto);
    }
}
