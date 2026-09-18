using Aplicacion.Comun.Mapeos;
using Aplicacion.Interfaces;
using Dominio.Entidades;


namespace Aplicacion.CasosDeUso.Pujas.ListarPujasPorSubasta;

public class ListadoPujasPorSubastaManejador : IConsultaManejador<ListadoPujasPorSubastaConsulta, IEnumerable<PujaDto>>
{
    private readonly IRepositorio<Puja> _repositorioPujas;
    private readonly IRepositorio<Usuario> _repositorioUsuarios;

    public ListadoPujasPorSubastaManejador(IRepositorio<Puja> repositorioPujas, IRepositorio<Usuario> repositorioUsuarios)
    {
        _repositorioPujas = repositorioPujas;
        _repositorioUsuarios = repositorioUsuarios;
    }

    public async Task<IEnumerable<PujaDto>> EjecucionAsync(ListadoPujasPorSubastaConsulta consulta)
    {
        var pujas = await _repositorioPujas.FiltradasAsync(p => p.SubastaId == consulta.SubastaId);
        var usuarios = await _repositorioUsuarios.TodosAsync();
        
        return pujas
            .OrderByDescending(p => p.Monto)
            .Select(p =>
            {
            var alias = usuarios.FirstOrDefault(u => u.Id == p.PostorId)?.Alias;
                        return p.MapeoDto(alias);
                    });
    }
}
