using System;

namespace Core.DTOs.Warehouse;

public class GetWarehouseBusinessDto
{
    public int Id { get; set; }
    public string? Document { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
}
