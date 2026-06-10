using SaleGz.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleGz.Application.Inventario
{

    public static class MovimientoInventarioMapper
    {
        public static MovimientoInventarioDto Map(MovimientoInventario m)
        {
            return new MovimientoInventarioDto(
                m.MovimientoId,
                m.ProductoId,
                m.Producto.Descripcion,
                m.Tipo,
                m.Tipo switch
                {
                    0 => "Entrada",
                    1 => "Salida",
                    2 => "Ajuste",
                    _ => "Desconocido"
                },
                m.Cantidad,
                m.Referencia,
                m.UsuarioId,
                m.Usuario.Nombre,
                m.Fecha,
                m.Nota,
                m.StockAnterior,
                m.StockActual
            );
        }
    }
}
