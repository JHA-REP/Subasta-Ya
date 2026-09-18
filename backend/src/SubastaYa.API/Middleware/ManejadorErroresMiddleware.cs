using System.Net;
using System.Text.Json;
using SubastaYa.Dominio.Excepciones;

namespace SubastaYa.API.Middleware;

/// <summary>
/// Middleware de manejo global de errores.
/// Convierte excepciones de dominio en respuestas HTTP estandarizadas.
/// </summary>
public class ManejadorErroresMiddleware
{
    private readonly RequestDelegate _siguiente;
    private readonly ILogger<ManejadorErroresMiddleware> _registro;

    public ManejadorErroresMiddleware(RequestDelegate siguiente, ILogger<ManejadorErroresMiddleware> registro)
    {
        _siguiente = siguiente;
        _registro = registro;
    }

    public async Task InvokeAsync(HttpContext contexto)
    {
        try
        {
            await _siguiente(contexto);
        }
        catch (Exception excepcion)
        {
            _registro.LogError(excepcion, "Error no controlado: {Mensaje}", excepcion.Message);
            await RespuestaErrorAsync(contexto, excepcion);
        }
    }

    private static async Task RespuestaErrorAsync(HttpContext contexto, Exception excepcion)
    {
        var codigoEstado = excepcion switch
        {
            ExcepcionNoEncontrado => HttpStatusCode.NotFound,
            ExcepcionValidacion => HttpStatusCode.BadRequest,
            ExcepcionConcurrencia => HttpStatusCode.Conflict,
            ExcepcionDominio => HttpStatusCode.UnprocessableEntity,
            _ => HttpStatusCode.InternalServerError
        };

        var respuesta = new
        {
            estado = (int)codigoEstado,
            mensaje = excepcion.Message,
            errores = excepcion is ExcepcionValidacion validacion
                ? validacion.Errores
                : null
        };

        contexto.Response.ContentType = "application/json";
        contexto.Response.StatusCode = (int)codigoEstado;

        var opciones = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        await contexto.Response.WriteAsync(JsonSerializer.Serialize(respuesta, opciones));
    }
}
