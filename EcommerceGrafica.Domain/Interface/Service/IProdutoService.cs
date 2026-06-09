using EcommerceGrafica.Domain.Model;

namespace EcommerceGrafica.Domain.Interface.Service
{
    public interface IProdutoService
    {
        Task<IEnumerable<ProdutoModel>> ListarAtivos();
        Task<ProdutoModel?> ObterPorId(int id);
        Task<ProdutoModel> RegistrarProduto(ProdutoModel produto);
    }
}
