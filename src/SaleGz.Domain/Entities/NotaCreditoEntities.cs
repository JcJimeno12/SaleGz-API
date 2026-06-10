using System;
using System.Collections.Generic;

namespace SaleGz.Domain.Entities;

public class NotaCredito
{
    public int NotaCreditoId { get; set; }
    public int FacturaId { get; set; }
    public decimal Total { get; set; }
    public decimal Itbis { get; set; }
    public string Comprobante { get; set; }
    public string EstadoComprobante { get; set; }
    public int Conteo { get; set; }
    public string RazonModificacion { get; set; }

    // Navigation
    public Factura Factura { get; set; }
}

public class NotaCreditoDetalle
{
    public int NotaCreditoDetalleId { get; set; }
    public int NotaCreditoId { get; set; }
    public int ProductoId { get; set; }
    public decimal Cantidad { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Itbis { get; set; }
    public decimal Comision { get; set; }

    public string Detalles { get; set; }

    // Navigation
    public NotaCredito NotaCredito { get; set; }
    public Producto Producto { get; set; }
}