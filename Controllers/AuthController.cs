using System.Security.Claims;
using ecommercegrafica.Auth;
using EcommerceGrafica.Domain.Dtos;
using EcommerceGrafica.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ecommercegrafica.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(IAuthService authService, AuthCookieWriter cookieWriter) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly AuthCookieWriter _cookieWriter = cookieWriter;

        /// <summary>Cadastra um novo usuário (e cliente) e já abre sessão.</summary>
        [HttpPost("register")]
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var ctx = BuildRequestContext();
            var result = await _authService.Register(request, ctx);
            _cookieWriter.WriteTokens(Response, result.AccessToken, result.RefreshToken);
            return Ok(new AuthResponse(result.Usuario));
        }

        /// <summary>Autentica por e-mail/senha e devolve a sessão em cookies httpOnly.</summary>
        [HttpPost("login")]
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var ctx = BuildRequestContext();
            var result = await _authService.Login(request, ctx);
            _cookieWriter.WriteTokens(Response, result.AccessToken, result.RefreshToken);
            return Ok(new AuthResponse(result.Usuario));
        }

        /// <summary>Rotaciona o par access+refresh a partir do cookie de refresh.</summary>
        [HttpPost("refresh")]
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> Refresh()
        {
            var refreshCookie = Request.Cookies[_cookieWriter.RefreshTokenName];
            var ctx = BuildRequestContext();
            var result = await _authService.Refresh(refreshCookie ?? string.Empty, ctx);
            _cookieWriter.WriteTokens(Response, result.AccessToken, result.RefreshToken);
            return Ok(new AuthResponse(result.Usuario));
        }

        /// <summary>Revoga a sessão atual e limpa os cookies.</summary>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshCookie = Request.Cookies[_cookieWriter.RefreshTokenName];
            var ctx = BuildRequestContext();
            await _authService.Logout(refreshCookie, ctx);
            _cookieWriter.Clear(Response);
            return NoContent();
        }

        /// <summary>Devolve o usuário atualmente autenticado (validando o JWT do cookie).</summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            var id = GetUserIdFromClaims();
            if (id is null) return Unauthorized();

            var usuario = await _authService.ObterUsuarioPorId(id.Value);
            if (usuario is null)
            {
                _cookieWriter.Clear(Response);
                return Unauthorized();
            }

            return Ok(usuario);
        }

        /// <summary>Exclui (pseudonimiza) a conta do usuário autenticado. LGPD art. 18, VI.</summary>
        [HttpDelete("me")]
        [Authorize]
        public async Task<IActionResult> ExcluirConta()
        {
            var id = GetUserIdFromClaims();
            if (id is null) return Unauthorized();

            var ctx = BuildRequestContext();
            await _authService.ExcluirConta(id.Value, ctx);
            _cookieWriter.Clear(Response);
            return NoContent();
        }

        private int? GetUserIdFromClaims()
        {
            var raw = User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
            return int.TryParse(raw, out var id) ? id : null;
        }

        private RequestContext BuildRequestContext()
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var ua = Request.Headers.UserAgent.ToString();
            return new RequestContext(ip, string.IsNullOrWhiteSpace(ua) ? null : ua);
        }
    }
}
