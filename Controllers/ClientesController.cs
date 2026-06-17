using EcommerceGrafica.Domain.Interface.Service;
using EcommerceGrafica.Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ecommercegrafica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    public class ClientesController(IClienteService clienteService) : ControllerBase
    {
        private readonly IClienteService _clienteService = clienteService;

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var clientes = await _clienteService.ListarTodos();
            return Ok(clientes);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var cliente = await _clienteService.ObterPorId(id);
            return cliente is null ? NotFound() : Ok(cliente);
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarClienteRequest request)
        {
            var cliente = new ClienteModel
            {
                Nome = request.Nome,
                Email = request.Email,
                Telefone = request.Telefone
            };

            var registrado = await _clienteService.RegistrarCliente(cliente);
            return CreatedAtAction(nameof(ObterPorId), new { id = registrado.Id }, registrado);
        }
    }

    public sealed record CriarClienteRequest(
        string Nome,
        string Email,
        string? Telefone = null);
}
