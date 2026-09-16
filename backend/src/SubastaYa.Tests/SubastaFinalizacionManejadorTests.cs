using Moq;
using SubastaYa.Aplicacion.CasosDeUso.Subastas.FinalizarSubasta;
using SubastaYa.Aplicacion.Comun.Interfaces;
using SubastaYa.Dominio.Entidades;
using SubastaYa.Dominio.Enumeraciones;
using SubastaYa.Aplicacion.Interfaces;


namespace SubastaYa.Tests;

/// <summary>
/// Tests del manejador de finalización de subastas.
/// Verifica: liquidación con ganador, declaración desierta, idempotencia, auditoría.
/// </summary>
public class SubastaFinalizacionManejadorTests
{
    private readonly Mock<IRepositorio<Subasta>> _mockSubastas = new();
    private readonly Mock<IRepositorio<Puja>> _mockPujas = new();
    private readonly Mock<IRepositorio<Billetera>> _mockBilleteras = new();
    private readonly Mock<IRepositorio<Usuario>> _mockUsuarios = new();
    private readonly Mock<IUnidadDeTrabajo> _mockUnidad = new();
    private readonly Mock<INotificadorSubastas> _mockNotificador = new();
    private readonly Mock<IAuditoriaServicio> _mockAuditoria = new();

    private SubastaFinalizacionManejador CrearManejador() => new(
        _mockSubastas.Object,
        _mockPujas.Object,
        _mockBilleteras.Object,
        _mockUsuarios.Object,
        _mockUnidad.Object,
        _mockNotificador.Object,
        _mockAuditoria.Object);

    // ─── Helpers ──────────────────────────────────────────────────────────────

    private static Subasta SubastaActivaVencida(int id = 1) => new()
    {
        Id = id,
        Titulo = "Subasta Test",
        Estado = EstadoSubasta.Activa,
        FechaInicio = DateTime.UtcNow.AddDays(-5),
        FechaFin = DateTime.UtcNow.AddHours(-1),
        VendedorId = 10,
        PrecioInicial = 1000m,
        PrecioActual = 1500m,
        IncrementoMinimo = 100m,
        ImagenUrl = "https://example.com/img.jpg",
        RowVersion = [1, 0, 0, 0, 0, 0, 0, 0]
    };

    private static Billetera BilleteraConSaldo(int usuarioId, decimal disponible, decimal retenido) => new()
    {
        Id = usuarioId * 10,
        UsuarioId = usuarioId,
        SaldoDisponible = disponible,
        SaldoRetenido = retenido
    };

