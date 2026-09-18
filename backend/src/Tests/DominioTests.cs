using Dominio.Entidades;
using Dominio.Enumeraciones;
using Dominio.Excepciones;
using Dominio.Reglas;

namespace Tests;

/// <summary>
/// Tests del dominio: reglas de finalización, Billetera y Subasta.
/// Verifica comportamientos de negocio sin dependencias externas.
/// </summary>
public class DominioTests
{
    // ─── SubastaFinalizacionReglas ─────────────────────────────────────────────

    [Fact]
    public void ValidacionEstadoParaFinalizacion_EstadoActiva_NoLanzaExcepcion()
    {
        // Act & Assert — no debe lanzar
        SubastaFinalizacionReglas.ValidacionEstadoParaFinalizacion(EstadoSubasta.Activa);
    }

    [Theory]
    [InlineData(EstadoSubasta.Finalizada)]
    [InlineData(EstadoSubasta.Desierta)]
    [InlineData(EstadoSubasta.Cancelada)]
    [InlineData(EstadoSubasta.Pendiente)]
    public void ValidacionEstadoParaFinalizacion_EstadoNoActiva_LanzaExcepcion(EstadoSubasta estado)
    {
        // Act & Assert
        Assert.Throws<ExcepcionValidacion>(() =>
            SubastaFinalizacionReglas.ValidacionEstadoParaFinalizacion(estado));
    }

    [Fact]
    public void ValidacionFechaVencimiento_FechaAlcanzada_NoLanzaExcepcion()
    {
        // Arrange
        var fechaFin = DateTime.UtcNow.AddHours(-1);
        var ahora = DateTime.UtcNow;

        // Act & Assert — no debe lanzar
        SubastaFinalizacionReglas.ValidacionFechaVencimiento(fechaFin, ahora);
    }

    [Fact]
    public void ValidacionFechaVencimiento_FechaFutura_LanzaExcepcion()
    {
        // Arrange
        var fechaFin = DateTime.UtcNow.AddHours(1);
        var ahora = DateTime.UtcNow;

        // Act & Assert
        Assert.Throws<ExcepcionValidacion>(() =>
            SubastaFinalizacionReglas.ValidacionFechaVencimiento(fechaFin, ahora));
    }

    // ─── Billetera ─────────────────────────────────────────────────────────────

    [Fact]
    public void ConfirmacionRetencion_MontoValido_ReduceSaldoRetenido()
    {
        // Arrange
        var billetera = new Billetera { SaldoDisponible = 0m, SaldoRetenido = 1500m };

        // Act
        billetera.ConfirmacionRetencion(1500m);

        // Assert
        Assert.Equal(0m, billetera.SaldoRetenido);
        Assert.Equal(0m, billetera.SaldoDisponible); // No retorna a disponible
    }

    [Fact]
    public void ConfirmacionRetencion_MontoMayorAlRetenido_LanzaExcepcion()
    {
        // Arrange
        var billetera = new Billetera { SaldoDisponible = 0m, SaldoRetenido = 500m };

        // Act & Assert
        Assert.Throws<ExcepcionValidacion>(() => billetera.ConfirmacionRetencion(1000m));
    }

    [Fact]
    public void LiberacionSaldo_MontoValido_RetornaADisponible()
    {
        // Arrange
        var billetera = new Billetera { SaldoDisponible = 1000m, SaldoRetenido = 500m };

        // Act
        billetera.LiberacionSaldo(500m);

        // Assert
        Assert.Equal(0m, billetera.SaldoRetenido);
        Assert.Equal(1500m, billetera.SaldoDisponible);
    }

    // ─── Subasta — Anti-Sniping ────────────────────────────────────────────────

    [Fact]
    public void ExtensionTiempoAntiSniping_UltimoMinuto_ExtiendeDosMinutos()
    {
        // Arrange — subasta con 30 segundos restantes
        var fechaFin = DateTime.UtcNow.AddSeconds(30);
        var subasta = new Subasta
        {
            FechaFin = fechaFin,
            Estado = EstadoSubasta.Activa,
            Titulo = "Test",
            Descripcion = "Test",
            ImagenUrl = "x",
            RowVersion = [1, 0, 0, 0, 0, 0, 0, 0]
        };
        var fechaAnterior = subasta.FechaFin;

        // Act
        subasta.ExtensionTiempoAntiSniping(DateTime.UtcNow);

        // Assert — debe extenderse 2 minutos
        Assert.True(subasta.FechaFin > fechaAnterior);
        var extensionReal = (subasta.FechaFin - fechaAnterior).TotalSeconds;
        Assert.InRange(extensionReal, 119, 121); // ~120 segundos
    }

    [Fact]
    public void ExtensionTiempoAntiSniping_FueraDeLosUltimosSensiunda_NoExtiende()
    {
        // Arrange — subasta con 5 minutos restantes
        var fechaFin = DateTime.UtcNow.AddMinutes(5);
        var subasta = new Subasta
        {
            FechaFin = fechaFin,
            Estado = EstadoSubasta.Activa,
            Titulo = "Test",
            Descripcion = "Test",
            ImagenUrl = "x",
            RowVersion = [1, 0, 0, 0, 0, 0, 0, 0]
        };

        // Act
        subasta.ExtensionTiempoAntiSniping(DateTime.UtcNow);

        // Assert — no debe extenderse
        Assert.Equal(fechaFin, subasta.FechaFin);
    }

    // ─── Subasta — ResultadoFinalizacion ──────────────────────────────────────

    [Fact]
    public void ResultadoFinalizacion_CambiaEstadoYRegistraGanador()
    {
        // Arrange
        var subasta = new Subasta
        {
            Estado = EstadoSubasta.Activa,
            Titulo = "Test", Descripcion = "Test", ImagenUrl = "x",
            RowVersion = [1, 0, 0, 0, 0, 0, 0, 0]
        };

        // Act
        subasta.ResultadoFinalizacion(ganadorId: 5, montoFinal: 4500m);

        // Assert
        Assert.Equal(EstadoSubasta.Finalizada, subasta.Estado);
        Assert.Equal(5, subasta.GanadorId);
        Assert.Equal(4500m, subasta.MontoFinal);
    }

    [Fact]
    public void ResultadoDesierta_CambiaEstadoADesierta()
    {
        // Arrange
        var subasta = new Subasta
        {
            Estado = EstadoSubasta.Activa,
            Titulo = "Test", Descripcion = "Test", ImagenUrl = "x",
            RowVersion = [1, 0, 0, 0, 0, 0, 0, 0]
        };

        // Act
        subasta.ResultadoDesierta();

        // Assert
        Assert.Equal(EstadoSubasta.Desierta, subasta.Estado);
        Assert.Null(subasta.GanadorId);
        Assert.Null(subasta.MontoFinal);
    }
}
