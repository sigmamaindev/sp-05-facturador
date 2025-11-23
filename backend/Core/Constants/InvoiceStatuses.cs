namespace Core.Constants;

public static class InvoiceStatuses
{
    public const string DRAFT = "BORRADOR";
    public const string SIGNED = "FIRMADO";
    public const string SRI_RECEIVED = "RECIBIDA";
    public const string SRI_RETURNED = "DEVUELTA";
    public const string SRI_REJECTED = "RECHAZADO";
    public const string SRI_NOT_AUTHORIZED = "NO AUTORIZADO";
    public const string SRI_TIMEOUT = "SRI NO RESPONDE";
    public const string SRI_UNAVAILABLE = "SRI NO DISPONIBLE";
    public const string SRI_INVALID_RESPONSE = "SRI RESPUESTA INVALIDA";
    public const string SRI_NO_STATE = "SRI SIN ESTADO";
    public const string ERROR = "ERROR";
    public const string SRI_AUTHORIZED = "AUTORIZADO";
}
