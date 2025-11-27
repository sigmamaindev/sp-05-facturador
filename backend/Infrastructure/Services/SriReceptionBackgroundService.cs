using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Infrastructure.Data;
using Core.Interfaces.Services;
using Core.Constants;
using Core.Enums;

namespace Infrastructure.Services;

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

                var pendingInvoices = await context.Invoices
                    .Where(i =>
                        i.IsElectronic &&
                        (i.Status == InvoiceStatus.SRI_TIMEOUT ||
                         i.Status == InvoiceStatus.SRI_UNAVAILABLE ||
                         i.Status == InvoiceStatus.PENDING))
                    .ToListAsync(stoppingToken);

                foreach (var invoice in pendingInvoices)
                {
                    var response = await sriReceptionService.SendInvoiceSriAsync(
                        invoice.XmlSigned!,
                        invoice.Environment == EnvironmentStatuses.PROD);

                    invoice.SriMessage = response.Message;

                    if (response.Status == InvoiceStatus.SRI_RECEIVED)
                    {
                        invoice.Status = InvoiceStatus.SRI_RECEIVED;
                    }
                    else if (response.Status == InvoiceStatus.SRI_REJECTED)
                    {
                        invoice.Status = InvoiceStatus.SRI_REJECTED;
                    }
                    else if (response.Status is InvoiceStatus.SRI_TIMEOUT or InvoiceStatus.SRI_UNAVAILABLE)
                    {
                        invoice.Status = response.Status;
                    }
                    else
                    {
                        invoice.Status = response.Status;
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
