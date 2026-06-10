using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SaleGz.Application.Common.Interfaces;
using SaleGz.Domain.Entities;

namespace SaleGz.Infrastructure.Persistence;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Empresa> Empresas => Set<Empresa>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Control> Controles => Set<Control>();

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Producto> Productos => Set<Producto>();

    public DbSet<NotaCredito> NotaCreditos { get; set; }
    public DbSet<NotaCreditoDetalle> NotaCreditoDetalles { get; set; }

    public DbSet<Factura> Facturas => Set<Factura>();
    public DbSet<FacturaDetalle> FacturaDetalles => Set<FacturaDetalle>();
    public DbSet<CuentaPorCobrarDetalle> CuentasPorCobrar => Set<CuentaPorCobrarDetalle>();
    public DatabaseFacade Database => base.Database;

    public DbSet<Fiscal> Fiscals => Set<Fiscal>();
    public DbSet<Consumo> Consumos => Set<Consumo>();
    public DbSet<RegimenEspecial> RegimenesEspeciales => Set<RegimenEspecial>();
    public DbSet<Gubernamental> Gubernamentales => Set<Gubernamental>();

    public DbSet<Compra> Compras => Set<Compra>();
    public DbSet<CompraDetalle> CompraDetalles => Set<CompraDetalle>();
    public DbSet<CuentaPorPagarDetalle> CuentasPorPagar => Set<CuentaPorPagarDetalle>();

    public DbSet<Cotizacion> Cotizaciones => Set<Cotizacion>();
    public DbSet<CotizacionDetalle> CotizacionDetalles => Set<CotizacionDetalle>();

    public DbSet<Gasto> Gastos => Set<Gasto>();
    public DbSet<GastoDetalle> GastoDetalles => Set<GastoDetalle>();
    public DbSet<ConsumoGasto> ConsumosGastos => Set<ConsumoGasto>();

    public DbSet<Cuadre> Cuadres => Set<Cuadre>();
    public DbSet<MovimientoInventario> MovimientosInventario => Set<MovimientoInventario>();
    public DbSet<UsuarioPermiso> UsuarioPermisos { get; set; }

    public DbSet<ComprobanteElectronica> ComprobantesElectronicos => Set<ComprobanteElectronica>();
    public DbSet<LogTransaccionDgii> LogsTransaccionesDgii => Set<LogTransaccionDgii>();



    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ───────── PRODUCTO ─────────
        builder.Entity<Producto>(p =>
        {
            p.Property(x => x.Cantidad).HasColumnType("decimal(18,2)");
            p.Property(x => x.Costo).HasColumnType("decimal(18,2)");
            p.Property(x => x.Precio1).HasColumnType("decimal(18,2)");
            p.Property(x => x.Precio2).HasColumnType("decimal(18,2)");
            p.Property(x => x.Precio3).HasColumnType("decimal(18,2)");
        });

        // ───────── NOTA DE CRÉDITO ─────────
        builder.Entity<NotaCredito>(nc =>
        {
            nc.HasKey(x => x.NotaCreditoId);
            nc.Property(x => x.Total).HasColumnType("decimal(18,2)");
            nc.Property(x => x.Itbis).HasColumnType("decimal(18,2)");
            nc.Property(x => x.EstadoComprobante).HasMaxLength(50);
            nc.Property(x => x.RazonModificacion).HasMaxLength(255);
        });

        builder.Entity<NotaCreditoDetalle>(ncd =>
        {
            ncd.HasOne(x => x.NotaCredito)
                .WithMany()
                .HasForeignKey(x => x.NotaCreditoId);

            ncd.HasOne(x => x.Producto)
                .WithMany()
                .HasForeignKey(x => x.ProductoId);
        });

        // ───────── INVENTARIO ─────────
        builder.Entity<MovimientoInventario>(m =>
        {
            m.ToTable("Inv_Movimientos");
            m.HasKey(x => x.MovimientoId);
            m.Property(x => x.Cantidad).HasColumnType("decimal(18,2)");
            m.Property(x => x.StockAnterior).HasColumnType("decimal(18,2)");
            m.Property(x => x.StockActual).HasColumnType("decimal(18,2)");
            m.Property(x => x.Referencia).HasMaxLength(100);
            m.Property(x => x.Nota).HasMaxLength(255);
            m.HasOne(x => x.Producto)
                .WithMany()
                .HasForeignKey(x => x.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);
            m.HasOne(x => x.Usuario)
                .WithMany()
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
            m.HasIndex(x => x.ProductoId);
            m.HasIndex(x => x.Fecha);
        });

        // ───────── CUENTAS POR PAGAR ─────────
        builder.Entity<CuentaPorPagarDetalle>(c =>
        {
            c.HasOne(x => x.Compra)
                .WithMany()
                .HasForeignKey(x => x.CompraId)
                .OnDelete(DeleteBehavior.NoAction);

            c.HasOne(x => x.Usuario)
                .WithMany()
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ───────── CUENTAS POR COBRAR ─────────
        builder.Entity<CuentaPorCobrarDetalle>(c =>
        {
            c.HasOne(x => x.Factura)
                .WithMany()
                .HasForeignKey(x => x.FacturaId)
                .OnDelete(DeleteBehavior.NoAction);

            c.HasOne(x => x.Usuario)
                .WithMany()
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ───────── COMPROBANTE ELECTRÓNICO ─────────

        builder.Entity<ComprobanteElectronica>(ce =>
        {
            ce.HasKey(x => x.ComprobanteElectronicoId);
            ce.Property(x => x.NumeroComprobante).HasMaxLength(50).IsRequired();
            ce.Property(x => x.Xml).HasMaxLength(int.MaxValue);
            ce.Property(x => x.FirmaDigital).HasMaxLength(int.MaxValue);
            ce.Property(x => x.TrackId).HasMaxLength(100);
            ce.Property(x => x.RespuestaDgii).HasMaxLength(int.MaxValue);
            ce.Property(x => x.MensajeError).HasMaxLength(500);
            ce.HasIndex(x => x.NumeroComprobante).IsUnique();
            ce.HasIndex(x => x.FacturaId);
            ce.HasIndex(x => x.TrackId);
            ce.HasIndex(x => x.Estado);
        });

        builder.Entity<LogTransaccionDgii>(lt =>
        {
            lt.HasKey(x => x.LogTransaccionDgiiId);
            lt.Property(x => x.Accion).HasMaxLength(100);
            lt.Property(x => x.Peticion).HasMaxLength(int.MaxValue);
            lt.Property(x => x.Respuesta).HasMaxLength(int.MaxValue);
            lt.HasIndex(x => x.ComprobanteElectronicoId);
            lt.HasIndex(x => x.FechaRegistro);
        });

        // ───────── RELACIÓN FACTURA -> COMPROBANTE ─────────
        builder.Entity<Factura>(f =>
        {
            f.HasOne(x => x.ComprobanteElectronica)
                .WithOne(x => x.Factura)
                .HasForeignKey<ComprobanteElectronica>(x => x.FacturaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

    }
}