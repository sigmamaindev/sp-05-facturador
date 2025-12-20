namespace Core.Constants;

public static class PaymentMethod
{
    public const string NFS = "01"; // SIN UTILIZACION DEL SISTEMA FINANCIERO
    public const string DEBIT_COMPESATION = "15"; // COMPENSACIÓN DE DEUDAS
    public const string DEBIT_CARD = "16"; // TARJETA DE DÉBITO
    public const string ELECTRONIC_MONEY = "17"; // DINERO ELECTRÓNICO
    public const string PREPAID_CARD = "18"; // TARJETA PREPAGO
    public const string CREDIT_CARD = "19"; // TARJETA DE CRÉDITO
    public const string FS = "20"; // OTROS CON UTILIZACIÓN DEL SISTEMA FINANCIERO
    public const string ENDORSEMENT_TITLES = "21"; // ENDOSO DE TÍTULOS
}
