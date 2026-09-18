using System.Linq.Expressions;
using Moq;
using Aplicacion.CasosDeUso.Billeteras.AcreditarSaldo;
using Aplicacion.CasosDeUso.Billeteras.ObtenerBilletera;
using Aplicacion.Interfaces;
using Dominio.Entidades;
using Dominio.Enumeraciones;

namespace Tests;

/// <summary>
/// Pruebas unitarias de manejadores de billetera y trazabilidad del ledger contable.
/// </summary>
public class BilleteraManejadoresTests
{
    private readonly Mock<IRepositorio<Billetera>> _mockBilleteras = new();
    private readonly Mock<IRepositorio<Usuario>> _mockUsuarios = new();
    private readonly Mock<IRepositorio<MovimientoContable>> _mockMovimientos = new();
    private readonly Mock<IUnidadDeTrabajo> _mockUnidad = new();

    [Fact]
    public async Task ObtenerBilleteraPorUsuario_CargaMovimientosOrdenadosPorFechaDescendente()
    {
        // Arrange
        var usuario = new Usuario { Id = 3, Alias = "maria_compradora" };
        var billetera = new Billetera
        {
            Id = 10,
            UsuarioId = 3,
            SaldoDisponible = 50000m,
            SaldoRetenido = 20000m
        };

        var fecha1 = new DateTime(2026, 9, 10, 12, 0, 0, DateTimeKind.Utc);
        var fecha2 = new DateTime(2026, 9, 15, 15, 30, 0, DateTimeKind.Utc);

        var movimientos = new List<MovimientoContable>
        {
            new() { Id = 1, BilleteraId = 10, Tipo = TipoMovimiento.Carga, Monto = 70000m, FechaMovimiento = fecha1, Concepto = "Carga inicial" },
            new() { Id = 2, BilleteraId = 10, Tipo = TipoMovimiento.Retencion, Monto = 20000m, FechaMovimiento = fecha2, Concepto = "Retención subasta #1" }
        };

        _mockBilleteras
            .Setup(b => b.FiltradasAsync(It.IsAny<Expression<Func<Billetera, bool>>>()))
            .ReturnsAsync(new List<Billetera> { billetera });

        _mockUsuarios
            .Setup(u => u.PorIdAsync(3))
            .ReturnsAsync(usuario);

        _mockMovimientos
            .Setup(m => m.FiltradasAsync(It.IsAny<Expression<Func<MovimientoContable, bool>>>()))
            .ReturnsAsync(movimientos);

        var manejador = new BilleteraPorUsuarioManejador(
            _mockBilleteras.Object,
            _mockUsuarios.Object,
            _mockMovimientos.Object);

        // Act
        var resultado = await manejador.EjecucionAsync(new BilleteraPorUsuarioConsulta(3));

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(3, resultado.UsuarioId);
        Assert.Equal("maria_compradora", resultado.UsuarioAlias);
        Assert.Equal(70000m, resultado.Saldo);
        Assert.Equal(20000m, resultado.SaldoRetenido);
        Assert.Equal(50000m, resultado.SaldoDisponible);

        Assert.Equal(2, resultado.Movimientos.Count);
        // Debe estar ordenado de más reciente a más antiguo
        Assert.Equal(2, resultado.Movimientos[0].Id);
        Assert.Equal(TipoMovimiento.Retencion, resultado.Movimientos[0].Tipo);
        Assert.Equal(fecha2, resultado.Movimientos[0].FechaMovimiento);

        Assert.Equal(1, resultado.Movimientos[1].Id);
        Assert.Equal(TipoMovimiento.Carga, resultado.Movimientos[1].Tipo);
        Assert.Equal(fecha1, resultado.Movimientos[1].FechaMovimiento);
    }

    [Fact]
    public async Task AcreditacionSaldo_RegistraMovimientoContableDeTipoCarga()
    {
        // Arrange
        var usuario = new Usuario { Id = 3, Alias = "maria_compradora" };
        var billetera = new Billetera
        {
            Id = 10,
            UsuarioId = 3,
            SaldoDisponible = 10000m,
            SaldoRetenido = 0m
        };

        _mockBilleteras
            .Setup(b => b.FiltradasAsync(It.IsAny<Expression<Func<Billetera, bool>>>()))
            .ReturnsAsync(new List<Billetera> { billetera });

        _mockUsuarios
            .Setup(u => u.PorIdAsync(3))
            .ReturnsAsync(usuario);

        MovimientoContable? movimientoGuardado = null;
        _mockMovimientos
            .Setup(m => m.AltaAsync(It.IsAny<MovimientoContable>()))
            .Callback<MovimientoContable>(mov => movimientoGuardado = mov)
            .Returns(Task.CompletedTask);

        _mockMovimientos
            .Setup(m => m.FiltradasAsync(It.IsAny<Expression<Func<MovimientoContable, bool>>>()))
            .ReturnsAsync(() => movimientoGuardado != null ? new List<MovimientoContable> { movimientoGuardado } : new List<MovimientoContable>());

        _mockUnidad
            .Setup(u => u.ConfirmacionAsync())
            .ReturnsAsync(1);

        var manejador = new AcreditacionSaldoManejador(
            _mockBilleteras.Object,
            _mockUsuarios.Object,
            _mockMovimientos.Object,
            _mockUnidad.Object);

        var comando = new AcreditacionSaldoComando
        {
            UsuarioId = 3,
            Monto = 50000m
        };

        // Act
        var resultado = await manejador.EjecucionAsync(comando);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(60000m, resultado.SaldoDisponible);
        Assert.NotNull(movimientoGuardado);
        Assert.Equal(10, movimientoGuardado.BilleteraId);
        Assert.Equal(50000m, movimientoGuardado.Monto);
        Assert.Equal(TipoMovimiento.Carga, movimientoGuardado.Tipo);
        Assert.Equal("Carga de saldo en cuenta", movimientoGuardado.Concepto);

        _mockUnidad.Verify(u => u.ConfirmacionAsync(), Times.Once);
    }
}