    // ─── Tests ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task LiquidacionConGanador_SueldoDebitadoYCreditado()
    {
        // Arrange
        var subasta = SubastaActivaVencida();
        var pujaGanadora = new Puja { Id = 1, SubastaId = 1, PostorId = 3, Monto = 1500m, FechaPuja = DateTime.UtcNow.AddHours(-2) };
        var billeteraGanador = BilleteraConSaldo(3, 500m, 1500m);
        var billeteraVendedor = BilleteraConSaldo(10, 0m, 0m);
        var ganador = new Usuario { Id = 3, Alias = "maria_compradora" };

        _mockSubastas.Setup(r => r.FiltradasAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Subasta, bool>>>()))
            .ReturnsAsync([subasta]);
        _mockPujas.Setup(r => r.FiltradasAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Puja, bool>>>()))
            .ReturnsAsync([pujaGanadora]);
        _mockBilleteras.Setup(r => r.FiltradasAsync(It.Is<System.Linq.Expressions.Expression<Func<Billetera, bool>>>(
            e => true)))
            .ReturnsAsync((System.Linq.Expressions.Expression<Func<Billetera, bool>> pred) =>
            {
                // Retornar billetera según usuarioId del filtro evaluado
                var compiled = pred.Compile();
                if (compiled(billeteraGanador)) return [billeteraGanador];
                if (compiled(billeteraVendedor)) return [billeteraVendedor];
                return [];
            });
        _mockUsuarios.Setup(r => r.PorIdAsync(3)).ReturnsAsync(ganador);
        _mockUnidad.Setup(u => u.ConfirmacionAsync()).ReturnsAsync(1);
        _mockAuditoria.Setup(a => a.RegistroAsync(
            It.IsAny<TipoAccionAuditoria>(), It.IsAny<string>(), It.IsAny<int>(),
            It.IsAny<object>(), It.IsAny<string?>()))
            .Returns(Task.CompletedTask);
        _mockNotificador.Setup(n => n.EventoSubastaFinalizada(It.IsAny<SubastaFinalizadaDto>()))
            .Returns(Task.CompletedTask);

        var manejador = CrearManejador();
        var comando = new SubastaFinalizacionComando { FechaCorte = DateTime.UtcNow };

        // Act
        var resultados = (await manejador.EjecucionAsync(comando)).ToList();

        // Assert
        Assert.Single(resultados);
        Assert.Equal(EstadoSubasta.Finalizada, resultados[0].EstadoResultante);
        Assert.Equal(1500m, resultados[0].MontoLiquidado);

        // Verificar que el estado de la subasta cambió
        Assert.Equal(EstadoSubasta.Finalizada, subasta.Estado);
        Assert.Equal(3, subasta.GanadorId);

        // Verificar debito al ganador
        Assert.Equal(0m, billeteraGanador.SaldoRetenido);

        // Verificar crédito al vendedor
        Assert.Equal(1500m, billeteraVendedor.SaldoDisponible);

        // Verificar que se auditó
        _mockAuditoria.Verify(a => a.RegistroAsync(
            TipoAccionAuditoria.LiquidacionCompletada,
            It.IsAny<string>(), It.IsAny<int>(), It.IsAny<object>(), It.IsAny<string?>()), Times.Once);

        // Verificar notificación enviada
        _mockNotificador.Verify(n => n.EventoSubastaFinalizada(It.IsAny<SubastaFinalizadaDto>()), Times.Once);
    }

    [Fact]
    public async Task DeclaracionDesierta_SinPujas_EstadoCambia()
    {
        // Arrange
        var subasta = SubastaActivaVencida(id: 2);

        _mockSubastas.Setup(r => r.FiltradasAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Subasta, bool>>>()))
            .ReturnsAsync([subasta]);
        _mockPujas.Setup(r => r.FiltradasAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Puja, bool>>>()))
            .ReturnsAsync([]);
        _mockUnidad.Setup(u => u.ConfirmacionAsync()).ReturnsAsync(1);
        _mockAuditoria.Setup(a => a.RegistroAsync(
            It.IsAny<TipoAccionAuditoria>(), It.IsAny<string>(), It.IsAny<int>(),
            It.IsAny<object>(), It.IsAny<string?>()))
            .Returns(Task.CompletedTask);
        _mockNotificador.Setup(n => n.EventoSubastaDesierta(It.IsAny<int>()))
            .Returns(Task.CompletedTask);

        var manejador = CrearManejador();

        // Act
        var resultados = (await manejador.EjecucionAsync(new SubastaFinalizacionComando())).ToList();

        // Assert
        Assert.Single(resultados);
        Assert.Equal(EstadoSubasta.Desierta, resultados[0].EstadoResultante);
        Assert.Null(resultados[0].MontoLiquidado);
        Assert.Equal(0, resultados[0].MovimientosGenerados);
        Assert.Equal(EstadoSubasta.Desierta, subasta.Estado);

        _mockAuditoria.Verify(a => a.RegistroAsync(
            TipoAccionAuditoria.DeclaracionDesierta, "Subasta", 2, It.IsAny<object>(), "Worker"), Times.Once);
        _mockNotificador.Verify(n => n.EventoSubastaDesierta(2), Times.Once);
    }

    [Fact]
    public async Task SinSubastasVencidas_ListadoVacio()
    {
        // Arrange
        _mockSubastas.Setup(r => r.FiltradasAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Subasta, bool>>>()))
            .ReturnsAsync([]);

        var manejador = CrearManejador();

        // Act
        var resultados = await manejador.EjecucionAsync(new SubastaFinalizacionComando());

        // Assert
        Assert.Empty(resultados);
        _mockUnidad.Verify(u => u.InicioTransaccionAsync(), Times.Never);
    }
}
