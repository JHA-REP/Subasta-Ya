namespace SubastaYa.Aplicacion.CasosDeUso.Pujas.ListarPujasPorUsuario;

/// <summary>
/// Consulta para obtener las subastas en las que participó un postor.
/// </summary>
public class ListadoPujasPorUsuarioConsulta
{
    public int UsuarioId { get; set; }

    public ListadoPujasPorUsuarioConsulta(int usuarioId)
    {
        UsuarioId = usuarioId;
    }
}
