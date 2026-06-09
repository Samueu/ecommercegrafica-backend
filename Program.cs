using ecommercegrafica.Middleware;
using EcommerceGrafica.Domain.Settings;
using EcommerceGrafica.Repository.Data;
using EcommerceGrafica.Setup;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "E-commerce Gráfica API", Version = "v1" });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy => policy
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});

builder.Services
    .IocConfiguration(builder.Configuration)
    .Configure<ConnectionStrings>(builder.Configuration.GetSection("ConnectionStrings"));

var app = builder.Build();

await DatabaseBootstrapper.EnsureSchemaAsync(app.Services);

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "E-commerce Gráfica API v1");
    options.RoutePrefix = "swagger";
});

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
