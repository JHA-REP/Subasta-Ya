using System.Text.Json.Serialization;
using SubastaYa.API.Middleware;
using SubastaYa.Infraestructura.Extensiones;

var constructor = WebApplication.CreateBuilder(args);

// === Servicios ===

// Controladores con serialización JSON configurada
constructor.Services.AddControllers()
    .AddJsonOptions(opciones =>
    {
        opciones.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        opciones.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        opciones.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Swagger / OpenAPI
constructor.Services.AddEndpointsApiExplorer();
constructor.Services.AddSwaggerGen();

// Infraestructura (EF Core, Repositorios, Servicios)
constructor.Services.ConInfraestructura(constructor.Configuration);

// CORS — preparación para frontend React
constructor.Services.AddCors(opciones =>
{
    opciones.AddPolicy("PermitirFrontend", politica =>
    {
        politica.WithOrigins("http://localhost:5173", "http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // Necesario para SignalR
    });
});

// Preparación para SignalR (se habilitará en etapas posteriores)
// constructor.Services.AddSignalR();

var app = constructor.Build();

// === Pipeline de middleware ===

// Manejo global de errores — debe ser el primero del pipeline
app.UseMiddleware<ManejadorErroresMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opciones =>
    {
        opciones.SwaggerEndpoint("/swagger/v1/swagger.json", "SubastaYa API v1");
        opciones.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors("PermitirFrontend");
app.UseAuthorization();
app.MapControllers();

// Preparación para SignalR (se habilitará en etapas posteriores)
// app.MapHub<SubastaHub>("/hubs/subastas");

app.Run();
