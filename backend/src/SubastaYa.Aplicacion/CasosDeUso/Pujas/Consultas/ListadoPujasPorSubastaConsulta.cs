namespace SubastaYa.Aplicacion.CasosDeUso.Pujas.Consultas;

public class ListadoPujasPorSubastaConsulta
{
    public int SubastaId { get; set; }

    public ListadoPujasPorSubastaConsulta(int subastaId)
    {
        SubastaId = subastaId;
    }
}
