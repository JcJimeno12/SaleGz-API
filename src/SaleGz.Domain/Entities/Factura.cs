using SaleGz.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleGz.Domain.Entities
{
    public class Factura
    {
        public int FacturaId { get; set; }
        public int EmpresaId { get; set; }
        public int? ClienteId { get; set; }
        public int UsuarioId { get; set; }

        public string NombreCliente { get; set; } = string.Empty;

        // ✅ NUEVO
        public string? RncCliente { get; set; }
        public int TipoComprobante { get; set; }

        public string TipoPago { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public decimal ITBS { get; set; }
        public decimal Pagado { get; set; }
        public decimal Exento { get; set; }
        public decimal Gravado { get; set; }
        public DateTime Fecha { get; set; }
        public EstadoFactura Estado { get; set; }
        public int NumOrden { get; set; }
        public bool Tarjeta { get; set; }

        // ✅ NUEVOS - FACTURACIÓN ELECTRÓNICA
        public bool RequiereEcf { get; set; } = true;
        public string? CodigoSeguridad { get; set; }
        public DateTime? FechaEnvioEcf { get; set; }
        public int? ComprobanteElectronicoId { get; set; }

        public Cliente? Cliente { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public ICollection<FacturaDetalle> Detalles { get; set; } = [];
        public ComprobanteElectronica? ComprobanteElectronica { get; set; }

        public TipoFactura TipoFactura { get; set; }
    }

    public class FacturaDetalle
    {
        public int FacturaDetalleId { get; set; }
        public int FacturaId { get; set; }
        public int ProductoId { get; set; }
        public decimal Cantidad { get; set; }
        public decimal SubTotal { get; set; }
        public decimal ITBS { get; set; }
        public decimal Costo { get; set; }
        public decimal Comision { get; set; }

        // Navegación
        public Factura Factura { get; set; } = null!;
        public Producto Producto { get; set; } = null!;

        public string? RncCliente { get; set; }
    }

}
