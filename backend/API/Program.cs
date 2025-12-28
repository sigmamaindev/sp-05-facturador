using System.Text;
using Yamgooo.SRI.Sign;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Infrastructure.Services.InvoiceService;
using Infrastructure.Services.UtilService;
using Infrastructure.Services.SriService;
using Infrastructure.Services.KardexService;
using Core.Interfaces.Repository;
using Core.Interfaces.Services.IInvoiceService;
using Core.Interfaces.Services.IUtilService;
using Core.Interfaces.Services.IKardexService;
using Core.Interfaces.Services.IPurchaseService;
using Infrastructure.Services.PurchaseService;
using Core.Interfaces.Services.IARService;
using Infrastructure.Services.ARService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowAll", policy =>
    {
        policy
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddDbContext<StoreContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Repositories
builder.Services.AddScoped<IBusinessRepository, BusinessRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IEstablishmentRepository, EstablishmentRepository>();
builder.Services.AddScoped<IEmissionPointRepository, EmissionPointRepository>();
builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUnitMeasureRepository, UnitMeasureRepository>();
builder.Services.AddScoped<ITaxRepository, TaxRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IAccountsReceivableRepository, AccountsReceivableRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<ICertificateRepository, CertificateRepository>();
builder.Services.AddScoped<IPurchaseRepository, PurchaseRepository>();
// Invoice Services
builder.Services.AddScoped<IInvoiceXmlBuilderService, InvoiceXmlBuilderService>();
builder.Services.AddScoped<IElectronicSignatureService, ElectronicSignatureService>();
builder.Services.AddScoped<IAesEncryptionService, AesEncryptionService>();
builder.Services.AddScoped<IInvoicePdfGeneratorService, InvoicePdfGeneratorService>();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IInvoiceValidationService, InvoiceValidationService>();
builder.Services.AddScoped<IInvoiceStockService, InvoiceStockService>();
builder.Services.AddScoped<IInvoiceCalculationService, InvoiceCalculationService>();
builder.Services.AddScoped<IInvoiceSequentialService, InvoiceSequentialService>();
builder.Services.AddScoped<IInvoiceEditionService, InvoiceEditionService>();
builder.Services.AddScoped<IInvoiceDtoFactory, InvoiceDtoFactory>();
builder.Services.AddScoped<ISriSignService, SriSignService>();
builder.Services.AddHttpClient<ISriReceptionService, SriReceptionService>();
// Accounts Receivable Services
builder.Services.AddScoped<IAccountsReceivableService, AccountsReceivableService>();
builder.Services.AddScoped<IARDtoFactory, ARDtoFactory>();
// Kardex Services
builder.Services.AddScoped<IKardexService, KardexService>();
// Purchase Services
builder.Services.AddScoped<IPurchaseValidationService, PurchaseValidationService>();
builder.Services.AddScoped<IPurchaseCalculationService, PurchaseCalculationService>();
builder.Services.AddScoped<IPurchaseEditionService, PurchaseEditionService>();
// Backgroud Services
builder.Services.AddHostedService<SriAuthorizationBackgroundService>();
builder.Services.AddHostedService<SriReceptionBackgroundService>();

var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
    };

    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = "Error de autorización",
                error = "No autorizado o token inválido",
                data = (object?)null,
                pagination = (object?)null
            });

            return context.Response.WriteAsync(result);
        },
        OnForbidden = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = "Acceso prohibido",
                error = "No tienes los permisos correspondientes",
                data = (object?)null,
                pagination = (object?)null
            });

            return context.Response.WriteAsync(result);
        }
    };
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.MapGet("/", () => Results.Json(new
{
    success = true,
    message = "API funcionando correctamente",
    serverTime = DateTime.UtcNow,
    environment = app.Environment.EnvironmentName
}));

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
