namespace Core.DTOs.ARDto;

using Core.DTOs.CustomerDto;

public class ARCustomerSummaryResDto
{
    public CustomerResDto Customer { get; set; } = new();
    public decimal TotalBalance { get; set; }
}
