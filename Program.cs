using EcommerceGrafica.Application;
using EcommerceGrafica.Infrastructure;
using EcommerceGrafica.Infrastructure.Persistence.Seed;
using ecommercegrafica.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "E-commerce Gráfica API", Version = "v1" });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure();

var app = builder.Build();

await DatabaseSeeder.SeedAsync(app.Services);

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "E-commerce Gráfica API v1");
        options.RoutePrefix = "swagger";
    });

    app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
