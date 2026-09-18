namespace Aplicacion.CasosDeUso.Categorias.ListarCategorias;

public class CategoriaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int CantidadSubastas { get; set; }
}
