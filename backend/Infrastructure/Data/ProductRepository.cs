using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Core.Interfaces.Repository;
using Core.DTOs;
using Core.DTOs.ProductDto;
using Core.DTOs.TaxDto;
using Core.DTOs.UnitMeasureDto;
using Core.DTOs.InventoryDto;
using Core.Entities;
using Core.Constants;

namespace Infrastructure.Data;

public class ProductRepository(StoreContext context, IHttpContextAccessor httpContextAccessor) : IProductRepository
{
    public async Task<ApiResponse<ProductResDto>> CreateProductAsync(ProductCreateReqDto productCreateReqDto)
    {
        var response = new ApiResponse<ProductResDto>();

        try
        {
            var businessId = GetBusinessIdFromToken();

            if (businessId == 0)
            {
                response.Success = false;
                response.Message = "Negocio no asociado a esta usuario";
                response.Error = "Error de asociación";

                return response;
            }

            var skuExists = await context.Products
            .AnyAsync(p => p.BusinessId == businessId && p.Sku == productCreateReqDto.Sku);

            if (skuExists)
            {
                response.Success = false;
                response.Message = "El producto ya está registrado en este negocio";
                response.Error = "Error de duplicación";

                return response;
            }

            var presentations = new List<ProductPresentationCreateReqDto>();

            if (productCreateReqDto.DefaultPresentation is null)
            {
                response.Success = false;
                response.Message = "La presentación por defecto es obligatoria";
                response.Error = "Error de validación";

                return response;
            }

            presentations.Add(productCreateReqDto.DefaultPresentation);

            if (productCreateReqDto.Presentations is not null && productCreateReqDto.Presentations.Count > 0)
            {
                presentations.AddRange(productCreateReqDto.Presentations);
            }

            var unitMeasureIds = presentations.Select(p => p.UnitMeasureId).Distinct().ToList();

            var unitMeasuresOk = await context.UnitMeasures
            .CountAsync(um => unitMeasureIds.Contains(um.Id));

            if (unitMeasuresOk != unitMeasureIds.Count)
            {
                response.Success = false;
                response.Message = "Una o más unidades de medida no son válidas";
                response.Error = "Error de validación";

                return response;
            }

            if (unitMeasureIds.Count != presentations.Count)
            {
                response.Success = false;
                response.Message = "No puedes repetir la misma unidad de medida en presentaciones";
                response.Error = "Error de validación";

                return response;
            }

            await using var trx = await context.Database.BeginTransactionAsync();

            var newProduct = new Product
            {
                Sku = productCreateReqDto.Sku.Trim(),
                Name = productCreateReqDto.Name.Trim(),
                Description = productCreateReqDto.Description.Trim(),
                BusinessId = businessId,
                TaxId = productCreateReqDto.TaxId,
                Type = string.IsNullOrWhiteSpace(productCreateReqDto.Type) ? ProductTypes.GOOD : productCreateReqDto.Type
            };

            context.Products.Add(newProduct);
            await context.SaveChangesAsync();

            var createdPresentations = new List<ProductPresentation>();
            for (var i = 0; i < presentations.Count; i++)
            {
                var p = presentations[i];
                var isDefault = i == 0;

                createdPresentations.Add(new ProductPresentation
                {
                    ProductId = newProduct.Id,
                    UnitMeasureId = p.UnitMeasureId,
                    Price01 = p.Price01,
                    Price02 = p.Price02,
                    Price03 = p.Price03,
                    Price04 = p.Price04,
                    NetWeight = p.NetWeight,
                    GrossWeight = p.GrossWeight,
                    IsDefault = isDefault,
                    IsActive = true
                });
            }

            context.Set<ProductPresentation>().AddRange(createdPresentations);
            await context.SaveChangesAsync();

            await trx.CommitAsync();

            response.Success = true;
            response.Message = "Producto creado correctamente";

            var tax = await context.Taxes
                .AsNoTracking()
                .Where(t => t.Id == newProduct.TaxId && t.BusinessId == businessId)
                .Select(t => new TaxResDto
                {
                    Id = t.Id,
                    Code = t.Code,
                    CodePercentage = t.CodePercentage,
                    Name = t.Name,
                    Group = t.Group,
                    Rate = t.Rate,
                    IsActive = t.IsActive
                })
                .FirstOrDefaultAsync();

            response.Data = new ProductResDto
            {
                Id = newProduct.Id,
                Sku = newProduct.Sku,
                Name = newProduct.Name,
                Description = newProduct.Description,
                Type = newProduct.Type,
                TaxId = newProduct.TaxId,
                Tax = tax,
                IsActive = newProduct.IsActive
            };
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al crear el producto";
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<ApiResponse<ProductResDto>> GetProductByIdAsync(int id)
    {
        var response = new ApiResponse<ProductResDto>();

        try
        {
            var businessId = GetBusinessIdFromToken();

            if (businessId == 0)
            {
                response.Success = false;
                response.Message = "Negocio no asociado a esta usuario";
                response.Error = "Error de asociación";

                return response;
            }

            var product = await context.Products
            .AsNoTracking()
            .Include(p => p.Tax)
            .Include(p => p.ProductPresentations)
                .ThenInclude(pp => pp.UnitMeasure)
            .Include(p => p.ProductWarehouses)
                .ThenInclude(pw => pw.Warehouse)
            .Where(p => p.IsActive && p.BusinessId == businessId && p.Id == id)
            .Select(p => new ProductResDto
            {
                Id = p.Id,
                Sku = p.Sku,
                Name = p.Name,
                Description = p.Description,
                Type = p.Type,
                TaxId = p.TaxId,
                Tax = p.Tax == null ? null : new TaxResDto
                {
                    Id = p.Tax.Id,
                    Code = p.Tax.Code,
                    CodePercentage = p.Tax.CodePercentage,
                    Name = p.Tax.Name,
                    Group = p.Tax.Group,
                    Rate = p.Tax.Rate,
                    IsActive = p.Tax.IsActive
                },
                IsActive = p.IsActive,

                DefaultPresentation = p.ProductPresentations
                    .Where(pp => pp.IsActive && pp.IsDefault)
                    .Select(pp => new ProductPresentationResDto
                    {
                        Id = pp.Id,
                        UnitMeasureId = pp.UnitMeasureId,
                        UnitMeasure = pp.UnitMeasure == null ? null : new UnitMeasureResDto
                        {
                            Id = pp.UnitMeasure.Id,
                            Code = pp.UnitMeasure.Code,
                            Name = pp.UnitMeasure.Name,
                            FactorBase = pp.UnitMeasure.FactorBase,
                            IsActive = pp.UnitMeasure.IsActive
                        },
                        Price01 = pp.Price01,
                        Price02 = pp.Price02,
                        Price03 = pp.Price03,
                        Price04 = pp.Price04,
                        NetWeight = pp.NetWeight,
                        GrossWeight = pp.GrossWeight,
                        IsDefault = pp.IsDefault,
                        IsActive = pp.IsActive
                    })
                    .FirstOrDefault(),

                Presentations = p.ProductPresentations
                    .OrderByDescending(pp => pp.IsDefault)
                    .ThenBy(pp => pp.UnitMeasureId)
                    .Select(pp => new ProductPresentationResDto
                    {
                        Id = pp.Id,
                        UnitMeasureId = pp.UnitMeasureId,
                        UnitMeasure = pp.UnitMeasure == null ? null : new UnitMeasureResDto
                        {
                            Id = pp.UnitMeasure.Id,
                            Code = pp.UnitMeasure.Code,
                            Name = pp.UnitMeasure.Name,
                            FactorBase = pp.UnitMeasure.FactorBase,
                            IsActive = pp.UnitMeasure.IsActive
                        },
                        Price01 = pp.Price01,
                        Price02 = pp.Price02,
                        Price03 = pp.Price03,
                        Price04 = pp.Price04,
                        NetWeight = pp.NetWeight,
                        GrossWeight = pp.GrossWeight,
                        IsDefault = pp.IsDefault,
                        IsActive = pp.IsActive
                    })
                    .ToList(),

                Inventory = p.ProductWarehouses.Select(pw => new InventoryResDto
                {
                    Id = pw.Id,
                    WarehouseId = pw.Warehouse!.Id,
                    WarehouseCode = pw.Warehouse.Code,
                    WarehouseName = pw.Warehouse.Name,
                    Stock = pw.Stock,
                    MinStock = pw.MinStock,
                    MaxStock = pw.MaxStock
                }).ToList()
            })
            .FirstOrDefaultAsync();

            if (product == null)
            {
                response.Success = false;
                response.Message = "Producto no encontrado";
                response.Error = "No existe un producto con el ID especificado";
                return response;
            }

            response.Success = true;
            response.Message = "Producto obtenido correctamente";
            response.Data = product;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al obtener el producto";
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<ApiResponse<List<ProductResDto>>> GetProductsAsync(string? keyword, int page, int limit)
    {
        var response = new ApiResponse<List<ProductResDto>>();

        try
        {
            var businessId = GetBusinessIdFromToken();

            if (businessId == 0)
            {
                response.Success = false;
                response.Message = "Negocio no asociado a esta usuario";
                response.Error = "Error de asociación";

                return response;
            }

            var query = context.Products
            .AsNoTracking()
            .Include(p => p.Tax)
            .Include(p => p.ProductPresentations)
                .ThenInclude(pp => pp.UnitMeasure)
            .Include(p => p.ProductWarehouses)
                .ThenInclude(pw => pw.Warehouse)
            .Where(p => p.IsActive && p.BusinessId == businessId);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(p =>
                    EF.Functions.ILike(p.Sku, $"%{keyword}%") ||
                    EF.Functions.ILike(p.Name, $"%{keyword}%") ||
                    EF.Functions.ILike(p.Description, $"%{keyword}%"));
            }

            var total = await query.CountAsync();

            var products = await query
                .OrderBy(p => p.Sku)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(p => new ProductResDto
                {
                    Id = p.Id,
                    Sku = p.Sku,
                    Name = p.Name,
                    Description = p.Description,
                    Type = p.Type,
                    TaxId = p.TaxId,
                    Tax = p.Tax == null ? null : new TaxResDto
                    {
                        Id = p.Tax.Id,
                        Code = p.Tax.Code,
                        CodePercentage = p.Tax.CodePercentage,
                        Name = p.Tax.Name,
                        Group = p.Tax.Group,
                        Rate = p.Tax.Rate,
                        IsActive = p.Tax.IsActive
                    },
                    IsActive = p.IsActive,

                    DefaultPresentation = p.ProductPresentations
                        .Where(pp => pp.IsActive && pp.IsDefault)
                        .Select(pp => new ProductPresentationResDto
                        {
                            Id = pp.Id,
                            UnitMeasureId = pp.UnitMeasureId,
                            UnitMeasure = pp.UnitMeasure == null ? null : new UnitMeasureResDto
                            {
                                Id = pp.UnitMeasure.Id,
                                Code = pp.UnitMeasure.Code,
                                Name = pp.UnitMeasure.Name,
                                FactorBase = pp.UnitMeasure.FactorBase,
                                IsActive = pp.UnitMeasure.IsActive
                            },
                            Price01 = pp.Price01,
                            Price02 = pp.Price02,
                            Price03 = pp.Price03,
                            Price04 = pp.Price04,
                            NetWeight = pp.NetWeight,
                            GrossWeight = pp.GrossWeight,
                            IsDefault = pp.IsDefault,
                            IsActive = pp.IsActive
                        })
                        .FirstOrDefault(),

                    Presentations = p.ProductPresentations
                        .Where(pp => pp.IsActive)
                        .OrderByDescending(pp => pp.IsDefault)
                        .ThenBy(pp => pp.UnitMeasureId)
                        .Select(pp => new ProductPresentationResDto
                        {
                            Id = pp.Id,
                            UnitMeasureId = pp.UnitMeasureId,
                            UnitMeasure = pp.UnitMeasure == null ? null : new UnitMeasureResDto
                            {
                                Id = pp.UnitMeasure.Id,
                                Code = pp.UnitMeasure.Code,
                                Name = pp.UnitMeasure.Name,
                                FactorBase = pp.UnitMeasure.FactorBase,
                                IsActive = pp.UnitMeasure.IsActive
                            },
                            Price01 = pp.Price01,
                            Price02 = pp.Price02,
                            Price03 = pp.Price03,
                            Price04 = pp.Price04,
                            NetWeight = pp.NetWeight,
                            GrossWeight = pp.GrossWeight,
                            IsDefault = pp.IsDefault,
                            IsActive = pp.IsActive
                        })
                        .ToList(),

                    Inventory = p.ProductWarehouses.Select(pw => new InventoryResDto
                    {
                        Id = pw.Id,
                        WarehouseId = pw.Warehouse!.Id,
                        WarehouseCode = pw.Warehouse.Code,
                        WarehouseName = pw.Warehouse.Name,
                        Stock = pw.Stock,
                        MinStock = pw.MinStock,
                        MaxStock = pw.MaxStock
                    }).ToList()
                })
                .ToListAsync();

            response.Success = true;
            response.Message = "Productos obtenidos correctamente";
            response.Data = products;
            response.Pagination = new Pagination
            {
                Total = total,
                Page = page,
                Limit = limit
            };
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al obtener los productos";
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<ApiResponse<ProductResDto>> UpdateProductAsync(int productId, ProductUpdateReqDto dto)
    {
        var response = new ApiResponse<ProductResDto>();

        try
        {
            var businessId = GetBusinessIdFromToken();

            if (businessId == 0)
            {
                response.Success = false;
                response.Message = "Negocio no asociado a esta usuario";
                response.Error = "Error de asociación";
                return response;
            }

            // Producto base
            var product = await context.Products
                .Include(p => p.ProductPresentations)
                .FirstOrDefaultAsync(p => p.Id == productId && p.IsActive && p.BusinessId == businessId);

            if (product == null)
            {
                response.Success = false;
                response.Message = "Producto no encontrado";
                response.Error = "No existe un producto con el ID especificado";
                return response;
            }

            // (Recomendado) validar Tax del negocio
            var taxOk = await context.Taxes
                .AnyAsync(t => t.Id == dto.TaxId && t.BusinessId == businessId && t.IsActive);

            if (!taxOk)
            {
                response.Success = false;
                response.Message = "Impuesto no válido para el negocio actual";
                response.Error = "Validación";
                return response;
            }

            await using var trx = await context.Database.BeginTransactionAsync();

            // Update base
            product.Name = dto.Name.Trim();
            product.Description = dto.Description.Trim();
            product.Type = dto.Type;
            product.TaxId = dto.TaxId;

            // ======= Presentación default =======
            if (dto.DefaultPresentation is null)
            {
                response.Success = false;
                response.Message = "La presentación default es obligatoria";
                response.Error = "Validación";
                return response;
            }

            // Garantizar una default existente (por DB debería existir)
            var defaultPres = product.ProductPresentations.FirstOrDefault(pp => pp.IsDefault);

            if (defaultPres == null)
            {
                // si por datos viejos no existe, la creamos
                defaultPres = new ProductPresentation
                {
                    ProductId = product.Id,
                    IsDefault = true,
                    IsActive = true
                };
                context.Set<ProductPresentation>().Add(defaultPres);
                product.ProductPresentations.Add(defaultPres);
            }

            // Validar UnitMeasure (ajusta si UnitMeasure es multi-empresa)
            var defaultUmOk = await context.UnitMeasures
                .AnyAsync(um => um.Id == dto.DefaultPresentation.UnitMeasureId /* && um.BusinessId == businessId */);

            if (!defaultUmOk)
            {
                response.Success = false;
                response.Message = "Unidad de medida default no válida";
                response.Error = "Validación";
                return response;
            }

            // Si cambian UnitMeasureId, validar que no choque con otra presentación del mismo producto
            var newDefaultUmId = dto.DefaultPresentation.UnitMeasureId;
            var umCollision = product.ProductPresentations.Any(pp =>
                pp.Id != defaultPres.Id && pp.UnitMeasureId == newDefaultUmId);

            if (umCollision)
            {
                response.Success = false;
                response.Message = "Ya existe una presentación con esa unidad de medida en este producto";
                response.Error = "Validación";
                return response;
            }

            defaultPres.UnitMeasureId = dto.DefaultPresentation.UnitMeasureId;
            defaultPres.Price01 = dto.DefaultPresentation.Price01;
            defaultPres.Price02 = dto.DefaultPresentation.Price02;
            defaultPres.Price03 = dto.DefaultPresentation.Price03;
            defaultPres.Price04 = dto.DefaultPresentation.Price04;
            defaultPres.NetWeight = dto.DefaultPresentation.NetWeight;
            defaultPres.GrossWeight = dto.DefaultPresentation.GrossWeight;
            defaultPres.IsActive = dto.DefaultPresentation.IsActive;
            defaultPres.IsDefault = true;

            // ======= Presentaciones extra (opcional) =======
            // Regla: si dto.Presentations viene vacío, no toques las existentes.
            // Si quieres sincronizar completo (crear/actualizar/desactivar), usa este bloque.
            if (dto.Presentations is not null && dto.Presentations.Count > 0)
            {
                // Validar duplicados de UnitMeasureId en request (incluye default)
                var allUmIds = new List<int> { dto.DefaultPresentation.UnitMeasureId };
                allUmIds.AddRange(dto.Presentations.Select(x => x.UnitMeasureId));

                if (allUmIds.Distinct().Count() != allUmIds.Count)
                {
                    response.Success = false;
                    response.Message = "No puedes repetir la misma unidad de medida en presentaciones";
                    response.Error = "Validación";
                    return response;
                }

                // Validar que UnitMeasures existan
                var umIdsDistinct = allUmIds.Distinct().ToList();
                var umCount = await context.UnitMeasures
                    .CountAsync(um => umIdsDistinct.Contains(um.Id) /* && um.BusinessId == businessId */);

                if (umCount != umIdsDistinct.Count)
                {
                    response.Success = false;
                    response.Message = "Una o más unidades de medida no son válidas";
                    response.Error = "Validación";
                    return response;
                }

                foreach (var presDto in dto.Presentations)
                {
                    // Ignorar si el request intenta marcar otra como default
                    if (presDto.IsDefault)
                    {
                        response.Success = false;
                        response.Message = "Solo puedes actualizar el default en DefaultPresentation";
                        response.Error = "Validación";
                        return response;
                    }

                    if (presDto.Id.HasValue)
                    {
                        // Update existente
                        var existing = product.ProductPresentations.FirstOrDefault(pp => pp.Id == presDto.Id.Value);
                        if (existing == null)
                        {
                            response.Success = false;
                            response.Message = $"Presentación {presDto.Id.Value} no encontrada en este producto";
                            response.Error = "Validación";
                            return response;
                        }

                        existing.UnitMeasureId = presDto.UnitMeasureId;
                        existing.Price01 = presDto.Price01;
                        existing.Price02 = presDto.Price02;
                        existing.Price03 = presDto.Price03;
                        existing.Price04 = presDto.Price04;
                        existing.NetWeight = presDto.NetWeight;
                        existing.GrossWeight = presDto.GrossWeight;
                        existing.IsActive = presDto.IsActive;
                        existing.IsDefault = false;
                    }
                    else
                    {
                        // Crear nueva
                        context.Set<ProductPresentation>().Add(new ProductPresentation
                        {
                            ProductId = product.Id,
                            UnitMeasureId = presDto.UnitMeasureId,
                            Price01 = presDto.Price01,
                            Price02 = presDto.Price02,
                            Price03 = presDto.Price03,
                            Price04 = presDto.Price04,
                            NetWeight = presDto.NetWeight,
                            GrossWeight = presDto.GrossWeight,
                            IsActive = presDto.IsActive,
                            IsDefault = false
                        });
                    }
                }
            }

            await context.SaveChangesAsync();
            await trx.CommitAsync();

            // Respuesta (reusar tu GetProductByIdAsync si quieres)
            var updated = await GetProductByIdAsync(productId);

            response.Success = true;
            response.Message = "Producto obtenido correctamente";
            response.Data = updated.Data;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al actualizar el producto";
            response.Error = ex.Message;
            return response;
        }

        return response;
    }

    private int GetBusinessIdFromToken()
    {
        var businessIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("BusinessId")?.Value;
        return int.TryParse(businessIdClaim, out var id) ? id : 0;
    }
}
