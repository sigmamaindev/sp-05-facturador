using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Infrastructure.Data;
using Core.Interfaces.Services;
using Core.Constants;

namespace Infrastructure.Services;

public class SriAuthorizationBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<SriAuthorizationBackgroundService> logger) : BackgroundService
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
                    .Where(i => i.Status == InvoiceStatus.SRI_RECEIVED && i.IsElectronic)
                    .ToListAsync(stoppingToken);

                var ecTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("America/Guayaquil"));

                foreach (var invoice in pendingInvoices)
                {
                    var response = await sriReceptionService.AuthorizeInvoiceSriAsync(
                        invoice.AccessKey,
                        invoice.Environment == EnvironmentStatuses.PROD);

                    invoice.SriMessage = response.Message;

                    if (response.Status == InvoiceStatus.SRI_AUTHORIZED)
                    {
                        invoice.Status = InvoiceStatus.SRI_AUTHORIZED;
                        invoice.AuthorizationNumber = response.AuthorizationNumber;
                        invoice.AuthorizationDate = ecTime;
                    }
                    else if (response.Status == InvoiceStatus.SRI_NOT_AUTHORIZED)
                    {
                        invoice.Status = InvoiceStatus.SRI_REJECTED;
                    }
                    else if (response.Status is InvoiceStatus.SRI_UNAVAILABLE or InvoiceStatus.SRI_TIMEOUT)
                    {
                        invoice.Status = InvoiceStatus.SRI_RECEIVED;
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
                logger.LogError("Operación cancelada al verificar autorizaciones del SRI.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al verificar autorizaciones del SRI.");
            }

            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
        }
    }
}
