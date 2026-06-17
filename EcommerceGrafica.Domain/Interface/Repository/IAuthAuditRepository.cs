using EcommerceGrafica.Domain.Model;

namespace EcommerceGrafica.Domain.Interface.Repository
{
    public interface IAuthAuditRepository
    {
        Task RegisterEvento(AuthAuditModel evento);
    }
}
