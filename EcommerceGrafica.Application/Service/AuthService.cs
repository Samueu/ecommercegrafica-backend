using System.Text.RegularExpressions;
using EcommerceGrafica.Application.Security;
using EcommerceGrafica.Domain.Dtos;
using EcommerceGrafica.Domain.Enums;
using EcommerceGrafica.Domain.Exceptions;
using EcommerceGrafica.Domain.Interface.Repository;
using EcommerceGrafica.Domain.Interface.Service;
using EcommerceGrafica.Domain.Model;

namespace EcommerceGrafica.Application.Service
{
    public sealed partial class AuthService(
        IUsuarioRepository usuarioRepository,
        IClienteRepository clienteRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IAuthAuditRepository auditRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator tokenGenerator) : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;
        private readonly IClienteRepository _clienteRepository = clienteRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
        private readonly IAuthAuditRepository _auditRepository = auditRepository;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly IJwtTokenGenerator _tokenGenerator = tokenGenerator;

        public async Task<AuthResult> Register(RegisterRequest request, RequestContext ctx)
        {
            if (request is null)
                throw new DomainException("Dados do cadastro são obrigatórios.");

            if (string.IsNullOrWhiteSpace(request.Nome))
                throw new DomainException("O nome é obrigatório.");

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new DomainException("O e-mail é obrigatório.");

            var email = request.Email.Trim().ToLowerInvariant();
            if (!EmailRegex().IsMatch(email))
                throw new DomainException("O e-mail informado é inválido.");

            if (!request.ConsentimentoLgpd)
                throw new DomainException(
                    "É necessário aceitar o tratamento de dados pessoais conforme a LGPD.");

            PasswordPolicy.Validar(request.Senha);

            var jaExiste = await _usuarioRepository.GetByEmail(email);
            if (jaExiste is not null)
                throw new DomainException("Já existe um usuário cadastrado com esse e-mail.");

            var agora = DateTime.UtcNow;

            // Tenta reaproveitar um cliente já cadastrado pelo mesmo e-mail;
            // se não houver, cria. Isso permite que o front continue chamando POST /api/Clientes
            // como fluxo separado ou que o registro de auth seja a porta única.
            var cliente = await _clienteRepository.GetByEmail(email);
            if (cliente is null)
            {
                cliente = new ClienteModel
                {
                    Nome = request.Nome.Trim(),
                    Email = email,
                    Telefone = string.IsNullOrWhiteSpace(request.Telefone) ? null : request.Telefone.Trim(),
                    CadastradoEm = agora
                };
                await _clienteRepository.RegisterCliente(cliente);
            }

            var usuario = new UsuarioModel
            {
                Email = email,
                SenhaHash = _passwordHasher.Hash(request.Senha),
                Role = UserRole.Customer,
                Ativo = true,
                CriadoEm = agora,
                ClienteId = cliente.Id,
                ConsentimentoEm = agora,
                ConsentimentoVersao = string.IsNullOrWhiteSpace(request.ConsentimentoVersao)
                    ? "v1"
                    : request.ConsentimentoVersao
            };

            await _usuarioRepository.RegisterUsuario(usuario);

            await Audit(AuthEvent.Cadastro, true, usuario.Id, email, ctx);

            var (access, refresh) = await EmitirTokens(usuario, ctx);
            return new AuthResult(UsuarioDto.FromModel(usuario, cliente.Nome), access, refresh);
        }

        public async Task<AuthResult> Login(LoginRequest request, RequestContext ctx)
        {
            var email = (request?.Email ?? string.Empty).Trim().ToLowerInvariant();
            var senha = request?.Senha ?? string.Empty;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
                throw new DomainException("Informe e-mail e senha.");

            var usuario = await _usuarioRepository.GetByEmail(email);
            // Para evitar enumeração de usuários, executamos a verificação mesmo
            // sem usuário (compara contra hash dummy) e devolvemos a mesma mensagem.
            var senhaConfere = usuario is not null
                ? _passwordHasher.Verify(senha, usuario.SenhaHash)
                : _passwordHasher.Verify(senha, DummyHash);

            if (usuario is null || !usuario.Ativo || !senhaConfere)
            {
                await Audit(AuthEvent.LoginFalha, false, usuario?.Id, email, ctx, "credenciais_invalidas");
                throw new DomainException("E-mail ou senha inválidos.");
            }

            usuario.UltimoLoginEm = DateTime.UtcNow;
            await _usuarioRepository.AtualizarUltimoLogin(usuario.Id, usuario.UltimoLoginEm.Value);

            await Audit(AuthEvent.Login, true, usuario.Id, email, ctx);

            var nome = await ObterNomeUsuario(usuario);
            var (access, refresh) = await EmitirTokens(usuario, ctx);
            return new AuthResult(UsuarioDto.FromModel(usuario, nome), access, refresh);
        }

