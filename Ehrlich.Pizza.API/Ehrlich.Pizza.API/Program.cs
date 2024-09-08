using Ehrlich.Pizza.API.Models;
using Ehrlich.Pizza.API.Providers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Pizza Place API",
        Version = "v1",
        Description = "REST API for Pizza Place's Database"
    });
    c.CustomSchemaIds(type => type.FullName);
});
builder.Services.AddDbContext<PizzaPlaceDbContext>();
builder.Services.AddApiVersioning();
builder.Services.AddScoped<IOrdersProvider, OrdersProvider>()
    .AddScoped<IPizzasProvider, PizzasProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
