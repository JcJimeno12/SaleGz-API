using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SaleGz.Domain.Entities;

// Alias para evitar conflicto con namespace SaleGz.Application.Empresa
using EmpresaEntity = SaleGz.Domain.Entities.Empresa;

namespace SaleGz.Application.Common.Interfaces;

public interface IAppDbContext
{
    // Empresa & Usuarios
    DbSet<EmpresaEntity> Empresas { get; }
    DbSet<Usuario> Usuarios { get; }
    DbSet<Control> Controles { get; }

    // Clientes & Productos
    DbSet<Cliente> Clientes { get; }
    DbSet<Categoria> Categorias { get; }
    DbSet<Producto> Productos { get; }

    // Facturación
    DbSet<Factura> Facturas { get; }
    DbSet<FacturaDetalle> FacturaDetalles { get; }
    DbSet<CuentaPorCobrarDetalle> CuentasPorCobrar { get; }

    // Comprobantes fiscales
    DbSet<Fiscal> Fiscals { get; }
    DbSet<Consumo> Consumos { get; }
    DbSet<RegimenEspecial> RegimenesEspeciales { get; }
    DbSet<Gubernamental> Gubernamentales { get; }

    // Compras
    DbSet<Compra> Compras { get; }
    DbSet<CompraDetalle> CompraDetalles { get; }
    DbSet<CuentaPorPagarDetalle> CuentasPorPagar { get; }

    // Cotizaciones
    DbSet<Cotizacion> Cotizaciones { get; }
    DbSet<CotizacionDetalle> CotizacionDetalles { get; }

    // Gastos
    DbSet<Gasto> Gastos { get; }
    DbSet<GastoDetalle> GastoDetalles { get; }
    DbSet<ConsumoGasto> ConsumosGastos { get; }

    // Cuadres
    DbSet<Cuadre> Cuadres { get; }

    // Inventario
    DbSet<MovimientoInventario> MovimientosInventario { get; }

   DbSet<UsuarioPermiso> UsuarioPermisos { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    DatabaseFacade Database { get; }


}

public interface ICurrentUserService
{
    int UsuarioId { get; }
    string Nombre { get; }
    int TipoUsuario { get; }
    bool IsAdmin { get; }
}

public interface ITokenService
{
    string GenerateToken(Usuario usuario);
}
