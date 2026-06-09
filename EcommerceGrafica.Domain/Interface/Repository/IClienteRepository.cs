using EcommerceGrafica.Domain.Model;

namespace EcommerceGrafica.Domain.Interface.Repository
{
    public interface IClienteRepository
    {
        Task<IEnumerable<ClienteModel>> ListarTodos();
        Task<ClienteModel?> GetById(Guid id);
        Task<ClienteModel?> GetByEmail(string email);
        Task RegisterCliente(ClienteModel cliente);
    }
}
