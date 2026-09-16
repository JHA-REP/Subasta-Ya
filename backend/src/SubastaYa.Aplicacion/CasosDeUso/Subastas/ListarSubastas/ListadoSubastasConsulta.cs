using SubastaYa.Dominio.Enumeraciones;

namespace SubastaYa.Aplicacion.CasosDeUso.Subastas.ListarSubastas;

public class ListadoSubastasConsulta
{
    public EstadoSubasta? Estado { get; set; }
    public int? CategoriaId { get; set; }
    public decimal? PrecioMin { get; set; }
    public decimal? PrecioMax { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanoPagina { get; set; } = 10;
    public string? TerminoBusqueda { get; set; }
    public string? CriterioOrden { get; set; }
}

//los que tienen "?" son opcionales, los que no tienen son obligatorios.