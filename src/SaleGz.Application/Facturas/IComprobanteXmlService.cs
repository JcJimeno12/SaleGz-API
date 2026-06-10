// SaleGz.Application.Common.Interfaces/IComprobanteXmlService.cs
using SaleGz.Domain.Entities;

namespace SaleGz.Application.Common.Interfaces;

public interface IComprobanteXmlService
{
    Task<string> GenerarXmlAsync(Factura factura, CancellationToken ct);
}