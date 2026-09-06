using SubastaYa.Aplicacion.CasosDeUso.Billeteras.Consultas;
using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Aplicacion.Interfaces;
using SubastaYa.Aplicacion.Mapeos;

namespace SubastaYa.Aplicacion.CasosDeUso.Billeteras.Manejadores;

public class BilleteraPorUsuarioManejador
{
    private readonly IRepositorioBilleteras _repositorioBilleteras;

    public BilleteraPorUsuarioManejador(IRepositorioBilleteras repositorioBilleteras)
    {
        _repositorioBilleteras = repositorioBilleteras;
    }

    public async Task<BilleteraDto?> EjecucionAsync(BilleteraPorUsuarioConsulta consulta)
    {
        var billetera = await _repositorioBilleteras.ObtenerPorUsuarioIdAsync(consulta.UsuarioId);
        return billetera?.MapeoDto();
    }
}
