using SubastaYa.Aplicacion.CasosDeUso.Categorias.ListarCategorias;
using SubastaYa.Dominio.Entidades;

namespace SubastaYa.Aplicacion.Comun.Mapeos;

public static class CategoriaMapeos
{
    public static CategoriaDto MapeoDto(this Categoria entidad)
    {
        return new CategoriaDto
        {
            Id = entidad.Id,
            Nombre = entidad.Nombre,
            Descripcion = entidad.Descripcion,
            CantidadSubastas = entidad.Subastas?.Count ?? 0
        };
    }
}
