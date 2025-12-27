using Microsoft.EntityFrameworkCore;
using Core.Entities;

namespace Infrastructure.Data;

public class StoreContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Business> Businesses { get; set; }
    public DbSet<Establishment> Establishments { get; set; }
    public DbSet<EmissionPoint> EmissionPoints { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<UserBusiness> UserBusinesses { get; set; }
    public DbSet<UserEstablishment> UserEstablishments { get; set; }
    public DbSet<UserEmissionPoint> UserEmissionPoints { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Tax> Taxes { get; set; }
    public DbSet<UnitMeasure> UnitMeasures { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<ProductWarehouse> ProductWarehouses { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Purchase> Purchases { get; set; }
    public DbSet<PurchaseDetail> PurchaseDetails { get; set; }
    public DbSet<AccountsReceivable> AccountsReceivables { get; set; }
    public DbSet<ARTransaction> ARTransactions { get; set; }
    public DbSet<Kardex> Kardexes { get; set; }
    public DbSet<BusinessCertificate> BusinessCertificates => Set<BusinessCertificate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StoreContext).Assembly);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Usuario");
            entity.Property(u => u.Document).HasColumnName("Documento");
            entity.Property(u => u.FullName).HasColumnName("NombreCompleto");
            entity.Property(u => u.Email).HasColumnName("Correo");
            entity.Property(u => u.Username).HasColumnName("NombreUsuario");
            entity.Property(u => u.Password).HasColumnName("Clave");
            entity.Property(u => u.Address).HasColumnName("Direccion");
            entity.Property(u => u.Cellphone).HasColumnName("Celular");
            entity.Property(u => u.Telephone).HasColumnName("Telefono");
            entity.Property(u => u.Sequence).HasColumnName("Secuencia");
            entity.Property(u => u.ImageUrl).HasColumnName("ImagenUrl");
            entity.Property(u => u.CompanyName).HasColumnName("RazonSocial");
            entity.Property(u => u.IsActive).HasColumnName("Activo");
            entity.Property(u => u.CreatedAt).HasColumnName("FechaCreado");
            entity.Property(u => u.UpdatedAt).HasColumnName("FechaActualizado");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Rol");
            entity.Property(r => r.Name).HasColumnName("Nombre");
        });

        modelBuilder.Entity<Business>(entity =>
        {
            entity.ToTable("Empresa");
            entity.Property(b => b.Document).HasColumnName("Documento");
            entity.Property(b => b.Name).HasColumnName("Nombre");
            entity.Property(b => b.Address).HasColumnName("Direccion");
            entity.Property(b => b.City).HasColumnName("Ciudad");
            entity.Property(b => b.Province).HasColumnName("Provincia");
            entity.Property(b => b.IsActive).HasColumnName("Activo");
            entity.Property(b => b.SriEnvironment).HasColumnName("AmbienteSri");
            entity.Property(b => b.CreatedAt).HasColumnName("FechaCreado");
            entity.Property(b => b.UpdatedAt).HasColumnName("FechaActualizado");
        });

        modelBuilder.Entity<Establishment>(entity =>
        {
            entity.ToTable("Establecimiento");
            entity.Property(e => e.Code).HasColumnName("Codigo");
            entity.Property(e => e.Name).HasColumnName("Nombre");
            entity.Property(e => e.IsActive).HasColumnName("Activo");
            entity.Property(e => e.BusinessId).HasColumnName("EmpresaId");
            entity.Property(e => e.CreatedAt).HasColumnName("FechaCreado");
            entity.Property(e => e.UpdatedAt).HasColumnName("FechaActualizado");

            entity.HasOne(e => e.Business)
            .WithMany(e => e.Establishments)
            .HasForeignKey(e => e.BusinessId);
        });

        modelBuilder.Entity<EmissionPoint>(entity =>
        {
            entity.ToTable("PuntoEmision");
            entity.Property(e => e.Code).HasColumnName("Codigo");
            entity.Property(e => e.Description).HasColumnName("Descripcion");
            entity.Property(e => e.IsActive).HasColumnName("Activo");
            entity.Property(e => e.EstablishmentId).HasColumnName("EstablecimientoId");
            entity.Property(e => e.CreatedAt).HasColumnName("FechaCreado");
            entity.Property(e => e.UpdatedAt).HasColumnName("FechaActualizado");

            entity.HasOne(pe => pe.Establishment)
            .WithMany(pe => pe.EmissionPoints)
            .HasForeignKey(pe => pe.EstablishmentId);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("UsuarioRol");
            entity.Property(ur => ur.UserId).HasColumnName("UsuarioId");
            entity.Property(ur => ur.RoleId).HasColumnName("RolId");

            entity.HasOne(ur => ur.User)
            .WithMany(ur => ur.UserRole)
            .HasForeignKey(ur => ur.UserId);

            entity.HasOne(ur => ur.Role)
            .WithMany(ur => ur.UserRole)
            .HasForeignKey(ur => ur.RoleId);
        });

        modelBuilder.Entity<UserBusiness>(entity =>
        {
            entity.ToTable("UsuarioEmpresa");
            entity.Property(ub => ub.UserId).HasColumnName("UsuarioId");
            entity.Property(ub => ub.BusinessId).HasColumnName("EmpresaId");

            entity.HasOne(ub => ub.User)
            .WithMany(ub => ub.UserBusiness)
            .HasForeignKey(ub => ub.UserId);

            entity.HasOne(ub => ub.Business)
            .WithMany(ub => ub.UserBusiness)
            .HasForeignKey(ub => ub.BusinessId);
        });

        modelBuilder.Entity<UserEstablishment>(entity =>
        {
            entity.ToTable("UsuarioEstablecimiento");
            entity.Property(ue => ue.UserId).HasColumnName("UsuarioId");
            entity.Property(ue => ue.EstablishmentId).HasColumnName("EstablecimientoId");

            entity.HasOne(ue => ue.User)
            .WithMany(ue => ue.UserEstablishment)
            .HasForeignKey(ue => ue.UserId);

            entity.HasOne(ue => ue.Establishment)
            .WithMany(ue => ue.UserEstablishments)
            .HasForeignKey(ue => ue.EstablishmentId);
        });

        modelBuilder.Entity<UserEmissionPoint>(entity =>
        {
            entity.ToTable("UsuarioPuntoEmision");
            entity.Property(uep => uep.UserId).HasColumnName("UsuarioId");
            entity.Property(uep => uep.EmissionPointId).HasColumnName("PuntoEmisionId");

            entity.HasOne(uep => uep.User)
            .WithMany(uep => uep.UserEmissionPoint)
            .HasForeignKey(uep => uep.UserId);

            entity.HasOne(uep => uep.EmissionPoint)
            .WithMany(uep => uep.UserEmissionPoints)
            .HasForeignKey(uep => uep.EmissionPointId);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Producto");
            entity.Property(p => p.Sku).HasColumnName("Sku");
            entity.Property(p => p.Name).HasColumnName("Nombre");
            entity.Property(p => p.Description).HasColumnName("Descripcion");
            entity.Property(p => p.Price).HasColumnName("Precio");
            entity.Property(p => p.Iva).HasColumnName("ConIva");
            entity.Property(p => p.IsActive).HasColumnName("Activo");
            entity.Property(p => p.BusinessId).HasColumnName("EmpresaId");
            entity.Property(p => p.UnitMeasureId).HasColumnName("UnidadMedidaId");
            entity.Property(p => p.Type).HasColumnName("Tipo");
            entity.Property(p => p.TaxId).HasColumnName("ImpuestoId");
            entity.Property(p => p.NetWeight).HasColumnName("PesoNeto");
            entity.Property(p => p.GrossWeight).HasColumnName("PesoBruto");

            entity.HasOne(p => p.Business)
            .WithMany(p => p.Products)
            .HasForeignKey(p => p.BusinessId);

            entity.HasOne(p => p.UnitMeasure)
            .WithMany(p => p.Products)
            .HasForeignKey(p => p.UnitMeasureId);

            entity.HasOne(p => p.Tax)
            .WithMany(p => p.Products)
            .HasForeignKey(p => p.TaxId);
        });

        modelBuilder.Entity<Tax>(entity =>
        {
            entity.ToTable("Impuesto");
            entity.Property(t => t.Code).HasColumnName("Codigo");
            entity.Property(t => t.CodePercentage).HasColumnName("CodigoPorcentaje");
            entity.Property(t => t.Name).HasColumnName("Nombre");
            entity.Property(t => t.Group).HasColumnName("Grupo");
            entity.Property(t => t.Rate).HasColumnName("Tasa");
            entity.Property(t => t.IsActive).HasColumnName("Activo");
            entity.Property(t => t.BusinessId).HasColumnName("EmpresaId");

            entity.HasOne(t => t.Business)
            .WithMany(t => t.Taxes)
            .HasForeignKey(t => t.BusinessId);

        });

        modelBuilder.Entity<UnitMeasure>(entity =>
        {
            entity.ToTable("UnidadMedida");
            entity.Property(um => um.Code).HasColumnName("Codigo");
            entity.Property(um => um.Name).HasColumnName("Nombre");
            entity.Property(um => um.FactorBase).HasColumnName("FactorBase");
            entity.Property(um => um.IsActive).HasColumnName("Activo");
            entity.Property(um => um.BusinessId).HasColumnName("EmpresaId");

            entity.HasOne(um => um.Business)
            .WithMany(um => um.UnitMeasures)
            .HasForeignKey(um => um.BusinessId);
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.ToTable("Bodega");
            entity.Property(w => w.Code).HasColumnName("Codigo");
            entity.Property(w => w.Name).HasColumnName("Nombre");
            entity.Property(w => w.Address).HasColumnName("Direccion");
            entity.Property(w => w.IsActive).HasColumnName("Activo");
            entity.Property(w => w.IsMain).HasColumnName("Principal");
            entity.Property(w => w.BusinessId).HasColumnName("EmpresaId");

            entity.HasOne(w => w.Business)
            .WithMany(w => w.Warehouses)
            .HasForeignKey(w => w.BusinessId);
        });

        modelBuilder.Entity<ProductWarehouse>(entity =>
        {
            entity.ToTable("ProductoBodega");
            entity.Property(pw => pw.Stock).HasColumnName("Cantidad");
            entity.Property(pw => pw.MinStock).HasColumnName("CantidadMinima");
            entity.Property(pw => pw.MaxStock).HasColumnName("CantidadMaxima");
            entity.Property(pw => pw.ProductId).HasColumnName("ProductoId");
            entity.Property(pw => pw.WarehouseId).HasColumnName("BodegaId");

            entity.HasOne(pw => pw.Product)
            .WithMany(pw => pw.ProductWarehouses)
            .HasForeignKey(pw => pw.ProductId);

            entity.HasOne(pw => pw.Warehouse)
            .WithMany(pw => pw.ProductWarehouses)
            .HasForeignKey(pw => pw.WarehouseId);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Cliente");
            entity.Property(c => c.Document).HasColumnName("Documento");
            entity.Property(c => c.DocumentType).HasColumnName("TipoDocumento");
            entity.Property(c => c.Name).HasColumnName("Nombre");
            entity.Property(c => c.Email).HasColumnName("Correo");
            entity.Property(c => c.Address).HasColumnName("Direccion");
            entity.Property(c => c.Cellphone).HasColumnName("Celular");
            entity.Property(c => c.Telephone).HasColumnName("Telefono");
            entity.Property(c => c.IsActive).HasColumnName("Activo");
            entity.Property(c => c.CreatedAt).HasColumnName("FechaCreado");
            entity.Property(c => c.UpdatedAt).HasColumnName("FechaActualizado");
            entity.Property(c => c.BusinessId).HasColumnName("EmpresaId");

            entity.HasOne(c => c.Business)
            .WithMany(c => c.Customers)
            .HasForeignKey(c => c.BusinessId);
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.ToTable("Factura");
            entity.Property(i => i.Sequential).HasColumnName("Secuencial");
            entity.Property(i => i.AccessKey).HasColumnName("ClaveAcceso");
            entity.Property(i => i.Environment).HasColumnName("Ambiente");
            entity.Property(i => i.ReceiptType).HasColumnName("TipoRecibo");
            entity.Property(i => i.Status).HasColumnName("Estado");
            entity.Property(i => i.IsElectronic).HasColumnName("Electronico");
            entity.Property(i => i.InvoiceDate).HasColumnName("FechaFactura").HasColumnType("timestamp without time zone");
            entity.Property(i => i.AuthorizationDate).HasColumnName("FechaAutorizacion").HasColumnType("timestamp without time zone");
            entity.Property(i => i.UserId).HasColumnName("UsuarioId");
            entity.Property(i => i.CustomerId).HasColumnName("ClienteId");
            entity.Property(i => i.BusinessId).HasColumnName("EmpresaId");
            entity.Property(i => i.EstablishmentId).HasColumnName("EstablecimientoId");
            entity.Property(i => i.EmissionPointId).HasColumnName("PuntoEmisionId");
            entity.Property(i => i.SubtotalWithoutTaxes).HasColumnName("SubtotalBase");
            entity.Property(i => i.SubtotalWithTaxes).HasColumnName("Subtotal");
            entity.Property(i => i.DiscountTotal).HasColumnName("TotalDescuento");
            entity.Property(i => i.TaxTotal).HasColumnName("TotalImpuesto");
            entity.Property(i => i.TotalInvoice).HasColumnName("Total");
            entity.Property(i => i.PaymentMethod).HasColumnName("MetodoPago");
            entity.Property(i => i.PaymentTermDays).HasColumnName("DiasPago");
            entity.Property(i => i.DueDate).HasColumnName("FechaVencimiento").HasColumnType("timestamp without time zone");
            entity.Property(i => i.Description).HasColumnName("Descripcion");
            entity.Property(i => i.AdditionalInformation).HasColumnName("InformacionAdicional");
            entity.Property(i => i.XmlSigned).HasColumnName("XmlFirmado");
            entity.Property(i => i.AuthorizationNumber).HasColumnName("NumeroAutorizacion");
            entity.Property(i => i.SriMessage).HasColumnName("MensajeSri");

            entity.HasOne(i => i.User)
            .WithMany(i => i.Invoices)
            .HasForeignKey(i => i.UserId);

            entity.HasOne(i => i.Customer)
            .WithMany(i => i.Invoices)
            .HasForeignKey(i => i.CustomerId);

            entity.HasOne(i => i.Business)
            .WithMany(i => i.Invoices)
            .HasForeignKey(i => i.BusinessId);

            entity.HasOne(i => i.Establishment)
            .WithMany(i => i.Invoices)
            .HasForeignKey(i => i.EstablishmentId);

            entity.HasOne(i => i.EmissionPoint)
            .WithMany(i => i.Invoices)
            .HasForeignKey(i => i.EmissionPointId);
        });

        modelBuilder.Entity<InvoiceDetail>(entity =>
        {
            entity.ToTable("FacturaDetalle");
            entity.Property(id => id.InvoiceId).HasColumnName("FacturaId");
            entity.Property(id => id.ProductId).HasColumnName("ProductoId");
            entity.Property(id => id.UnitMeasureId).HasColumnName("UnidadMedidaId");
            entity.Property(id => id.WarehouseId).HasColumnName("BodegaId");
            entity.Property(id => id.TaxId).HasColumnName("ImpuestoId");
            entity.Property(id => id.TaxRate).HasColumnName("TasaImpuesto");
            entity.Property(id => id.TaxValue).HasColumnName("ValorImpuesto");
            entity.Property(id => id.Quantity).HasColumnName("Cantidad");
            entity.Property(id => id.NetWeight).HasColumnName("PesoNeto");
            entity.Property(id => id.GrossWeight).HasColumnName("PesoBruto");
            entity.Property(id => id.UnitPrice).HasColumnName("PrecioUnitario");
            entity.Property(id => id.Discount).HasColumnName("Descuento");
            entity.Property(id => id.Subtotal).HasColumnName("Subtotal");
            entity.Property(id => id.Total).HasColumnName("Total");

            entity.HasOne(id => id.Invoice)
            .WithMany(id => id.InvoiceDetails)
            .HasForeignKey(id => id.InvoiceId);

            entity.HasOne(id => id.Product)
            .WithMany(id => id.InvoiceDetails)
            .HasForeignKey(id => id.ProductId);

            entity.HasOne(id => id.UnitMeasure)
            .WithMany(id => id.InvoiceDetails)
            .HasForeignKey(id => id.UnitMeasureId);

            entity.HasOne(id => id.Warehouse)
            .WithMany(id => id.InvoiceDetails)
            .HasForeignKey(id => id.WarehouseId);

            entity.HasOne(id => id.Tax)
            .WithMany(id => id.InvoiceDetails)
            .HasForeignKey(id => id.TaxId);
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.ToTable("Proveedor");
            entity.Property(s => s.BusinessName).HasColumnName("RazonSocial");
            entity.Property(s => s.Document).HasColumnName("Documento");
            entity.Property(s => s.Email).HasColumnName("Correo");
            entity.Property(s => s.Address).HasColumnName("Direccion");
            entity.Property(s => s.Cellphone).HasColumnName("Celular");
            entity.Property(s => s.Telephone).HasColumnName("Telefono");
            entity.Property(s => s.IsActive).HasColumnName("Activo");
            entity.Property(s => s.CreatedAt).HasColumnName("FechaCreado");
            entity.Property(s => s.UpdatedAt).HasColumnName("FechaActualizado");
            entity.Property(s => s.BusinessId).HasColumnName("EmpresaId");

            entity.HasOne(s => s.Business)
            .WithMany(b => b.Suppliers)
            .HasForeignKey(s => s.BusinessId);
        });

        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.ToTable("Compra");
            entity.Property(p => p.BusinessId).HasColumnName("EmpresaId");
            entity.Property(p => p.Environment).HasColumnName("Ambiente");
            entity.Property(p => p.EmissionTypeCode).HasColumnName("TipoEmision");
            entity.Property(p => p.BusinessName).HasColumnName("RazonSocial");
            entity.Property(p => p.Name).HasColumnName("NombreComercial");
            entity.Property(p => p.Document).HasColumnName("Documento");
            entity.Property(p => p.AccessKey).HasColumnName("ClaveAcceso");
            entity.Property(p => p.ReceiptType).HasColumnName("TipoRecibo");
            entity.Property(p => p.EstablishmentCode).HasColumnName("Establecimiento");
            entity.Property(p => p.EmissionPointCode).HasColumnName("PuntoEmision");
            entity.Property(p => p.Sequential).HasColumnName("Secuencial");
            entity.Property(p => p.MainAddress).HasColumnName("DireccionMatriz");
            entity.Property(p => p.IssueDate).HasColumnName("FechaEmision");
            entity.Property(p => p.EstablishmentAddress).HasColumnName("DireccionEstablecimiento");
            entity.Property(p => p.SpecialTaxpayer).HasColumnName("ContribuyenteEspecial");
            entity.Property(p => p.MandatoryAccounting).HasColumnName("ObligadoContabilidad");
            entity.Property(p => p.Sequential).HasColumnName("Secuencial");
            entity.Property(p => p.TypeDocumentSubjectDetained).HasColumnName("TipoIdSujetoRetenido");
            entity.Property(p => p.TypeSubjectDetained).HasColumnName("TipoSujetoRetenido");
            entity.Property(p => p.RelatedParty).HasColumnName("PartidoRelacionado");
            entity.Property(p => p.BusinessNameSubjectDetained).HasColumnName("RazSocSujetoRetenido");
            entity.Property(p => p.DocumentSubjectDetained).HasColumnName("DocumentoSujetoRetenido");
            entity.Property(p => p.FiscalPeriod).HasColumnName("PeriodoFiscal");
            entity.Property(p => p.SupplierId).HasColumnName("ProveedorId");
            entity.Property(p => p.Status).HasColumnName("Estado");
            entity.Property(p => p.IsElectronic).HasColumnName("Electronico");
            entity.Property(p => p.AuthorizationNumber).HasColumnName("NumeroAutorizacion");
            entity.Property(p => p.AuthorizationDate).HasColumnName("FechaAutorizacion");
            entity.Property(p => p.SubtotalWithoutTaxes).HasColumnName("SubtotalBase");
            entity.Property(p => p.SubtotalWithTaxes).HasColumnName("Subtotal");
            entity.Property(p => p.DiscountTotal).HasColumnName("TotalDescuento");
            entity.Property(p => p.TaxTotal).HasColumnName("TotalImpuesto");
            entity.Property(p => p.TotalPurchase).HasColumnName("Total");

            entity.HasOne(p => p.Supplier)
            .WithMany(s => s.Purchases)
            .HasForeignKey(p => p.SupplierId);
        });

        modelBuilder.Entity<PurchaseDetail>(entity =>
        {
            entity.ToTable("CompraDetalle");
            entity.Property(pd => pd.PurchaseId).HasColumnName("CompraId");
            entity.Property(pd => pd.ProductId).HasColumnName("ProductoId");
            entity.Property(pd => pd.WarehouseId).HasColumnName("BodegaId");
            entity.Property(pd => pd.TaxId).HasColumnName("ImpuestoId");
            entity.Property(pd => pd.UnitMeasureId).HasColumnName("UnidadMedidaId");
            entity.Property(pd => pd.TaxRate).HasColumnName("TasaImpuesto");
            entity.Property(pd => pd.TaxValue).HasColumnName("ValorImpuesto");
            entity.Property(pd => pd.Discount).HasColumnName("Descuento");
            entity.Property(pd => pd.Quantity).HasColumnName("Cantidad");
            entity.Property(pd => pd.NetWeight).HasColumnName("PesoNeto");
            entity.Property(pd => pd.GrossWeight).HasColumnName("PesoBruto");
            entity.Property(pd => pd.UnitCost).HasColumnName("CostoUnitario");
            entity.Property(pd => pd.Subtotal).HasColumnName("Subtotal");
            entity.Property(pd => pd.Total).HasColumnName("Total");

            entity.HasOne(pd => pd.Purchase)
            .WithMany(p => p.PurchaseDetails)
            .HasForeignKey(pd => pd.PurchaseId);

            entity.HasOne(pd => pd.Product)
            .WithMany(p => p.PurchaseDetails)
            .HasForeignKey(pd => pd.ProductId);

            entity.HasOne(pd => pd.Warehouse)
            .WithMany(w => w.PurchaseDetails)
            .HasForeignKey(pd => pd.WarehouseId);

            entity.HasOne(pd => pd.Tax)
            .WithMany(t => t.PurchaseDetails)
            .HasForeignKey(pd => pd.TaxId);
        });

        modelBuilder.Entity<Kardex>(entity =>
        {
            entity.ToTable("Kardex");
            entity.Property(k => k.BusinessId).HasColumnName("EmpresaId");
            entity.Property(k => k.ProductId).HasColumnName("ProductoId");
            entity.Property(k => k.WarehouseId).HasColumnName("BodegaId");
            entity.Property(k => k.MovementDate).HasColumnName("FechaMovimiento").HasColumnType("timestamp without time zone");
            entity.Property(k => k.QuantityIn).HasColumnName("CantidadEntrada");
            entity.Property(k => k.QuantityOut).HasColumnName("CantidadSalida");
            entity.Property(k => k.UnitCost).HasColumnName("CostoUnitario");
            entity.Property(k => k.TotalCost).HasColumnName("CostoTotal");
            entity.Property(k => k.MovementType).HasColumnName("TipoMovimiento");
            entity.Property(k => k.Reference).HasColumnName("Referencia");

            entity.HasOne(k => k.Business)
            .WithMany(b => b.Kardexes)
            .HasForeignKey(k => k.BusinessId);

            entity.HasOne(k => k.Product)
            .WithMany(p => p.Kardexes)
            .HasForeignKey(k => k.ProductId);

            entity.HasOne(k => k.Warehouse)
            .WithMany(w => w.Kardexes)
            .HasForeignKey(k => k.WarehouseId);
        });

        modelBuilder.Entity<AccountsReceivable>(entity =>
        {
            entity.ToTable("CuentaPorCobrar");
            entity.Property(a => a.BusinessId).HasColumnName("EmpresaId");
            entity.Property(a => a.CustomerId).HasColumnName("ClienteId");
            entity.Property(a => a.InvoiceId).HasColumnName("FacturaId");
            entity.Property(a => a.IssueDate).HasColumnName("FechaEmision").HasColumnType("timestamp without time zone");
            entity.Property(a => a.DueDate).HasColumnName("FechaVencimiento").HasColumnType("timestamp without time zone");
            entity.Property(a => a.ExpectedPaymentDate).HasColumnName("FechaPagoEsperado").HasColumnType("timestamp without time zone");
            entity.Property(a => a.OriginalAmount).HasColumnName("MontoOriginal");
            entity.Property(a => a.Balance).HasColumnName("Saldo");
            entity.Property(a => a.Status).HasColumnName("Estado");

            entity.HasOne(a => a.Business)
            .WithMany(b => b.AccountsReceivables)
            .HasForeignKey(a => a.BusinessId);

            entity.HasOne(a => a.Customer)
            .WithMany(c => c.AccountsReceivables)
            .HasForeignKey(a => a.CustomerId);

            entity.HasOne(a => a.Invoice)
            .WithOne(i => i.AccountsReceivable)
            .HasForeignKey<AccountsReceivable>(a => a.InvoiceId);

            entity.HasMany(a => a.Transactions)
            .WithOne(t => t.AccountsReceivable)
            .HasForeignKey(t => t.AccountReceivableId)
            .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(a => new { a.BusinessId, a.InvoiceId })
            .IsUnique()
            .HasDatabaseName("UX_CuentaPorCobrar_Empresa_Factura");
        });

        modelBuilder.Entity<ARTransaction>(entity =>
        {
            entity.ToTable("CuentaPorCobrarTransaccion");
            entity.Property(t => t.AccountReceivableId).HasColumnName("CuentaPorCobrarId");
            entity.Property(t => t.ARTransactionType).HasColumnName("Tipo");
            entity.Property(t => t.Amount).HasColumnName("Monto");
            entity.Property(t => t.PaymentMethod).HasColumnName("MetodoPago");
            entity.Property(t => t.Reference).HasColumnName("Referencia");
            entity.Property(t => t.PaymentDetails).HasColumnName("DetallesPago");
            entity.Property(t => t.Notes).HasColumnName("Notas");
            entity.Property(t => t.CreatedAt).HasColumnName("FechaCreado");
            entity.Property(t => t.UpdatedAt).HasColumnName("FechaActualizado");

            entity.HasOne(t => t.AccountsReceivable)
            .WithMany(a => a.Transactions)
            .HasForeignKey(t => t.AccountReceivableId);
        });

        modelBuilder.Entity<BusinessCertificate>(entity =>
        {
            entity.ToTable("CertificadoEmpresa");
            entity.Property(bc => bc.BusinessId).HasColumnName("EmpresaId");
            entity.Property(bc => bc.CertificateBase64).HasColumnName("CertificadoBase64");
            entity.Property(bc => bc.Password).HasColumnName("CertificadoClave");
            entity.Property(bc => bc.CreatedAt).HasColumnName("FechaCreacion");

            entity.HasOne(bc => bc.Business)
            .WithOne(bc => bc.BusinessCertificate)
            .HasForeignKey<BusinessCertificate>(bc => bc.BusinessId);
        });

        base.OnModelCreating(modelBuilder);
    }
}
