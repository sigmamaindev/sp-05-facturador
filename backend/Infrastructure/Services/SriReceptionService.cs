using System.Net.Http.Headers;
using System.Text;
using System.Xml;
using Core.Constants;
using Core.DTOs.SRI;
using Core.Interfaces.Services;

namespace Infrastructure.Services;

public class SriReceptionService(HttpClient httpClient) : ISriReceptionService
{
    public async Task<SriReceptionResponseDto> SendInvoiceSriAsync(string signedXml, bool isProduction)
    {

        var url = isProduction
            ? "https://cel.sri.gob.ec/comprobantes-electronicos-ws/RecepcionComprobantesOffline"
            : "https://celcer.sri.gob.ec/comprobantes-electronicos-ws/RecepcionComprobantesOffline";


        var xmlBytes = Encoding.UTF8.GetBytes(signedXml);
        var xmlBase64 = Convert.ToBase64String(xmlBytes);

        var soapEnvelope =
$@"<?xml version=""1.0"" encoding=""UTF-8""?>
<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/""
                  xmlns:ec=""http://ec.gob.sri.ws.recepcion"">
  <soapenv:Header/>
  <soapenv:Body>
    <ec:validarComprobante>
      <xml>{xmlBase64}</xml>
    </ec:validarComprobante>
  </soapenv:Body>
</soapenv:Envelope>";

        var content = new StringContent(soapEnvelope, Encoding.UTF8);
        content.Headers.ContentType = MediaTypeHeaderValue.Parse("text/xml; charset=utf-8");

        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(25));

            var httpResponse = await httpClient.PostAsync(url, content, cts.Token);
            var xmlResponse = await httpResponse.Content.ReadAsStringAsync();

