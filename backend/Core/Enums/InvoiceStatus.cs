using System.Runtime.Serialization;

namespace Core.Enums;

public enum InvoiceStatus
{
    [EnumMember(Value = "BORRADOR")]
    DRAFT,
    [EnumMember(Value = "FIRMADO")]
    SIGNED,
    [EnumMember(Value = "PENDIENTE")]
    PENDING,
    [EnumMember(Value = "RECIBIDA")]
    SRI_RECEIVED,
    [EnumMember(Value = "DEVUELTA")]
    SRI_RETURNED,
    [EnumMember(Value = "AUTORIZADO")]
    SRI_AUTHORIZED,
    [EnumMember(Value = "RECHAZADO")]
    SRI_REJECTED,
    [EnumMember(Value = "NO AUTORIZADO")]
    SRI_NOT_AUTHORIZED,
    [EnumMember(Value = "SRI NO RESPONDE")]
    SRI_TIMEOUT,
    [EnumMember(Value = "SRI NO DISPONIBLE")]
    SRI_UNAVAILABLE,
    [EnumMember(Value = "SRI RESPUESTA INVALIDA")]
    SRI_INVALID_RESPONSE,
    [EnumMember(Value = "SRI SIN ESTADO")]
    SRI_NO_STATE,
    [EnumMember(Value = "ERROR")]
    ERROR
}
