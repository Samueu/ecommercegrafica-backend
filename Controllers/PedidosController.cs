using EcommerceGrafica.Domain.Interface.Service;
using EcommerceGrafica.Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace ecommercegrafica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController(IPedidoService pedidoService) : ControllerBase
    {
        private readonly IPedidoService _pedidoService = pedidoService;

        [HttpGet]
        public async Task<IActionResult> Listar([FromQuery] Guid? clienteId)
        {
            var pedidos = clienteId.HasValue
                ? await _pedidoService.ListarPorCliente(clienteId.Value)
                : await _pedidoService.ListarTodos();

            return Ok(pedidos);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var pedido = await _pedidoService.ObterPorId(id);
            return pedido is null ? NotFound() : Ok(pedido);
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarPedidoRequest request)
        {
            var pedido = new PedidoModel
            {
                ClienteId = request.ClienteId,
                Logradouro = request.Logradouro,
                Numero = request.Numero,
                Bairro = request.Bairro,
                Cidade = request.Cidade,
                Estado = request.Estado,
                Cep = request.Cep,
                Itens = request.Itens?.Select(i => new ItemPedidoModel
                {
                    ProdutoId = i.ProdutoId,
                    Quantidade = i.Quantidade
                }).ToList() ?? new List<ItemPedidoModel>()
            };

            var registrado = await _pedidoService.RegistrarPedido(pedido);
            return CreatedAtAction(nameof(ObterPorId), new { id = registrado.Id }, registrado);
        }
    }

    public sealed record CriarPedidoRequest(
        Guid ClienteId,
        string Logradouro,
        string Numero,
        string Bairro,
        string Cidade,
        string Estado,
        string Cep,
        List<CriarItemPedidoRequest> Itens);

    public sealed record CriarItemPedidoRequest(
        Guid ProdutoId,
        int Quantidade);
}
