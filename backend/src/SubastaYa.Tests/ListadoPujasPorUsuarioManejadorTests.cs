using System.Linq.Expressions;
using Moq;
using SubastaYa.Aplicacion.CasosDeUso.Pujas.ListarPujasPorUsuario;
using SubastaYa.Aplicacion.Interfaces;
using SubastaYa.Dominio.Entidades;
using SubastaYa.Dominio.Enumeraciones;

namespace SubastaYa.Tests;

/// <summary>
/// Pruebas unitarias para ListadoPujasPorUsuarioManejador (Mis Compras / Pujas).
/// </summary>
public class ListadoPujasPorUsuarioManejadorTests
{
    private readonly Mock<IRepositorio<Puja>> _mockPujas = new();
    private readonly Mock<IRepositorio<Subasta>> _mockSubastas = new();
    private readonly Mock<IRepositorio<Usuario>> _mockUsuarios = new();
    private readonly Mock<IRepositorio<Categoria>> _mockCategorias = new();

    [Fact]
    public async Task EjecucionAsync_SinPujas_RetornaVacio()
    {
        // Arrange
        _mockPujas
            .Setup(p => p.FiltradasAsync(It.IsAny<Expression<Func<Puja, bool>>>()))
            .ReturnsAsync(new List<Puja>());

        var manejador = new ListadoPujasPorUsuarioManejador(
            _mockPujas.Object,
            _mockSubastas.Object,
            _mockUsuarios.Object,
            _mockCategorias.Object);

        // Act
        var resultado = await manejador.EjecucionAsync(new ListadoPujasPorUsuarioConsulta(3));

        // Assert
        Assert.Empty(resultado);
    }

    [Fact]
    public async Task EjecucionAsync_ConPujasActivasYFinalizadas_CalculaCorrectamenteEstadosYLiderazgo()
    {
        // Arrange
        int usuarioId = 3; // María
        int otroUsuarioId = 4; // Carlos

        // Subasta 1: Activa, María está liderando (su puja es 150, la mayor)
        var subasta1 = new Subasta
        {
            Id = 1,
            Titulo = "MacBook Pro M3",
            Estado = EstadoSubasta.Activa,
            PrecioInicial = 100m,
            PrecioActual = 150m,
            VendedorId = 10,
            CategoriaId = 1
        };

        // Subasta 2: Activa, María ofertó 200 pero Carlos la superó con 250
        var subasta2 = new Subasta
        {
            Id = 2,
            Titulo = "iPhone 15",
            Estado = EstadoSubasta.Activa,
            PrecioInicial = 100m,
            PrecioActual = 250m,
            VendedorId = 10,
            CategoriaId = 1
        };

        // Subasta 3: Finalizada, María es la ganadora
        var subasta3 = new Subasta
        {
            Id = 3,
            Titulo = "PlayStation 5",
            Estado = EstadoSubasta.Finalizada,
            GanadorId = usuarioId,
            MontoFinal = 500m,
            PrecioActual = 500m,
            VendedorId = 10,
            CategoriaId = 2
        };

        // Pujas del usuario (María)
        var pujasMaria = new List<Puja>
        {
            new() { Id = 1, SubastaId = 1, PostorId = usuarioId, Monto = 120m, FechaPuja = DateTime.UtcNow.AddHours(-2) },
            new() { Id = 2, SubastaId = 1, PostorId = usuarioId, Monto = 150m, FechaPuja = DateTime.UtcNow.AddHours(-1) },
            new() { Id = 3, SubastaId = 2, PostorId = usuarioId, Monto = 200m, FechaPuja = DateTime.UtcNow.AddHours(-3) },
            new() { Id = 4, SubastaId = 3, PostorId = usuarioId, Monto = 500m, FechaPuja = DateTime.UtcNow.AddDays(-1) },
        };

        // Pujas totales por subasta
        var todasPujasSubasta1 = new List<Puja>
        {
            pujasMaria[0],
            pujasMaria[1]
        };

        var todasPujasSubasta2 = new List<Puja>
        {
            pujasMaria[2],
            new() { Id = 5, SubastaId = 2, PostorId = otroUsuarioId, Monto = 250m, FechaPuja = DateTime.UtcNow.AddHours(-1) }
        };

        var todasPujasSubasta3 = new List<Puja>
        {
            pujasMaria[3]
        };

        _mockPujas
            .Setup(p => p.FiltradasAsync(It.IsAny<Expression<Func<Puja, bool>>>()))
            .ReturnsAsync((Expression<Func<Puja, bool>> pred) =>
            {
                var comp = pred.Compile();
                var todas = new List<Puja> { pujasMaria[0], pujasMaria[1], pujasMaria[2], pujasMaria[3], todasPujasSubasta2[1] };
                return todas.Where(comp).ToList();
            });

        _mockSubastas
            .Setup(s => s.FiltradasAsync(It.IsAny<Expression<Func<Subasta, bool>>>()))
            .ReturnsAsync(new List<Subasta> { subasta1, subasta2, subasta3 });

        _mockUsuarios
            .Setup(u => u.PorIdAsync(10))
            .ReturnsAsync(new Usuario { Id = 10, Alias = "vendedor_oficial" });

        _mockCategorias
            .Setup(c => c.PorIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Categoria { Id = 1, Nombre = "Tecnología" });

        var manejador = new ListadoPujasPorUsuarioManejador(
            _mockPujas.Object,
            _mockSubastas.Object,
            _mockUsuarios.Object,
            _mockCategorias.Object);

        // Act
        var resultado = (await manejador.EjecucionAsync(new ListadoPujasPorUsuarioConsulta(usuarioId))).ToList();

        // Assert
        Assert.Equal(3, resultado.Count);

        // Verificar Subasta 1 (Liderando)
        var item1 = resultado.First(r => r.SubastaId == 1);
        Assert.Equal(150m, item1.MiMayorPuja);
        Assert.Equal(2, item1.CantidadMisPujas);
        Assert.True(item1.EstaLiderando);
        Assert.False(item1.FueSuperado);
        Assert.False(item1.EsGanador);

        // Verificar Subasta 2 (Superada por Carlos)
        var item2 = resultado.First(r => r.SubastaId == 2);
        Assert.Equal(200m, item2.MiMayorPuja);
        Assert.Equal(250m, item2.PrecioActual);
        Assert.False(item2.EstaLiderando);
        Assert.True(item2.FueSuperado);
        Assert.False(item2.EsGanador);

        // Verificar Subasta 3 (Ganada)
        var item3 = resultado.First(r => r.SubastaId == 3);
        Assert.True(item3.EsGanador);
        Assert.False(item3.EstaLiderando);
        Assert.False(item3.FueSuperado);
        Assert.Equal(500m, item3.MontoFinal);
    }
}
