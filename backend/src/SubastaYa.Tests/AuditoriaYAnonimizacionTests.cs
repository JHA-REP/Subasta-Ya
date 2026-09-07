using Moq;
using SubastaYa.Aplicacion.Interfaces;
using SubastaYa.Aplicacion.Mapeos;
using SubastaYa.Dominio.Entidades;
using SubastaYa.Dominio.Enumeraciones;

namespace SubastaYa.Tests;

/// <summary>
/// Tests del servicio de auditoría y de la lógica de anonimización.
/// </summary>
public class AuditoriaYAnonimizacionTests
{
    // ─── Anonimización ────────────────────────────────────────────────────────

    [Theory]
    [InlineData("maria_compradora", "mar***")]
    [InlineData("ab", "ab***")]
    [InlineData("a", "a***")]
    [InlineData("", "???")]
    [InlineData("   ", "???")]
    [InlineData("juan_vendedor", "jua***")]
    public void AliasAnonimizado_RetornaFormatoEsperado(string alias, string esperado)
    {
        // Act
        var resultado = PujaMapeos.AliasAnonimizado(alias);

        // Assert
        Assert.Equal(esperado, resultado);
    }

    [Fact]
    public void PujaMapeoDto_IncluyeLiderAnonimizado()
    {
        // Arrange
        var puja = new Puja
        {
            Id = 1,
            SubastaId = 5,
            PostorId = 3,
            Monto = 8500m,
            FechaPuja = DateTime.UtcNow
        };

        // Act
        var dto = puja.MapeoDto("pedro_postor");

        // Assert
        Assert.Equal("pedro_postor", dto.PostorAlias);
        Assert.Equal("ped***", dto.LiderAnonimizado);
        Assert.Equal(8500m, dto.Monto);
    }

    // ─── Auditoría vía mock ───────────────────────────────────────────────────

    [Fact]
    public async Task AuditoriaServicio_RegistroLlamadoConParametrosCorrectos()
    {
        // Arrange
        var mockAuditoria = new Mock<IAuditoriaServicio>();
        mockAuditoria
            .Setup(a => a.RegistroAsync(
                It.IsAny<TipoAccionAuditoria>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<object>(),
                It.IsAny<string?>()))
            .Returns(Task.CompletedTask);

        // Act — simular registro de extensión Anti-Sniping
        await mockAuditoria.Object.RegistroAsync(
            TipoAccionAuditoria.ExtensionAntiSniping,
            "Subasta",
            42,
            new { descripcion = "Anti-sniping activado" },
            "Sistema");

        // Assert
        mockAuditoria.Verify(a => a.RegistroAsync(
            TipoAccionAuditoria.ExtensionAntiSniping,
            "Subasta",
            42,
            It.IsAny<object>(),
            "Sistema"), Times.Once);
    }

    [Fact]
    public async Task AuditoriaServicio_HistorialConsultado()
    {
        // Arrange
        var registros = new List<RegistroAuditoria>
        {
            new() { Id = 1, Accion = TipoAccionAuditoria.CambioEstadoSubasta, EntidadTipo = "Subasta", EntidadId = 1, FechaRegistro = DateTime.UtcNow },
            new() { Id = 2, Accion = TipoAccionAuditoria.LiquidacionCompletada, EntidadTipo = "Subasta", EntidadId = 1, FechaRegistro = DateTime.UtcNow }
        };

        var mockAuditoria = new Mock<IAuditoriaServicio>();
        mockAuditoria
            .Setup(a => a.HistorialPorEntidadAsync("Subasta", 1))
            .ReturnsAsync(registros);

        // Act
        var historial = (await mockAuditoria.Object.HistorialPorEntidadAsync("Subasta", 1)).ToList();

        // Assert
        Assert.Equal(2, historial.Count);
        Assert.Equal(TipoAccionAuditoria.CambioEstadoSubasta, historial[0].Accion);
        Assert.Equal(TipoAccionAuditoria.LiquidacionCompletada, historial[1].Accion);
    }

    // ─── Reglas de dominio de auditoría ───────────────────────────────────────

    [Fact]
    public void RegistroAuditoria_PropiedadesInicializadas()
    {
        // Arrange & Act
        var registro = new RegistroAuditoria
        {
            Accion = TipoAccionAuditoria.FinalizacionWorker,
            EntidadTipo = "Subasta",
            EntidadId = 7,
            DetalleJson = "{\"cicloFecha\":\"2026-09-07\"}",
            UsuarioOrigen = "Worker",
            FechaRegistro = DateTime.UtcNow
        };

        // Assert
        Assert.Equal(TipoAccionAuditoria.FinalizacionWorker, registro.Accion);
        Assert.Equal("Subasta", registro.EntidadTipo);
        Assert.Equal(7, registro.EntidadId);
        Assert.NotEmpty(registro.DetalleJson);
    }
}
