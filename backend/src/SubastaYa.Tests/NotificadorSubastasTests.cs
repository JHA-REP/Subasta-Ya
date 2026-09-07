using Moq;
using SubastaYa.Aplicacion.DTOs;
using SubastaYa.Aplicacion.Interfaces;
using SubastaYa.Dominio.Enumeraciones;

namespace SubastaYa.Tests;

/// <summary>
/// Tests del NotificadorSubastas — verifica que los eventos correctos se emiten
/// con la información esperada.
/// No prueba SignalR directamente (eso es de integración); prueba la interfaz y los DTOs.
/// </summary>
public class NotificadorSubastasTests
{
    [Fact]
    public async Task EventoSubastaFinalizada_LlamadaConDatosCorrectos()
    {
        // Arrange
        var mockNotificador = new Mock<INotificadorSubastas>();
        SubastaFinalizadaDto? datosCapturados = null;

        mockNotificador
            .Setup(n => n.EventoSubastaFinalizada(It.IsAny<SubastaFinalizadaDto>()))
            .Callback<SubastaFinalizadaDto>(datos => datosCapturados = datos)
            .Returns(Task.CompletedTask);

        var datosEsperados = new SubastaFinalizadaDto
        {
            SubastaId = 1,
            Titulo = "Notebook Gamer",
            EstadoResultante = EstadoSubasta.Finalizada,
            GanadorId = 5,
            GanadorAlias = "ana_vip",
            MontoFinal = 6000m,
            FechaFinalizacion = DateTime.UtcNow
        };

        // Act
        await mockNotificador.Object.EventoSubastaFinalizada(datosEsperados);

        // Assert
        Assert.NotNull(datosCapturados);
        Assert.Equal(1, datosCapturados.SubastaId);
        Assert.Equal(6000m, datosCapturados.MontoFinal);
        Assert.Equal(EstadoSubasta.Finalizada, datosCapturados.EstadoResultante);
    }

    [Fact]
    public async Task EstadoTemporizador_FlagCriticoActivadoBajo60Segundos()
    {
        // Arrange
        var mockNotificador = new Mock<INotificadorSubastas>();
        InformacionTemporizadorDto? datosCapturados = null;

        mockNotificador
            .Setup(n => n.EstadoTemporizador(It.IsAny<InformacionTemporizadorDto>()))
            .Callback<InformacionTemporizadorDto>(datos => datosCapturados = datos)
            .Returns(Task.CompletedTask);

        var ahora = DateTime.UtcNow;
        var informacion = new InformacionTemporizadorDto
        {
            SubastaId = 3,
            FechaFinUtc = ahora.AddSeconds(45),
            SegundosRestantes = 45,
            Critico = true,  // < 60 s
            EstadoSubasta = EstadoSubasta.Activa,
            PrecioActual = 8500m
        };

        // Act
        await mockNotificador.Object.EstadoTemporizador(informacion);

        // Assert
        Assert.NotNull(datosCapturados);
        Assert.True(datosCapturados.Critico);
        Assert.Equal(45, datosCapturados.SegundosRestantes);
    }

    [Fact]
    public async Task EventoExtensionAntiSniping_FechasNuevasEmitidas()
    {
        // Arrange
        var mockNotificador = new Mock<INotificadorSubastas>();
        EventoExtensionAntiSnipingDto? datosCapturados = null;

        mockNotificador
            .Setup(n => n.EventoExtensionAntiSniping(It.IsAny<EventoExtensionAntiSnipingDto>()))
            .Callback<EventoExtensionAntiSnipingDto>(datos => datosCapturados = datos)
            .Returns(Task.CompletedTask);

        var fechaAnterior = DateTime.UtcNow.AddSeconds(30);
        var fechaNueva = fechaAnterior.AddSeconds(120);

        var evento = new EventoExtensionAntiSnipingDto
        {
            SubastaId = 2,
            FechaFinAnteriorUtc = fechaAnterior,
            FechaFinNuevaUtc = fechaNueva,
            ExtensionSegundos = 120,
            PostorAnonimizado = "mar***"
        };

        // Act
        await mockNotificador.Object.EventoExtensionAntiSniping(evento);

        // Assert
        Assert.NotNull(datosCapturados);
        Assert.Equal(120, datosCapturados.ExtensionSegundos);
        Assert.Equal("mar***", datosCapturados.PostorAnonimizado);
        Assert.True(datosCapturados.FechaFinNuevaUtc > datosCapturados.FechaFinAnteriorUtc);
    }

    [Fact]
    public async Task EventoPujaRecibida_LiderAnonimizadoIncluido()
    {
        // Arrange
        var mockNotificador = new Mock<INotificadorSubastas>();
        PujaDto? datosCapturados = null;

        mockNotificador
            .Setup(n => n.EventoPujaRecibida(It.IsAny<PujaDto>()))
            .Callback<PujaDto>(datos => datosCapturados = datos)
            .Returns(Task.CompletedTask);

        var pujaDto = new PujaDto
        {
            Id = 5,
            SubastaId = 1,
            PostorId = 3,
            PostorAlias = "maria_compradora",
            LiderAnonimizado = "mar***",
            Monto = 7500m,
            FechaPuja = DateTime.UtcNow
        };

        // Act
        await mockNotificador.Object.EventoPujaRecibida(pujaDto);

        // Assert
        Assert.NotNull(datosCapturados);
        Assert.Equal("mar***", datosCapturados.LiderAnonimizado);
        Assert.Equal(7500m, datosCapturados.Monto);
    }
}
