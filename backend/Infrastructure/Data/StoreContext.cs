using Core.Entities;
using Microsoft.EntityFrameworkCore;

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
    public DbSet<InvoiceDetails> InvoiceDetails { get; set; }
    public DbSet<InvoiceTaxTotal> InvoiceTaxTotals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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
            entity.Property(c => c.Name).HasColumnName("Nombre");
            entity.Property(c => c.Email).HasColumnName("Correo");
            entity.Property(c => c.Address).HasColumnName("Direccion");
            entity.Property(c => c.Cellphone).HasColumnName("Celular");
            entity.Property(c => c.Telephone).HasColumnName("Telefono");
            entity.Property(c => c.IsActive).HasColumnName("Activo");
            entity.Property(c => c.CreatedAt).HasColumnName("FechaCreado");
            entity.Property(c => c.UpdatedAt).HasColumnName("FechaActualizado");
            entity.Property(c => c.DocumentTypeId).HasColumnName("TipoDocumentoId");
            entity.Property(c => c.BusinessId).HasColumnName("EmpresaId");

            entity.HasOne(c => c.DocumentType)
            .WithMany(c => c.Customers)
            .HasForeignKey(c => c.DocumentTypeId);

            entity.HasOne(c => c.Business)
            .WithMany(c => c.Customers)
            .HasForeignKey(c => c.BusinessId);
        });

        modelBuilder.Entity<DocumentType>(entity =>
        {
            entity.ToTable("TipoDocumento");
            entity.Property(dt => dt.Code).HasColumnName("Codigo");
            entity.Property(dt => dt.Name).HasColumnName("Nombre");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.ToTable("Factura");
            entity.Property(i => i.Sequential).HasColumnName("Sequencial");
            entity.Property(i => i.Subtotal).HasColumnName("Subtotal");
            entity.Property(i => i.DiscountTotal).HasColumnName("TotalDescuento");
            entity.Property(i => i.TotalInvoice).HasColumnName("TotalFactura");
            entity.Property(i => i.Status).HasColumnName("Estado");
            entity.Property(i => i.Description).HasColumnName("Descripcion");
            entity.Property(i => i.InvoiceDate).HasColumnName("FechaFactura");
            entity.Property(i => i.BusinessId).HasColumnName("EmpresaId");
            entity.Property(i => i.EstablishmentId).HasColumnName("EstablecimientoId");
            entity.Property(i => i.EmissionPointId).HasColumnName("PuntoEmisionId");
            entity.Property(i => i.CustomerId).HasColumnName("ClienteId");

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

        modelBuilder.Entity<InvoiceDetails>(entity =>
        {
            entity.ToTable("FacturaDetalle");
            entity.Property(id => id.Description).HasColumnName("Descripcion");
            entity.Property(id => id.Quantity).HasColumnName("Cantidad");
            entity.Property(id => id.UnitPrice).HasColumnName("PrecioUnitario");
            entity.Property(id => id.Discount).HasColumnName("Descuento");
            entity.Property(id => id.Subtotal).HasColumnName("Subtotal");
            entity.Property(id => id.IvaValue).HasColumnName("ValorIva");
            entity.Property(id => id.Total).HasColumnName("Total");
            entity.Property(id => id.TaxRate).HasColumnName("TasaImpuesto");
            entity.Property(id => id.ProductId).HasColumnName("ProductoId");
            entity.Property(id => id.InvoiceId).HasColumnName("FacturaId");

            entity.HasOne(id => id.Product)
            .WithMany(id => id.InvoiceDetails)
            .HasForeignKey(id => id.ProductId);

            entity.HasOne(id => id.Invoice)
            .WithMany(id => id.InvoiceDetails)
            .HasForeignKey(id => id.InvoiceId);
        });

        modelBuilder.Entity<InvoiceTaxTotal>(entity =>
        {
            entity.ToTable("FacturaImpuestoTotal");
            entity.Property(itt => itt.TaxRate).HasColumnName("TasaImpuesto");
            entity.Property(itt => itt.TaxableBase).HasColumnName("BaseImponible");
            entity.Property(itt => itt.TaxValue).HasColumnName("ValorImpuesto");
            entity.Property(itt => itt.TaxCode).HasColumnName("CodigoImpuesto");
            entity.Property(itt => itt.InvoiceId).HasColumnName("FacturaId");

            entity.HasOne(itt => itt.Invoice)
            .WithMany(itt => itt.InvoiceTaxTotals)
            .HasForeignKey(itt => itt.InvoiceId);
        });
    }
}
