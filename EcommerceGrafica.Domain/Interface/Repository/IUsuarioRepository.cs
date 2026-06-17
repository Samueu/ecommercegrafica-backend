using EcommerceGrafica.Domain.Model;

namespace EcommerceGrafica.Domain.Interface.Repository
{
    public interface IUsuarioRepository
    {
        Task<UsuarioModel?> GetById(int id);
        Task<UsuarioModel?> GetByEmail(string email);
        Task RegisterUsuario(UsuarioModel usuario);
        Task AtualizarUltimoLogin(int id, DateTime quandoUtc);
        Task DesativarUsuario(int id);
    }
}
