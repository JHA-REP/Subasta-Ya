using System.Text.Json.Serialization;
using SubastaYa.API.Middleware;
using SubastaYa.Infraestructura.Extensiones;
using SubastaYa.Aplicacion.CasosDeUso.Subastas.Manejadores;
using SubastaYa.Aplicacion.CasosDeUso.Pujas.Manejadores;
using SubastaYa.Aplicacion.CasosDeUso.Billeteras.Manejadores;
using SubastaYa.Aplicacion.CasosDeUso.Categorias.Manejadores;
using SubastaYa.Aplicacion.CasosDeUso.Usuarios.Manejadores;
using SubastaYa.Aplicacion.Interfaces;
using SubastaYa.Infraestructura.TiempoReal;

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

//  registro de manejadores cqrs de aplicacion
constructor.Services.AddScoped<NuevaSubastaManejador>();
constructor.Services.AddScoped<SubastaPorIdManejador>();
constructor.Services.AddScoped<ListadoSubastasManejador>();

constructor.Services.AddScoped<PujaRegistroManejador>();
constructor.Services.AddScoped<ListadoPujasPorSubastaManejador>();

constructor.Services.AddScoped<AcreditacionSaldoManejador>();
constructor.Services.AddScoped<BilleteraPorUsuarioManejador>();

constructor.Services.AddScoped<ListadoCategoriasManejador>();
constructor.Services.AddScoped<CategoriaPorIdManejador>();

constructor.Services.AddScoped<ListadoUsuariosManejador>();
constructor.Services.AddScoped<UsuarioPorIdManejador>();

// SignalR — tiempo real
constructor.Services.AddSignalR();
constructor.Services.AddScoped<INotificadorSubastas, NotificadorSubastasSignalR>();

// Worker de procesamiento de subastas como BackgroundService
constructor.Services.AddHostedService<ProcesadorSubastas>();

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

// SignalR — Hub de subastas en tiempo real
app.MapHub<SubastaHub>("/hubs/subastas");

app.Run();
