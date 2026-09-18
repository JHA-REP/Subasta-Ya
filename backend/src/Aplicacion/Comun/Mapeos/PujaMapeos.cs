using Aplicacion.CasosDeUso.Pujas.ListarPujasPorSubasta;
using Dominio.Entidades;

namespace Aplicacion.Comun.Mapeos;

public static class PujaMapeos
{
    public static PujaDto MapeoDto(this Puja entidad, string? aliasPostor = null)
    {
        var alias = aliasPostor ?? string.Empty;
        return new PujaDto
        {
            Id = entidad.Id,
            SubastaId = entidad.SubastaId,
            PostorId = entidad.PostorId,
            PostorAlias = alias,
            LiderAnonimizado = AliasAnonimizado(alias),
            Monto = entidad.Monto,
            FechaPuja = entidad.FechaPuja
        };
    }

    /// <summary>
    /// Anonimiza un alias para difusión pública: primeros 3 caracteres + "***".
    /// Ej: "maria_compradora" → "mar***".
    /// </summary>
    public static string AliasAnonimizado(string alias)
    {
        if (string.IsNullOrWhiteSpace(alias))
            return "???";

        var prefijo = alias.Length >= 3 ? alias[..3] : alias;
        return $"{prefijo}***";
    }
}
