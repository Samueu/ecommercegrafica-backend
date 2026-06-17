using System.Text;
using System.Threading.RateLimiting;
using ecommercegrafica.Auth;
using ecommercegrafica.Middleware;
using EcommerceGrafica.Domain.Settings;
using EcommerceGrafica.Repository.Data;
using EcommerceGrafica.Setup;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "E-commerce Gráfica API", Version = "v1" });
});

// ---------------------------------------------------------------------------
// Bind das settings
// ---------------------------------------------------------------------------
builder.Services
    .Configure<ConnectionStrings>(builder.Configuration.GetSection("ConnectionStrings"))
    .Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"))
    .Configure<AuthCookieSettings>(builder.Configuration.GetSection("AuthCookie"))
    .Configure<CorsSettings>(builder.Configuration.GetSection("Cors"));

var jwt = builder.Configuration.GetSection("Jwt").Get<JwtSettings>() ?? new JwtSettings();
var cookieCfg = builder.Configuration.GetSection("AuthCookie").Get<AuthCookieSettings>() ?? new AuthCookieSettings();
var corsCfg = builder.Configuration.GetSection("Cors").Get<CorsSettings>() ?? new CorsSettings();

if (string.IsNullOrWhiteSpace(jwt.SigningKey) || jwt.SigningKey.Length < 32)
{
    throw new InvalidOperationException(
        "Configuração obrigatória ausente: Jwt:SigningKey (mínimo 32 caracteres). " +
        "Defina via variável de ambiente em produção.");
}

// Suporte adicional: aceita também uma variável CSV (CorsAllowedOrigins=https://a,https://b),
// que é mais simples de configurar em painéis de hospedagem (Render etc.) do que
// múltiplas chaves indexadas no formato Cors__AllowedOrigins__0, __1…
var csvOrigins = builder.Configuration["CorsAllowedOrigins"];
if (!string.IsNullOrWhiteSpace(csvOrigins))
{
    corsCfg.AllowedOrigins = csvOrigins
        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}

// Normaliza: remove barras finais (origin do browser nunca tem '/' no final).
corsCfg.AllowedOrigins = corsCfg.AllowedOrigins
    .Select(o => o.TrimEnd('/'))
    .Where(o => !string.IsNullOrWhiteSpace(o))
    .ToArray();

Console.WriteLine($"[CORS] AllowedOrigins efetivas: [{string.Join(", ", corsCfg.AllowedOrigins)}]");

// ---------------------------------------------------------------------------
// CORS com credenciais — exige lista explícita de origens (não pode usar '*')
// ---------------------------------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontends", policy =>
    {
        if (corsCfg.AllowedOrigins.Length == 0)
        {
            // Fallback dev: localhost em portas comuns do Next.
            Console.WriteLine(
                "[CORS] Nenhuma origem configurada. Usando fallback de dev (localhost:3000). " +
                "Em produção, defina Cors__AllowedOrigins__0 ou CorsAllowedOrigins.");
            policy.WithOrigins(
                "http://localhost:3000",
                "http://127.0.0.1:3000",
                "https://localhost:3000");
        }
        else
        {
            policy.WithOrigins(corsCfg.AllowedOrigins);
        }

        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ---------------------------------------------------------------------------
// Autenticação JWT lendo o token do header OU do cookie httpOnly
// ---------------------------------------------------------------------------
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.SaveToken = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),
            ClockSkew = TimeSpan.FromSeconds(30),
            NameClaimType = "sub",
            RoleClaimType = "role"
        };

        options.Events = new JwtBearerEvents
        {
            // Permite que o JWT venha pelo cookie httpOnly (estratégia "cookie-as-bearer").
            OnMessageReceived = context =>
            {
                if (string.IsNullOrEmpty(context.Token) &&
                    context.Request.Cookies.TryGetValue(cookieCfg.AccessTokenName, out var fromCookie))
                {
                    context.Token = fromCookie;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// ---------------------------------------------------------------------------
// Rate limiting (proteção contra força bruta nos endpoints de auth)
// ---------------------------------------------------------------------------
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("auth", httpContext =>
    {
        // Particiona por IP + path para evitar que um IP malicioso bloqueie outros.
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: $"{ip}:{httpContext.Request.Path}",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            });
    });
});

// ---------------------------------------------------------------------------
// ForwardedHeaders: necessário quando o app roda atrás de um reverse proxy
// (Render, IIS, Nginx). Sem isso, Request.IsHttps fica false e o IP de origem
// nos logs/auditoria é o IP interno do proxy. Aceitamos qualquer proxy porque
// o tráfego só chega ao container pelo proxy interno da plataforma.
// ---------------------------------------------------------------------------
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// ---------------------------------------------------------------------------
// Demais serviços (DI da aplicação)
// ---------------------------------------------------------------------------
builder.Services.IocConfiguration(builder.Configuration);
builder.Services.AddSingleton<AuthCookieWriter>();

var app = builder.Build();

// Precisa rodar ANTES de qualquer middleware que leia esquema/IP
// (auth, cors, rate limit, etc).
app.UseForwardedHeaders();

await DatabaseBootstrapper.EnsureSchemaAsync(app.Services);

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "E-commerce Gráfica API v1");
    options.RoutePrefix = "swagger";
});

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.UseCors("AllowFrontends");

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
