namespace SubastaYa.Aplicacion.CasosDeUso.Pujas.ListarPujasPorSubasta;

public class ListadoPujasPorSubastaConsulta
{
    public int SubastaId { get; set; }

    public ListadoPujasPorSubastaConsulta(int subastaId)
    {
        SubastaId = subastaId;
    }
}
