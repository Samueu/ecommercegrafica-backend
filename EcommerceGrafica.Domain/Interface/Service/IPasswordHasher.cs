namespace EcommerceGrafica.Domain.Interface.Service
{
    public interface IPasswordHasher
    {
        string Hash(string senhaEmClaro);
        bool Verify(string senhaEmClaro, string hashArmazenado);
        bool NeedsRehash(string hashArmazenado);
    }
}
