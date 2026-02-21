namespace Core.DTOs.SriDto;

public class SriPersonaResDto
{
    public string Identificacion { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string TipoPersona { get; set; } = string.Empty;
    public string? CodigoPersona { get; set; }
}
