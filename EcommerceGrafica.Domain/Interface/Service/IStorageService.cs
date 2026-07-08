using Microsoft.AspNetCore.Http;

namespace EcommerceGrafica.Domain.Interface.Service
{
    public interface IStorageService
    {
        Task<string> UploadProdutoImagemAsync(IFormFile arquivo, CancellationToken cancellationToken = default);
    }
}
