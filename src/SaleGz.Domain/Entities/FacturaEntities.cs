using SaleGz.Domain.Enums;

namespace SaleGz.Domain.Entities;

//public class Factura
//{
//    public int FacturaId { get; set; }
//    public int EmpresaId { get; set; }
//    public int? ClienteId { get; set; }
//    public int UsuarioId { get; set; }

//    public string NombreCliente { get; set; } = string.Empty;

//    // ✅ NUEVO
//    public string? RncCliente { get; set; }
//    public int TipoComprobante { get; set; }

//    public string TipoPago { get; set; } = string.Empty;
//    public decimal Total { get; set; }
//    public decimal ITBS { get; set; }
//    public decimal Pagado { get; set; }
//    public decimal Exento { get; set; }
//    public decimal Gravado { get; set; }
//    public DateTime Fecha { get; set; }
//    public EstadoFactura Estado { get; set; }
//    public int NumOrden { get; set; }
//    public bool Tarjeta { get; set; }

//    public Cliente? Cliente { get; set; }
//    public Usuario Usuario { get; set; } = null!;
//    public ICollection<FacturaDetalle> Detalles { get; set; } = []; 

//    public TipoFactura TipoFactura { get; set; }
//}
//public class FacturaDetalle
//{
//    public int FacturaDetalleId { get; set; }
//    public int FacturaId { get; set; }
//    public int ProductoId { get; set; }
//    public decimal Cantidad { get; set; }
//    public decimal SubTotal { get; set; }
//    public decimal ITBS { get; set; }
//    public decimal Costo { get; set; }
//    public decimal Comision { get; set; }

//    // Navegación
//    public Factura Factura { get; set; } = null!;
//    public Producto Producto { get; set; } = null!;

//    public string? RncCliente { get; set; }
//}

public class CuentaPorCobrarDetalle
{ 
    public int CuentaPorCobrarDetalleId { get; set; }
    public int FacturaId { get; set; }
    public int UsuarioId { get; set; }
    public decimal Cantidad { get; set; }
    public DateTime FechaUpdate { get; set; }

    // Navegación
    public Factura Factura { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
}

public class Fiscal
{
    public int FiscalId { get; set; }
    public int FacturaId { get; set; }
    public int EmpresaId { get; set; }
    public int Conteo { get; set; }

    public Factura Factura { get; set; } = null!;
}

public class Consumo
{
    public int ConsumoId { get; set; }
    public int FacturaId { get; set; }
    public int EmpresaId { get; set; }
    public int Conteo { get; set; }

    public Factura Factura { get; set; } = null!;
}

public class RegimenEspecial
{
    public int RegimenEspecialId { get; set; }
    public int FacturaId { get; set; }
    public int Conteo { get; set; }

    public Factura Factura { get; set; } = null!;
}

public class Gubernamental
{
    public int GubernamentalId { get; set; }
    public int FacturaId { get; set; }
    public int Conteo { get; set; }

    public Factura Factura { get; set; } = null!;
}
