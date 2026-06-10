using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SaleGz.Application.Common.Interfaces;
using SaleGz.Domain.Entities;
using SaleGz.Domain.Enums;
using SaleGz.Infrastructure.Persistence;

namespace SaleGz.Infrastructure.Repositories;

public class ComprobanteRepository : IComprobanteRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<ComprobanteRepository> _logger;

    public ComprobanteRepository(AppDbContext context, ILogger<ComprobanteRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ComprobanteElectronica?> ObtenerPorFacturaAsync(int facturaId, CancellationToken ct)
    {
        return await _context.ComprobantesElectronicos
            .FirstOrDefaultAsync(x => x.FacturaId == facturaId, ct);
    }

    public async Task<ComprobanteElectronica?> ObtenerPorNumeroComprobanteAsync(string numeroComprobante, CancellationToken ct)
    {
        return await _context.ComprobantesElectronicos
            .FirstOrDefaultAsync(x => x.NumeroComprobante == numeroComprobante, ct);
    }

    public async Task<ComprobanteElectronica?> ObtenerPorTrackIdAsync(string trackId, CancellationToken ct)
    {
        return await _context.ComprobantesElectronicos
            .FirstOrDefaultAsync(x => x.TrackId == trackId, ct);
    }

    public async Task<List<ComprobanteElectronica>> ObtenerPendientesAsync(CancellationToken ct)
    {
        return await _context.ComprobantesElectronicos
            .Where(x => x.Estado == EstadoComprobante.Pendiente ||
                       x.Estado == EstadoComprobante.Enviado)
            .Where(x => x.FechaCreacion >= DateTime.Now.AddDays(-30))
            .OrderBy(x => x.FechaCreacion)
            .ToListAsync(ct);
    }

    public async Task<List<ComprobanteElectronica>> ObtenerRechazadosAsync(CancellationToken ct)
    {
        return await _context.ComprobantesElectronicos
            .Where(x => x.Estado == EstadoComprobante.Rechazado)
            .Where(x => x.IntentosCuenta < 3)
            .OrderBy(x => x.FechaCreacion)
            .ToListAsync(ct);
    }

    public async Task AgregarAsync(ComprobanteElectronica comprobante, CancellationToken ct)
    {
        _context.ComprobantesElectronicos.Add(comprobante);
        await _context.SaveChangesAsync(ct);
        _logger.LogInformation("Comprobante electrónico creado. FacturaId: {FacturaId}", comprobante.FacturaId);
    }

    public async Task ActualizarAsync(ComprobanteElectronica comprobante, CancellationToken ct)
    {
        _context.ComprobantesElectronicos.Update(comprobante);
        await _context.SaveChangesAsync(ct);
        _logger.LogInformation("Comprobante electrónico actualizado. NumeroComprobante: {NumeroComprobante}",
            comprobante.NumeroComprobante);
    }

    public async Task RegistrarLogAsync(LogTransaccionDgii log, CancellationToken ct)
    {
        _context.LogsTransaccionesDgii.Add(log);
        await _context.SaveChangesAsync(ct);
    }
}