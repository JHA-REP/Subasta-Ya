using SubastaYa.Aplicacion.CasosDeUso.Billeteras.Consultas;
using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Aplicacion.Mapeos;
using SubastaYa.Dominio.Entidades;
using SubastaYa.Dominio.Interfaces;

namespace SubastaYa.Aplicacion.CasosDeUso.Billeteras.Manejadores;

public class BilleteraPorUsuarioManejador
{
    private readonly IRepositorio<Billetera> _repositorioBilleteras;

    public BilleteraPorUsuarioManejador(IRepositorio<Billetera> repositorioBilleteras)
    {
        _repositorioBilleteras = repositorioBilleteras;
    }

    public async Task<BilleteraDto?> EjecucionAsync(BilleteraPorUsuarioConsulta consulta)
    {
        var billeteras = await _repositorioBilleteras.FiltradasAsync(b => b.UsuarioId == consulta.UsuarioId);
        return billeteras.FirstOrDefault()?.MapeoDto();
    }
}
