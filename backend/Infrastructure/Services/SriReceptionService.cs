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
            ? "https://cel.sri.gob.ec/comprobantes-electronicos-ws/RecepcionComprobantesOffline?wsdl"
            : "https://celcer.sri.gob.ec/comprobantes-electronicos-ws/RecepcionComprobantesOffline?wsdl";


        var soapEnvelope =
        $@"<?xml version=""1.0"" encoding=""UTF-8""?>
        <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/""
                  xmlns:ec=""http://ec.gob.sri.ws.recepcion"">
        <soapnv:Header/>
        <soapenv:Body>
        <ec:validarComprobante>
        <xml><![CDATA[{signedXml}]]></xml>
        </ec:validarComprobante>
        </soapenv:Body>
        </soapenv:Envelope>";

        var content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");
        content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");
        content.Headers.Add("SOAPAction", "validarComprobante");

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
                State = InvoiceStatuses.SRI_ERROR,
                Message = $"Error desconocido al enviar comprobante: {ex.Message}"
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
                State = "SRI_NO_RESPONDE",
                Message = "El SRI no devolvió ninguna respuesta."
            };
        }

        if (xml.StartsWith("<!DOCTYPE html") || xml.StartsWith("<html"))
        {
            return new SriReceptionResponseDto
            {
                Success = false,
                State = "SRI_MANTENIMIENTO",
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
                State = "SRI_RESPUESTA_INVALIDA",
                Message = "El SRI devolvió una respuesta no válida."
            };
        }

        var fault = doc.GetElementsByTagName("faultstring").Item(0);
        if (fault != null)
        {
            return new SriReceptionResponseDto
            {
                Success = false,
                State = "SRI_FAULT",
                Message = $"SOAP Fault: {fault.InnerText}"
            };
        }

        var estadoNode = doc.GetElementsByTagName("estado").Item(0);

        if (estadoNode == null)
        {
            return new SriReceptionResponseDto
            {
                Success = false,
                State = "SRI_SIN_ESTADO",
                Message = "El SRI no devolvió estado en la respuesta."
            };
        }

        var estado = estadoNode.InnerText.Trim();
        result.State = estado;

        if (estado == "RECIBIDA")
        {
            result.Success = true;
            result.Message = "Comprobante recibido por el SRI.";
            return result;
        }

        if (estado == "DEVUELTA")
        {
            result.Success = false;
            result.Message = ParseErrors(doc);
            return result;
        }

        return new SriReceptionResponseDto
        {
            Success = false,
            State = estado,
            Message = $"Estado desconocido devuelto por el SRI: {estado}"
        };
    }

    private static string ParseErrors(XmlDocument doc)
    {
        var sb = new StringBuilder();
        var mensajes = doc.GetElementsByTagName("mensaje");

        if (mensajes.Count == 0)
            return "El SRI devolvió un error sin detalles.";

        foreach (XmlNode m in mensajes)
        {
            var identificador = m["identificador"]?.InnerText ?? "";
            var mensaje = m["mensaje"]?.InnerText ?? "";
            var info = m["informacionAdicional"]?.InnerText ?? "";

            sb.AppendLine($"{identificador}: {mensaje} {info}".Trim());
        }

        return sb.ToString();
    }
}
