// SaleGz.Application.Common.Interfaces/IComprobanteRepository.cs
using SaleGz.Domain.Entities;

namespace SaleGz.Application.Common.Interfaces;

public interface IComprobanteRepository
{
    Task AgregarAsync(ComprobanteElectronica comprobante, CancellationToken ct);
    Task ActualizarAsync(ComprobanteElectronica comprobante, CancellationToken ct);
}