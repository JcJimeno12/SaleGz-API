using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleGz.Application.Facturas.Service
{
    public interface IXmlSignatureService
    {
        Task<string> FirmarXmlAsync(
       string xml,
       CancellationToken cancellationToken = default);
    }
}
