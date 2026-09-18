namespace Aplicacion.Comun.DTOs;

public class ResultadoPaginadoDto<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int TotalItems { get; set; }
    public int Pagina { get; set; }
    public int TamanoPagina { get; set; }
    public int TotalPaginas { get; set; }
}
