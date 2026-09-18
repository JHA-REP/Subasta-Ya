using Aplicacion.Interfaces;
using Dominio.Entidades;
using Dominio.Enumeraciones;

namespace Aplicacion.CasosDeUso.Pujas.ListarPujasPorUsuario;


// Manejador de consulta para listar las subastas en las que participó un usuario.


public class ListadoPujasPorUsuarioManejador : IConsultaManejador<ListadoPujasPorUsuarioConsulta, IEnumerable<ParticipacionSubastaDto>>
{
    private readonly IRepositorio<Puja> _repositorioPujas;
    private readonly IRepositorio<Subasta> _repositorioSubastas;
    private readonly IRepositorio<Usuario> _repositorioUsuarios;
    private readonly IRepositorio<Categoria> _repositorioCategorias;

    public ListadoPujasPorUsuarioManejador(
        IRepositorio<Puja> repositorioPujas,
        IRepositorio<Subasta> repositorioSubastas,
        IRepositorio<Usuario> repositorioUsuarios,
        IRepositorio<Categoria> repositorioCategorias)
    {
        _repositorioPujas = repositorioPujas;
        _repositorioSubastas = repositorioSubastas;
        _repositorioUsuarios = repositorioUsuarios;
        _repositorioCategorias = repositorioCategorias;
    }

    public async Task<IEnumerable<ParticipacionSubastaDto>> EjecucionAsync(ListadoPujasPorUsuarioConsulta consulta)
    {
        // 1. Obtener todas las pujas realizadas por este postor
        var pujasUsuario = await _repositorioPujas.FiltradasAsync(p => p.PostorId == consulta.UsuarioId);
        if (!pujasUsuario.Any())
            return Enumerable.Empty<ParticipacionSubastaDto>();

        // 2. Agrupar por subasta
        var gruposPorSubasta = pujasUsuario
            .GroupBy(p => p.SubastaId)
            .ToList();

        var subastaIds = gruposPorSubasta.Select(g => g.Key).ToList();
        var subastas = await _repositorioSubastas.FiltradasAsync(s => subastaIds.Contains(s.Id));
        var subastasDict = subastas.ToDictionary(s => s.Id);

        var usuariosCache = new Dictionary<int, Usuario>();
        var categoriasCache = new Dictionary<int, Categoria>();

        var resultados = new List<ParticipacionSubastaDto>();

        foreach (var grupo in gruposPorSubasta)
        {
            if (!subastasDict.TryGetValue(grupo.Key, out var subasta))
                continue;

            // Obtener todas las pujas de esta subasta para evaluar liderazgo y conteo
            var todasLasPujas = (await _repositorioPujas.FiltradasAsync(p => p.SubastaId == subasta.Id)).ToList();
            var pujaLider = todasLasPujas.OrderByDescending(p => p.Monto).FirstOrDefault();

            // Cargar Vendedor si no está
            string vendedorAlias = subasta.Vendedor?.Alias ?? string.Empty;
            if (string.IsNullOrEmpty(vendedorAlias) && subasta.VendedorId > 0)
            {
                if (!usuariosCache.TryGetValue(subasta.VendedorId, out var vendedor))
                {
                    vendedor = await _repositorioUsuarios.PorIdAsync(subasta.VendedorId);
                    if (vendedor != null) usuariosCache[subasta.VendedorId] = vendedor;
                }
                vendedorAlias = vendedor?.Alias ?? string.Empty;
            }

            // Cargar Categoria si no está
            string categoriaNombre = subasta.Categoria?.Nombre ?? string.Empty;
            if (string.IsNullOrEmpty(categoriaNombre) && subasta.CategoriaId > 0)
            {
                if (!categoriasCache.TryGetValue(subasta.CategoriaId, out var cat))
                {
                    cat = await _repositorioCategorias.PorIdAsync(subasta.CategoriaId);
                    if (cat != null) categoriasCache[subasta.CategoriaId] = cat;
                }
                categoriaNombre = cat?.Nombre ?? string.Empty;
            }

            // Cargar Ganador si aplica
            string? ganadorAlias = subasta.Ganador?.Alias;
            if (string.IsNullOrEmpty(ganadorAlias) && subasta.GanadorId.HasValue)
            {
                if (!usuariosCache.TryGetValue(subasta.GanadorId.Value, out var ganador))
                {
                    ganador = await _repositorioUsuarios.PorIdAsync(subasta.GanadorId.Value);
                    if (ganador != null) usuariosCache[subasta.GanadorId.Value] = ganador;
                }
                ganadorAlias = ganador?.Alias;
            }

            // Si la subasta está activa, el ganador temporal es el líder actual
            if (subasta.Estado == EstadoSubasta.Activa && pujaLider != null && string.IsNullOrEmpty(ganadorAlias))
            {
                if (!usuariosCache.TryGetValue(pujaLider.PostorId, out var lider))
                {
                    lider = await _repositorioUsuarios.PorIdAsync(pujaLider.PostorId);
                    if (lider != null) usuariosCache[pujaLider.PostorId] = lider;
                }
                ganadorAlias = lider?.Alias;
            }

            bool esGanador = subasta.Estado == EstadoSubasta.Finalizada && subasta.GanadorId == consulta.UsuarioId;
            bool estaLiderando = subasta.Estado == EstadoSubasta.Activa && pujaLider?.PostorId == consulta.UsuarioId;
            bool fueSuperado = subasta.Estado == EstadoSubasta.Activa && pujaLider != null && pujaLider.PostorId != consulta.UsuarioId;

            resultados.Add(new ParticipacionSubastaDto
            {
                SubastaId = subasta.Id,
                Titulo = subasta.Titulo,
                Descripcion = subasta.Descripcion,
                ImagenUrl = subasta.ImagenUrl,
                Categoria = categoriaNombre,
                VendedorAlias = vendedorAlias,
                Estado = subasta.Estado,
                FechaInicio = subasta.FechaInicio,
                FechaFin = subasta.FechaFin,
                PrecioActual = subasta.PrecioActual,
                MiMayorPuja = grupo.Max(p => p.Monto),
                FechaUltimaPuja = grupo.Max(p => p.FechaPuja),
                CantidadMisPujas = grupo.Count(),
                CantidadPujasTotales = todasLasPujas.Count,
                EsGanador = esGanador,
                EstaLiderando = estaLiderando,
                FueSuperado = fueSuperado,
                GanadorAlias = ganadorAlias,
                MontoFinal = subasta.MontoFinal
            });
        }

        return resultados.OrderByDescending(r => r.FechaUltimaPuja);
    }
}
