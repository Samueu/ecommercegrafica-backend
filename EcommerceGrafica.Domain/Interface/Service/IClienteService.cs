using EcommerceGrafica.Domain.Model;

namespace EcommerceGrafica.Domain.Interface.Service
{
    public interface IClienteService
    {
        Task<IEnumerable<ClienteModel>> ListarTodos();
        Task<ClienteModel?> ObterPorId(int id);
        Task<ClienteModel> RegistrarCliente(ClienteModel cliente);
    }
}
