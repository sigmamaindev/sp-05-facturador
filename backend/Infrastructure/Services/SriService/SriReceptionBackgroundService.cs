using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Infrastructure.Data;
using Core.Constants;
using Core.Interfaces.Services.IInvoiceService;
using Core.Interfaces.Services.IUtilService;

namespace Infrastructure.Services.SriService;

public class SriReceptionBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<SriReceptionBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
                var sriReceptionService = scope.ServiceProvider.GetRequiredService<ISriReceptionService>();
                var invoiceXmlBuilder = scope.ServiceProvider.GetRequiredService<IInvoiceXmlBuilderService>();
                var electronicSignature = scope.ServiceProvider.GetRequiredService<IElectronicSignatureService>();
                var aesEncryptionService = scope.ServiceProvider.GetRequiredService<IAesEncryptionService>();

                var pendingInvoices = await context.Invoices
                    .Include(i => i.Customer)
                    .Include(i => i.Business).ThenInclude(b => b!.BusinessCertificate)
                    .Include(i => i.Establishment)
                    .Include(i => i.EmissionPoint)
                    .Include(i => i.InvoiceDetails).ThenInclude(d => d.Product)
                    .Include(i => i.InvoiceDetails).ThenInclude(d => d.Warehouse)
                    .Include(i => i.InvoiceDetails).ThenInclude(d => d.Tax)
                    .Where(i =>
                        i.IsElectronic &&
                        (i.Status == InvoiceStatus.SRI_TIMEOUT ||
                         i.Status == InvoiceStatus.SRI_UNAVAILABLE ||
                         i.Status == InvoiceStatus.PENDING))
                    .ToListAsync(stoppingToken);

                foreach (var invoice in pendingInvoices)
                {
                    try
                    {
                        if (invoice.Business?.BusinessCertificate == null)
                        {
                            invoice.Status = InvoiceStatus.ERROR;
                            invoice.SriMessage = "El negocio no tiene un certificado configurado para firmar la factura.";
                            continue;
                        }

                        invoice.AccessKey = string.IsNullOrWhiteSpace(invoice.AccessKey)
                            ? GenerateAccessKey(
                                invoice.InvoiceDate,
                                invoice.ReceiptType,
                                invoice.Business.Document,
                                invoice.Environment,
                                invoice.Establishment?.Code ?? string.Empty,
                                invoice.EmissionPoint?.Code ?? string.Empty,
                                invoice.Sequential)
                            : invoice.AccessKey;

                        var xml = invoiceXmlBuilder.BuildXMLInvoice(
                            invoice,
                            invoice.Business!,
                            invoice.Establishment!,
                            invoice.EmissionPoint!,
                            invoice.Customer!);

                        var certificateBytes = Convert.FromBase64String(invoice.Business.BusinessCertificate.CertificateBase64);
                        var certificatePassword = aesEncryptionService.Decrypt(invoice.Business.BusinessCertificate.Password);

                        invoice.XmlSigned = await electronicSignature.SignXmlAsync(
                            xml,
                            certificateBytes,
                            certificatePassword,
                            stoppingToken);

                        var response = await sriReceptionService.SendInvoiceSriAsync(
                            invoice.XmlSigned,
                            invoice.Environment == EnvironmentStatus.PROD);

                        invoice.SriMessage = response.Message;
                        invoice.Status = response.Status;
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error al firmar o enviar la factura {InvoiceId} al SRI.", invoice.Id);
                        invoice.Status = InvoiceStatus.ERROR;
                        invoice.SriMessage = "Error al firmar o enviar la factura al SRI.";
                    }
                }

                await context.SaveChangesAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                logger.LogError("Operación cancelada al enviar comprobantes a Recepción del SRI.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al enviar comprobantes a Recepción del SRI.");
            }

            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
        }
    }

    private static string GenerateAccessKey(DateTime date, string documentType, string businessDocument, string environment, string establishment, string emissionPoint, string sequencial)
    {
        string currentDate = date.ToString("ddMMyyyy");

        string docType = documentType.PadLeft(2, '0');

        string document = businessDocument.PadLeft(13, '0');

        string invoiceEnvironment = environment;

        string serie = $"{establishment}{emissionPoint}";

        string sec = sequencial.PadLeft(9, '0');

        string numericCode = new Random().Next(10000000, 99999999).ToString();

        string emissionType = "1";

        string preKey = $"{currentDate}{docType}{document}{invoiceEnvironment}{serie}{sec}{numericCode}{emissionType}";

        string dv = CalculateCheckDigit(preKey).ToString();

        return preKey + dv;
    }

    private static int CalculateCheckDigit(string chain)
    {
        int[] factors = [2, 3, 4, 5, 6, 7];
        int factorIndex = 0;
        int sum = 0;

        for (int i = chain.Length - 1; i >= 0; i--)
        {
            int digit = int.Parse(chain[i].ToString());
            sum += digit * factors[factorIndex];
            factorIndex = (factorIndex + 1) % factors.Length;
        }

        int modulo = sum % 11;
        int digitVerifier = 11 - modulo;

        if (digitVerifier == 10) digitVerifier = 1;
        if (digitVerifier == 11) digitVerifier = 0;

        return digitVerifier;
    }
}
