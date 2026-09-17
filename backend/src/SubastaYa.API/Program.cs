using System.Text.Json.Serialization;
using SubastaYa.API.Middleware;
using SubastaYa.Aplicacion.CasosDeUso.Billeteras.AcreditarSaldo;
using SubastaYa.Aplicacion.CasosDeUso.Billeteras.ObtenerBilletera;
using SubastaYa.Aplicacion.CasosDeUso.Categorias.ListarCategorias;
using SubastaYa.Aplicacion.CasosDeUso.Categorias.ObtenerCategoriaPorId;
using SubastaYa.Aplicacion.CasosDeUso.Pujas.ListarPujasPorSubasta;
using SubastaYa.Aplicacion.CasosDeUso.Pujas.ListarPujasPorUsuario;
using SubastaYa.Aplicacion.CasosDeUso.Pujas.RegistrarPuja;
using SubastaYa.Aplicacion.CasosDeUso.Subastas.CrearSubasta;
using SubastaYa.Aplicacion.CasosDeUso.Subastas.FinalizarSubasta;
using SubastaYa.Aplicacion.CasosDeUso.Subastas.ListarSubastas;
using SubastaYa.Aplicacion.CasosDeUso.Subastas.ObtenerSubastaPorId;
using SubastaYa.Aplicacion.CasosDeUso.Usuarios.ListarUsuarios;
using SubastaYa.Aplicacion.CasosDeUso.Usuarios.ObtenerUsuarioPorId;
using SubastaYa.Aplicacion.Comun.Interfaces;
using SubastaYa.Infraestructura.Extensiones;
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

// Infraestructura (EF Core, Repositorios, Auditoría, Manejadores críticos)
constructor.Services.ConInfraestructura(constructor.Configuration);

// Manejadores CQRS de Aplicación
constructor.Services.AddScoped<NuevaSubastaManejador>();
constructor.Services.AddScoped<SubastaPorIdManejador>();
constructor.Services.AddScoped<ListadoSubastasManejador>();
constructor.Services.AddScoped<SubastaFinalizacionManejador>();

constructor.Services.AddScoped<PujaRegistroManejador>();
constructor.Services.AddScoped<ListadoPujasPorSubastaManejador>();
constructor.Services.AddScoped<ListadoPujasPorUsuarioManejador>();

constructor.Services.AddScoped<AcreditacionSaldoManejador>();
constructor.Services.AddScoped<BilleteraPorUsuarioManejador>();

constructor.Services.AddScoped<ListadoCategoriasManejador>();
constructor.Services.AddScoped<CategoriaPorIdManejador>();

constructor.Services.AddScoped<ListadoUsuariosManejador>();
constructor.Services.AddScoped<UsuarioPorIdManejador>();

// SignalR — tiempo real
constructor.Services.AddSignalR();
constructor.Services.AddScoped<INotificadorSubastas, NotificadorSubastasSignalR>();

// BackgroundServices:
// 1. ProcesadorSubastas — detecta y liquida subastas vencidas cada 30 s
constructor.Services.AddHostedService<ProcesadorSubastas>();
// 2. TemporizadorSubastas — emite estadoTemporizador cada 10 s
constructor.Services.AddHostedService<TemporizadorSubastas>();

// CORS — preparación para frontend React/Next.js
constructor.Services.AddCors(opciones =>
{
    opciones.AddPolicy("PermitirFrontend", politica =>
    {
        politica.WithOrigins("http://localhost:5173", "http://localhost:3000", "http://localhost:8080", "http://localhost:8081")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // Necesario para SignalR WebSocket
    });
});

var app = constructor.Build();

// === Pipeline de middleware ===

// Manejo global de errores — primero del pipeline
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

// SignalR — Hub de subastas
app.MapHub<SubastaHub>("/hubs/subastas");

app.Run();
