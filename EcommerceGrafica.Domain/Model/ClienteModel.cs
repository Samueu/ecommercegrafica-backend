namespace EcommerceGrafica.Domain.Model
{
    public class ClienteModel
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Telefone { get; set; }
        public DateTime CadastradoEm { get; set; }
    }
}
