using SaleGz.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace SaleGz.Domain.Entities;

// ── COMPRAS ──────────────────────────────────────────
public class Compra
{
    public int CompraId { get; set; }
    public int? ClienteId { get; set; }
    public int UsuarioId { get; set; }
    public decimal Total { get; set; }
    public DateTime Fecha { get; set; }
    public string TipoPago { get; set; } = string.Empty;
    public EstadoCompra Estado { get; set; }

    // Navegación
    public Cliente? Cliente { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public ICollection<CompraDetalle> Detalles { get; set; } = [];
}

public class CompraDetalle
{
    public int CompraDetalleId { get; set; }
    public int CompraId { get; set; }
    public int ProductoId { get; set; }
    public decimal Cantidad { get; set; }
    public decimal SubTotal { get; set; }

    // Navegación
    public Compra Compra { get; set; } = null!;
    public Producto Producto { get; set; } = null!;
}

public class CuentaPorPagarDetalle
{

    public int CuentaPorPagarDetalleId { get; set; }
    public int CompraId { get; set; }
    public int UsuarioId { get; set; }
    public decimal Monto { get; set; }
    public DateTime Fecha { get; set; }

    // Navegación
    public Compra Compra { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
}

// ── COTIZACIONES ──────────────────────────────────────
public class Cotizacion
{
    public int CotizacionId { get; set; }
    public int? ClienteId { get; set; }
    public int UsuarioId { get; set; }
    public decimal Total { get; set; }
    public decimal ITBIS { get; set; }
    public DateTime Fecha { get; set; }

    // Navegación
    public Cliente? Cliente { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public ICollection<CotizacionDetalle> Detalles { get; set; } = [];
}

public class CotizacionDetalle
{
    public int CotizacionDetalleId { get; set; }
    public int CotizacionId { get; set; }
    public int ProductoId { get; set; }
    public decimal Cantidad { get; set; }
    public decimal SubTotal { get; set; }
    public decimal ITBIS { get; set; }

    // Navegación
    public Cotizacion Cotizacion { get; set; } = null!;
    public Producto Producto { get; set; } = null!;
}

// ── GASTOS ────────────────────────────────────────────
public class Gasto
{
    [Key]
    public int GastosId { get; set; }
    public int UsuarioId { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }

    // Navegación
    public Usuario Usuario { get; set; } = null!;
    public ICollection<GastoDetalle> Detalles { get; set; } = [];
}

public class ConsumoGasto
{
    [Key]
    public int ConsumosGastosId { get; set; }
    public string Descripcion { get; set; } = string.Empty;
}

public class GastoDetalle
{
    [Key]
    public int GastosDetalleId { get; set; }
    public int GastosId { get; set; }
    public int ConsumosGastosId { get; set; }
    public decimal Monto { get; set; }

    // Navegación
    public Gasto Gasto { get; set; } = null!;
    public ConsumoGasto ConsumoGasto { get; set; } = null!;
}

// ── CUADRE DE CAJA ────────────────────────────────────
public class Cuadre
{
    [Key]
    public int CuadreId { get; set; }
    public int EmpresaId { get; set; }
    public int UsuarioId { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }

    // Denominaciones RD$
    public decimal P1 { get; set; }
    public decimal P5 { get; set; }
    public decimal P10 { get; set; }
    public decimal P20 { get; set; }
    public decimal P25 { get; set; }
    public decimal P50 { get; set; }
    public decimal P100 { get; set; }
    public decimal P200 { get; set; }
    public decimal P500 { get; set; }
    public decimal P1000 { get; set; }
    public decimal P2000 { get; set; }

    // Otros medios de pago
    public decimal TotalCheque { get; set; }
    public decimal TotalTarjeta { get; set; }
    public decimal TotalTransaccion { get; set; }
    public int CantidadCheque { get; set; }
    public int CantidadTarjeta { get; set; }
    public int CantidadTransaccion { get; set; }

    // Navegación
    public Usuario Usuario { get; set; } = null!;


    public decimal FondoInicial { get; set; }
    public decimal VentasSistema { get; set; }
    public decimal Diferencia { get; set; }
    public string? HoraApertura { get; set; }
    public string? HoraCierre { get; set; }
    public string? Notas { get; set; }
    public int EstadoCuadre { get; set; }  // 0=Pendiente, 1=Aprobado, 2=ConDiferencia



}

// ── CONTROLES ─────────────────────────────────────────
public class Control
{
    [Key]
    public int ControlesId { get; set; }
    public int EmpresaId { get; set; }
    public int LimiteDiaPago { get; set; }
    public TamanoFactura TamanoFactura { get; set; }
    public decimal ITBS { get; set; }
    public int LimiteConteoFiscal { get; set; }
    public int LimiteConteoConsumo { get; set; }
    public ItbsCalculo ItbsCalculo { get; set; }
    public bool MostrarTelefonoCliente { get; set; }
    public bool MostrarDireccionCliente { get; set; }
    public int NumOrden { get; set; }
    public int TipoOrden { get; set; }
    public int LimiteConteoRegimenEspecial { get; set; }
    public int LimiteConteoGubernamental { get; set; }
    public DateTime? FechaLimiteFiscal { get; set; }
    public DateTime? FechaLimiteConsumo { get; set; }
    public DateTime? FechaLimiteGubernamental { get; set; }
    public DateTime? FechaLimiteRegimenEspecial { get; set; }
}

public class UsuarioPermiso
{
    public int UsuarioPermisoId { get; set; }
    public int UsuarioId { get; set; }

    // Facturas
    public bool VerFacturas { get; set; } = true;
    public bool CrearFacturas { get; set; } = true;
    public bool CancelarFacturas { get; set; } = false;

    // Cotizaciones
    public bool VerCotizaciones { get; set; } = true;
    public bool CrearCotizaciones { get; set; } = true;

    // Compras
    public bool VerCompras { get; set; } = false;
    public bool CrearCompras { get; set; } = false;

    // Gastos
    public bool VerGastos { get; set; } = false;
    public bool CrearGastos { get; set; } = false;

    // Cuadres
    public bool VerCuadres { get; set; } = true;
    public bool CrearCuadres { get; set; } = true;

    // Clientes & Productos
    public bool VerClientes { get; set; } = true;
    public bool CrearClientes { get; set; } = true;
    public bool VerProductos { get; set; } = true;

    // Reportes
    public bool VerReportes { get; set; } = false;

    // Inventario
    public bool VerInventario { get; set; } = false;
    public bool CrearInventario { get; set; } = false;

    // Navegación
    public virtual Usuario Usuario { get; set; } = null!;
}

// ── INVENTARIO ────────────────────────────────
public class MovimientoInventario
{
    [Key]
    public int MovimientoId { get; set; }
    public int ProductoId { get; set; }
    public int Tipo { get; set; }  // 0=Entrada, 1=Salida, 2=Ajuste
    public decimal Cantidad { get; set; }
    public string? Referencia { get; set; }  // "Factura #5", "Compra #2"
    public int UsuarioId { get; set; }
    public DateTime Fecha { get; set; }
    public string? Nota { get; set; }

    // Navegación
    public Producto Producto { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;

    public decimal StockAnterior { get; set; }  
    public decimal StockActual { get; set; }

}