            return ParseResponse(xmlResponse);
        }
        catch (TaskCanceledException)
        {
            return new SriReceptionResponseDto
            {
                Success = false,
                State = InvoiceStatuses.SRI_TIMEOUT,
                Message = "El SRI no respondió (timeout). Intente más tarde."
            };
        }
        catch (HttpRequestException)
        {
            return new SriReceptionResponseDto
            {
                Success = false,
                State = InvoiceStatuses.SRI_UNAVAILABLE,
                Message = "No fue posible conectarse al SRI. Servicio no disponible."
            };
        }
        catch (Exception ex)
        {
            return new SriReceptionResponseDto
            {
                Success = false,
                State = InvoiceStatuses.ERROR,
                Message = $"Error desconocido al enviar comprobante: {ex.Message}"
            };
        }
    }

    public async Task<SriAuthorizationResponseDto> AuthorizeInvoiceSriAsync(string accessKey, bool isProduction)
    {
        var url = isProduction
            ? "https://cel.sri.gob.ec/comprobantes-electronicos-ws/AutorizacionComprobantesOffline"
            : "https://celcer.sri.gob.ec/comprobantes-electronicos-ws/AutorizacionComprobantesOffline";

        var soapEnvelope =
@$"<?xml version=""1.0"" encoding=""UTF-8""?>
<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/""
                  xmlns:ec=""http://ec.gob.sri.ws.autorizacion"">
  <soapenv:Header/>
  <soapenv:Body>
    <ec:autorizacionComprobante>
      <claveAccesoComprobante>{accessKey}</claveAccesoComprobante>
    </ec:autorizacionComprobante>
  </soapenv:Body>
</soapenv:Envelope>";

        var content = new StringContent(soapEnvelope, Encoding.UTF8);
        content.Headers.ContentType = MediaTypeHeaderValue.Parse("text/xml; charset=utf-8");

        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(25));

            var httpResponse = await httpClient.PostAsync(url, content, cts.Token);
            var xmlResponse = await httpResponse.Content.ReadAsStringAsync();

            return ParseAuthorizationResponse(xmlResponse);
        }
        catch (TaskCanceledException)
        {
            return new SriAuthorizationResponseDto
            {
                Success = false,
                State = InvoiceStatuses.SRI_TIMEOUT,
                Message = "El SRI no respondió (timeout). Intente más tarde."
            };
        }
        catch (HttpRequestException)
        {
            return new SriAuthorizationResponseDto
            {
                Success = false,
                State = InvoiceStatuses.SRI_UNAVAILABLE,
                Message = "No fue posible conectarse al SRI. Servicio no disponible."
            };
        }
        catch (Exception ex)
        {
            return new SriAuthorizationResponseDto
            {
                Success = false,
                State = InvoiceStatuses.ERROR,
                Message = $"Error desconocido al autorizar comprobante: {ex.Message}"
            };
        }
    }

    private static SriReceptionResponseDto ParseResponse(string xml)
    {
        var result = new SriReceptionResponseDto();

        if (string.IsNullOrWhiteSpace(xml))
        {
            return new SriReceptionResponseDto
            {
                Success = false,
                State = InvoiceStatuses.SRI_TIMEOUT,
                Message = "El SRI no devolvió ninguna respuesta."
            };
        }

        if (xml.StartsWith("<!DOCTYPE html") || xml.StartsWith("<html"))
        {
            return new SriReceptionResponseDto
            {
                Success = false,
                State = InvoiceStatuses.SRI_UNAVAILABLE,
                Message = "El SRI está en mantenimiento o devolvió HTML."
            };
        }

        var doc = new XmlDocument();

        try
        {
            doc.LoadXml(xml);
        }
        catch
        {
            return new SriReceptionResponseDto
            {
                Success = false,
                State = InvoiceStatuses.SRI_INVALID_RESPONSE,
                Message = "El SRI devolvió una respuesta no válida."
            };
        }

        var fault = doc.GetElementsByTagName("faultstring").Item(0);
        if (fault != null)
        {
            var faultMessage = fault.InnerText.Trim();

            if (faultMessage.Contains("unmarshalling error", StringComparison.OrdinalIgnoreCase))
            {
                return new SriReceptionResponseDto
                {
                    Success = false,
                    State = InvoiceStatuses.SRI_INVALID_RESPONSE,
                    Message = "El SRI no pudo interpretar el XML enviado (Unmarshalling Error). Revise que el comprobante coincida con el esquema y que la firma sea válida."
                };
            }

            return new SriReceptionResponseDto
            {
                Success = false,
                State = InvoiceStatuses.ERROR,
                Message = $"SOAP Fault: {faultMessage}"
            };
        }

        var statusNode = doc.GetElementsByTagName("estado").Item(0);

        if (statusNode == null)
        {
            return new SriReceptionResponseDto
            {
                Success = false,
                State = "",
                Message = "El SRI no devolvió estado en la respuesta."
            };
        }

        var status = statusNode.InnerText.Trim();
        result.State = status;

        if (status == InvoiceStatuses.SRI_RECEIVED)
        {
            result.Success = true;
            result.Message = "Comprobante recibido por el SRI.";
            return result;
        }

        if (status == InvoiceStatuses.SRI_RETURNED || status == InvoiceStatuses.SRI_REJECTED)
        {
            result.Success = false;
            result.Message = ParseErrors(doc);
            return result;
        }

        return new SriReceptionResponseDto
        {
            Success = false,
            State = status,
            Message = $"Estado desconocido devuelto por el SRI: {status}"
        };
    }

    private static string ParseErrors(XmlDocument doc)
    {
        var sb = new StringBuilder();
        var messages = doc.GetElementsByTagName("mensaje");

        if (messages.Count == 0)
            return "El SRI devolvió un error sin detalles.";

        foreach (XmlNode m in messages)
        {
            var identifier = m["identificador"]?.InnerText ?? "";
            var message = m["mensaje"]?.InnerText ?? "";
            var info = m["informacionAdicional"]?.InnerText ?? "";

            sb.AppendLine($"{identifier}: {message} {info}".Trim());
        }

        return sb.ToString();
    }

    private static SriAuthorizationResponseDto ParseAuthorizationResponse(string xml)
    {
        if (string.IsNullOrWhiteSpace(xml))
        {
            return new SriAuthorizationResponseDto
            {
                Success = false,
                State = InvoiceStatuses.SRI_TIMEOUT,
                Message = "El SRI no devolvió ninguna respuesta."
            };
        }

        if (xml.StartsWith("<!DOCTYPE html") || xml.StartsWith("<html"))
        {
            return new SriAuthorizationResponseDto
            {
                Success = false,
                State = InvoiceStatuses.SRI_UNAVAILABLE,
                Message = "El SRI está en mantenimiento o devolvió HTML."
            };
        }

        var doc = new XmlDocument();

        try
        {
            doc.LoadXml(xml);
        }
        catch
        {
            return new SriAuthorizationResponseDto
            {
                Success = false,
                State = InvoiceStatuses.SRI_INVALID_RESPONSE,
                Message = "El SRI devolvió una respuesta no válida."
            };
        }

        var fault = doc.GetElementsByTagName("faultstring").Item(0);
        if (fault != null)
        {
            var faultMessage = fault.InnerText.Trim();

            return new SriAuthorizationResponseDto
            {
                Success = false,
                State = InvoiceStatuses.ERROR,
                Message = $"SOAP Fault: {faultMessage}"
            };
        }

        var authorizationNode = doc.GetElementsByTagName("autorizacion").Item(0);
        var statusNode = authorizationNode?["estado"] ?? doc.GetElementsByTagName("estado").Item(0);

        if (statusNode == null)
        {
            return new SriAuthorizationResponseDto
            {
                Success = false,
                State = InvoiceStatuses.SRI_NO_STATE,
                Message = "El SRI no devolvió estado en la autorización."
            };
        }

        var status = statusNode.InnerText.Trim().ToUpperInvariant();

        if (status == InvoiceStatuses.SRI_AUTHORIZED)
        {
            var authorizationNumber = authorizationNode?["numeroAutorizacion"]?.InnerText.Trim() ?? string.Empty;
            DateTime? authorizationDate = null;
            var dateText = authorizationNode?["fechaAutorizacion"]?.InnerText.Trim();

            if (DateTime.TryParse(dateText, out var parsedDate))
            {
                authorizationDate = parsedDate;
            }

            return new SriAuthorizationResponseDto
            {
                Success = true,
                State = InvoiceStatuses.SRI_AUTHORIZED,
                Message = "Comprobante autorizado.",
                AuthorizationNumber = authorizationNumber,
                AuthorizationDate = authorizationDate
            };
        }

        if (status == InvoiceStatuses.SRI_NOT_AUTHORIZED)
        {
            return new SriAuthorizationResponseDto
            {
                Success = false,
                State = InvoiceStatuses.SRI_NOT_AUTHORIZED,
                Message = ParseAuthorizationMessages(authorizationNode)
            };
        }

        return new SriAuthorizationResponseDto
        {
            Success = false,
            State = status,
            Message = ParseAuthorizationMessages(authorizationNode)
        };
    }

    private static string ParseAuthorizationMessages(XmlNode? authorizationNode)
    {
        var messages = authorizationNode?.SelectNodes(".//mensaje");

        if (messages == null || messages.Count == 0)
            return "El SRI devolvió un estado sin mensajes.";

        var sb = new StringBuilder();

        foreach (XmlNode m in messages)
        {
            var identifier = m["identificador"]?.InnerText ?? "";
            var message = m["mensaje"]?.InnerText ?? "";
            var info = m["informacionAdicional"]?.InnerText ?? "";

            sb.AppendLine($"{identifier}: {message} {info}".Trim());
        }

        return sb.ToString();
    }
}
