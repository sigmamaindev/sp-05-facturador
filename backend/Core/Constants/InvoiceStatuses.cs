namespace Core.Constants;

public static class InvoiceStatuses
{
    public const string DRAFT = "BORRADOR";
    public const string PENDING = "PENDIENTE";
    public const string SIGNED = "FIRMADO";
    public const string SRI_RECEIVED = "RECIBIDA";
    public const string SRI_RETURNED = "DEVUELTA";
    public const string sri_REJECTED = "RECHAZADA";
    public const string SRI_TIMEOUT = "SRI NO RESPONDE";
    public const string SRI_UNAVAILABLE = "SRI NO DISPONIBLE";
    public const string SRI_INVALID_RESPONSE = "SRI RESPUESTA INVALIDA";
    public const string SRI_ERROR = "ERROR";
    public const string AUTHORIZED = "AUTORIZADO";
    public const string CANCELED = "ANULADO";
}
