using System.Text.Json;
using Core.DTOs.SriDto;
using Core.Interfaces.Services.ISriService;

namespace Infrastructure.Services.SriService;

public class SriPersonLookupService(HttpClient httpClient) : ISriPersonLookupService
{
    public async Task<SriPersonaResDto?> LookupByDocumentAsync(string document)
    {
        try
        {
            var url = $"https://srienlinea.sri.gob.ec/sri-catastro-sujeto-servicio-internet/rest/Persona/obtenerPersonaDesdeRucPorIdentificacion?numeroRuc={document}";

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var response = await httpClient.GetAsync(url, cts.Token);

            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var persona = JsonSerializer.Deserialize<SriPersonaResDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return persona;
        }
        catch
        {
            return null;
        }
    }
}
