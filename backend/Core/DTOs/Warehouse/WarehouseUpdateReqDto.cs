using System;

namespace Core.DTOs.Warehouse;

public class WarehouseUpdateReqDto
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}
