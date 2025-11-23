using Core.Constants;
using Core.Interfaces.Services;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
                    .Where(i => i.Status == InvoiceStatuses.SRI_RECEIVED && i.IsElectronic)
                    .ToListAsync(stoppingToken);

                foreach (var invoice in pendingInvoices)
                {
                    var response = await sriReceptionService.AuthorizeInvoiceSriAsync(
                        invoice.AccessKey,
                        invoice.Environment == EnvironmentStatuses.PROD);

                    invoice.SriMessage = response.Message;

                    if (response.State == InvoiceStatuses.SRI_AUTHORIZED)
                    {
                        invoice.Status = InvoiceStatuses.SRI_AUTHORIZED;
                        invoice.AuthorizationNumber = response.AuthorizationNumber;
                        invoice.AuthorizationDate = DateTime.UtcNow;
                    }
                    else if (response.State == InvoiceStatuses.SRI_NOT_AUTHORIZED)
                    {
                        invoice.Status = InvoiceStatuses.SRI_REJECTED;
                    }
                    else if (response.State is InvoiceStatuses.SRI_UNAVAILABLE or InvoiceStatuses.SRI_TIMEOUT)
                    {
                        invoice.Status = InvoiceStatuses.SRI_RECEIVED;
                    }
                    else
                    {
                        invoice.Status = response.State;
                    }
                }

                await context.SaveChangesAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Ignore cancellation exceptions when stopping
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al verificar autorizaciones del SRI.");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
