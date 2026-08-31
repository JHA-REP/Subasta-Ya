using SubastaYa.Aplicacion.DTOs;

namespace SubastaYa.Aplicacion.Interfaces;

/// <summary>
/// Contrato del servicio de usuarios.
/// </summary>
public interface IServicioUsuarios
{
    Task<IEnumerable<UsuarioDto>> ListadoAsync();
    Task<UsuarioDto> DetallePorIdAsync(int id);
}
