namespace SubastaYa.Aplicacion.CasosDeUso.Billeteras.Consultas;

public class BilleteraPorUsuarioConsulta
{
    public int UsuarioId { get; set; }

    public BilleteraPorUsuarioConsulta(int usuarioId)
    {
        UsuarioId = usuarioId;
    }
}
