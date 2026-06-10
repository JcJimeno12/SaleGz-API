using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SaleGz.Application.Facturas.Service
{

    public class XmlSignatureService : IXmlSignatureService
    {
        private readonly DgiiSettings _settings;

        public XmlSignatureService(DgiiSettings settings)
        {
            _settings = settings;
        }

        public async Task<string> FirmarXmlAsync(
            string xml,
            CancellationToken cancellationToken = default)
        {
            var certificado = new X509Certificate2(
                _settings.CertificatePath,
                _settings.CertificatePassword,
                X509KeyStorageFlags.MachineKeySet |
                X509KeyStorageFlags.Exportable);

            var xmlDoc = new XmlDocument
            {
                PreserveWhitespace = true
            };

            xmlDoc.LoadXml(xml);

            var signedXml = new SignedXml(xmlDoc)
            {
                SigningKey = certificado.GetRSAPrivateKey()
            };

            var reference = new Reference("");
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());

            signedXml.AddReference(reference);

            var keyInfo = new KeyInfo();
            keyInfo.AddClause(new KeyInfoX509Data(certificado));

            signedXml.KeyInfo = keyInfo;

            signedXml.ComputeSignature();

            XmlElement firma = signedXml.GetXml();

            xmlDoc.DocumentElement!
                .AppendChild(xmlDoc.ImportNode(firma, true));

            return await Task.FromResult(xmlDoc.OuterXml);
        }
    }