        public async Task<AuthResult> Refresh(string refreshTokenRaw, RequestContext ctx)
        {
            if (string.IsNullOrWhiteSpace(refreshTokenRaw))
                throw new DomainException("Sessão inválida.");

            var hash = _tokenGenerator.HashRefreshToken(refreshTokenRaw);
            var token = await _refreshTokenRepository.GetByHash(hash);

            if (token is null)
            {
                await Audit(AuthEvent.RefreshFalha, false, null, string.Empty, ctx, "token_inexistente");
                throw new DomainException("Sessão inválida.");
            }

            if (!token.EstaAtivo(DateTime.UtcNow))
            {
                // Detecção de reutilização: se o refresh token já foi rotacionado e alguém tenta
                // usá-lo de novo, revogamos toda a sessão do usuário (possível roubo).
                if (token.RevogadoEm is not null)
                {
                    await _refreshTokenRepository.RevogarTodosDoUsuario(
                        token.UsuarioId, "reuso_detectado", DateTime.UtcNow);
                }

                await Audit(AuthEvent.RefreshFalha, false, token.UsuarioId, string.Empty, ctx, "token_inativo");
                throw new DomainException("Sessão expirada. Faça login novamente.");
            }

            var usuario = await _usuarioRepository.GetById(token.UsuarioId);
            if (usuario is null || !usuario.Ativo)
            {
                await Audit(AuthEvent.RefreshFalha, false, token.UsuarioId, string.Empty, ctx, "usuario_inativo");
                throw new DomainException("Sessão inválida.");
            }

            // Rotação: revoga o atual e emite um par novo.
            var novoAccess = _tokenGenerator.CreateAccessToken(usuario);
            var novoRefresh = _tokenGenerator.CreateRefreshToken();

            var novoRefreshModel = new RefreshTokenModel
            {
                UsuarioId = usuario.Id,
                TokenHash = novoRefresh.TokenHash,
                CriadoEm = DateTime.UtcNow,
                ExpiraEm = novoRefresh.ExpiraEm,
                IpOrigem = ctx.IpOrigem,
                UserAgent = ctx.UserAgent
            };

            var novoId = await _refreshTokenRepository.RegisterToken(novoRefreshModel);
            await _refreshTokenRepository.RevogarToken(token.Id, "rotacionado", DateTime.UtcNow, novoId);

            await Audit(AuthEvent.Refresh, true, usuario.Id, usuario.Email, ctx);

            var nome = await ObterNomeUsuario(usuario);
            return new AuthResult(UsuarioDto.FromModel(usuario, nome), novoAccess, novoRefresh);
        }

        public async Task Logout(string? refreshTokenRaw, RequestContext ctx)
        {
            if (string.IsNullOrWhiteSpace(refreshTokenRaw))
                return;

            var hash = _tokenGenerator.HashRefreshToken(refreshTokenRaw);
            var token = await _refreshTokenRepository.GetByHash(hash);
            if (token is null || token.RevogadoEm is not null)
                return;

            await _refreshTokenRepository.RevogarToken(token.Id, "logout", DateTime.UtcNow);
            await Audit(AuthEvent.Logout, true, token.UsuarioId, string.Empty, ctx);
        }

        public async Task<UsuarioDto?> ObterUsuarioPorId(int id)
        {
            var usuario = await _usuarioRepository.GetById(id);
            if (usuario is null || !usuario.Ativo)
                return null;

            var nome = await ObterNomeUsuario(usuario);
            return UsuarioDto.FromModel(usuario, nome);
        }

        public async Task ExcluirConta(int usuarioId, RequestContext ctx)
        {
            var usuario = await _usuarioRepository.GetById(usuarioId);
            if (usuario is null)
                return;

            // Direito ao esquecimento (LGPD art. 18, VI):
            // 1) Revoga todas as sessões abertas;
            // 2) Pseudonimiza/desativa o usuário (mantém o id por integridade fiscal/contábil
            //    dos pedidos passados — base legal: cumprimento de obrigação legal, LGPD art. 7º, II).
            // Caso seja desejada a remoção total mais tarde, basta um job que apague pedidos
            // antigos respeitando o prazo de guarda fiscal.
            await _refreshTokenRepository.RevogarTodosDoUsuario(usuarioId, "exclusao_conta", DateTime.UtcNow);
            await _usuarioRepository.DesativarUsuario(usuarioId);

            await Audit(AuthEvent.ExclusaoConta, true, usuarioId, usuario.Email, ctx);
        }

        private async Task<(AccessToken access, RefreshTokenIssued refresh)> EmitirTokens(
            UsuarioModel usuario, RequestContext ctx)
        {
            var access = _tokenGenerator.CreateAccessToken(usuario);
            var refresh = _tokenGenerator.CreateRefreshToken();

            await _refreshTokenRepository.RegisterToken(new RefreshTokenModel
            {
                UsuarioId = usuario.Id,
                TokenHash = refresh.TokenHash,
                CriadoEm = DateTime.UtcNow,
                ExpiraEm = refresh.ExpiraEm,
                IpOrigem = ctx.IpOrigem,
                UserAgent = ctx.UserAgent
            });

            return (access, refresh);
        }

        private async Task<string?> ObterNomeUsuario(UsuarioModel usuario)
        {
            if (usuario.ClienteId is null) return null;
            var cliente = await _clienteRepository.GetById(usuario.ClienteId.Value);
            return cliente?.Nome;
        }

        private async Task Audit(
            string evento, bool sucesso, int? usuarioId, string email, RequestContext ctx, string? detalhe = null)
        {
            await _auditRepository.RegisterEvento(new AuthAuditModel
            {
                UsuarioId = usuarioId,
                EmailTentativa = email,
                Evento = evento,
                Sucesso = sucesso,
                IpOrigem = ctx.IpOrigem,
                UserAgent = ctx.UserAgent,
                Detalhe = detalhe,
                CriadoEm = DateTime.UtcNow
            });
        }

        // Hash BCrypt fixo (do texto "dummy"), só para gastar tempo equivalente quando
        // o usuário não existe e mitigar timing attacks de enumeração.
        private const string DummyHash =
            "$2a$12$CwTycUXWue0Thq9StjUM0u3v3w8Mx0Q9N9Z9G6m4qf2yU3oV2zQ5C";

        [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
        private static partial Regex EmailRegex();
    }
}
