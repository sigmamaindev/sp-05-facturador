using System;
using System.Collections.Generic;

namespace Core.DTOs.PurchaseDto;

public class PurchaseCreateReqDto
{
    public int BusinessId { get; set; }
    public int EstablishmentId { get; set; }
    public int WarehouseId { get; set; }
    public int SupplierId { get; set; }
    public DateTime PurchaseDate { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public List<PurchaseDetailCreateReqDto> Details { get; set; } = [];
}

