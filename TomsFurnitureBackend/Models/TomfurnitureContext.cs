using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TomsFurnitureBackend.Models;

public partial class TomfurnitureContext : DbContext
{
    public TomfurnitureContext()
    {
    }

    public TomfurnitureContext(DbContextOptions<TomfurnitureContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Banner> Banners { get; set; }
    public virtual DbSet<Brand> Brands { get; set; }
    public virtual DbSet<Cart> Carts { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Color> Colors { get; set; }
    public virtual DbSet<Comment> Comments { get; set; }
    public virtual DbSet<Country> Countries { get; set; }
    public virtual DbSet<Feedback> Feedbacks { get; set; }
    public virtual DbSet<ImageProductReview> ImageProductReviews { get; set; }
    public virtual DbSet<Material> Materials { get; set; }
    public virtual DbSet<News> News { get; set; }
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<OrderAddress> OrderAddresses { get; set; }
    public virtual DbSet<OrderDetail> OrderDetails { get; set; }
    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }
    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<ProductReview> ProductReviews { get; set; }
    public virtual DbSet<ProductVariant> ProductVariants { get; set; }
    public virtual DbSet<Promotion> Promotions { get; set; }
    public virtual DbSet<PromotionType> PromotionTypes { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<Shipping> Shippings { get; set; }
    public virtual DbSet<Size> Sizes { get; set; }
    public virtual DbSet<Slider> Sliders { get; set; }
    public virtual DbSet<StoreInformation> StoreInformations { get; set; }
    public virtual DbSet<Supplier> Suppliers { get; set; }
    public virtual DbSet<Unit> Units { get; set; }
    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Chuỗi kết nối đã được cấu hình trong Program.cs qua DI, không cần hardcode ở đây
        // Nếu không có cấu hình từ DI, sẽ sử dụng chuỗi kết nối mặc định (dùng trong môi trường dev/test)
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Data Source=ASUS;Initial Catalog=TOMFURNITURE;Integrated Security=True;Encrypt=True;TrustServerCertificate=True");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Banner>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Banners__3214EC079E29752A");
            entity.HasIndex(e => e.Title, "UQ__Banners__2CB664DC27EC1B98").IsUnique();
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.DisplayOrder).HasDefaultValue(0);
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.ImageUrl).HasMaxLength(255).HasColumnName("ImageURL");
            entity.Property(e => e.LinkUrl).HasMaxLength(255).HasColumnName("LinkURL");
            entity.Property(e => e.Position).HasMaxLength(50);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.User)
                .WithMany(p => p.Banners)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull) // Nếu User bị xóa, đặt UserId về NULL
                .HasConstraintName("FK__Banners__UserId__68487DD7");
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Brands__3214EC07ADD9AD4D");
            entity.Property(e => e.BrandName).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ImageUrl).HasMaxLength(255).HasColumnName("ImageURL");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cart__3214EC0754A64DDF");
            entity.ToTable("Cart");
            entity.HasIndex(e => new { e.ProVarId, e.UserId }, "UQ__Cart__54877F19D9B546AB").IsUnique();
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ProVarId).HasColumnName("ProVarID");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.ProVar)
                .WithMany(p => p.Carts)
                .HasForeignKey(d => d.ProVarId)
                .OnDelete(DeleteBehavior.Cascade) // Xóa Cart khi ProductVariant bị xóa
                .HasConstraintName("FK__Cart__ProVarID__1EA48E88");

            entity.HasOne(d => d.User)
                .WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade) // Xóa Cart khi User bị xóa
                .HasConstraintName("FK__Cart__UserID__1F98B2C1");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC07D5A17EB8");
            entity.Property(e => e.CategoryName).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Descriptions).HasMaxLength(255);
            entity.Property(e => e.ImageUrl).HasMaxLength(255).HasColumnName("ImageURL");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Color>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Colors__3214EC0703B253C9");
            entity.HasIndex(e => e.ColorName, "UQ__Colors__C71A5A7B10E5DA5D").IsUnique();
            entity.Property(e => e.ColorCode).HasMaxLength(100);
            entity.Property(e => e.ColorName).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Comment__3214EC0766959CCE");
            entity.ToTable("Comment");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ProReviewId).HasColumnName("ProReviewID");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.ProReview)
                .WithMany(p => p.Comments)
                .HasForeignKey(d => d.ProReviewId)
                .OnDelete(DeleteBehavior.Cascade) // Xóa Comment khi ProductReview bị xóa
                .HasConstraintName("FK__Comment__ProRevi__7A672E12");

            entity.HasOne(d => d.User)
                .WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull) // Đặt UserId về NULL khi User bị xóa
                .HasConstraintName("FK__Comment__UserId__797309D9");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Countrie__3214EC072229E15E");
            entity.Property(e => e.CountryName).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ImageUrl).HasMaxLength(255).HasColumnName("ImageURL");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Feedback__3214EC0736862A09");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Message).HasMaxLength(255);
            entity.Property(e => e.ParentFeedbackId).HasColumnName("ParentFeedbackID");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.ParentFeedback)
                .WithMany(p => p.InverseParentFeedback)
                .HasForeignKey(d => d.ParentFeedbackId)
                .OnDelete(DeleteBehavior.NoAction) // Không xóa Feedback con khi xóa Feedback cha
                .HasConstraintName("FK__Feedbacks__Paren__236943A5");

            entity.HasOne(d => d.User)
                .WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull) // Đặt UserId về NULL khi User bị xóa
                .HasConstraintName("FK__Feedbacks__UserI__22751F6C");
        });

        modelBuilder.Entity<ImageProductReview>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ImagePro__3214EC07082D781D");
            entity.Property(e => e.Attribute).HasMaxLength(255);
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ImageUrl).HasMaxLength(255).HasColumnName("ImageURL");
            entity.Property(e => e.ProductReviewId).HasColumnName("ProductReviewID");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.ProductReview)
                .WithMany(p => p.ImageProductReviews)
                .HasForeignKey(d => d.ProductReviewId)
                .OnDelete(DeleteBehavior.Cascade) // Xóa Image khi ProductReview bị xóa
                .HasConstraintName("FK__ImageProd__Produ__75A278F5");
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Material__3214EC071D80A998");
            entity.HasIndex(e => e.MaterialName, "UQ__Material__9C87053C2831A2CD").IsUnique();
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.MaterialName).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<News>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__News__3214EC07FF58C126");
            entity.HasIndex(e => e.Title, "UQ__News__2CB664DCB8153AC0").IsUnique();
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.NewsAvatar).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.User)
                .WithMany(p => p.News)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull) // Đặt UserId về NULL khi User bị xóa
                .HasConstraintName("FK__News__UserId__3F466844");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Orders__3214EC0714A930E2");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.OrderAddId).HasColumnName("OrderAddID");
            entity.Property(e => e.OrderDate).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
            entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");
            entity.Property(e => e.OrderStaId).HasColumnName("OrderStaID");
            entity.Property(e => e.PaymentMethodId).HasColumnName("PaymentMethodID");
            entity.Property(e => e.PriceDiscount).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.PromotionId).HasColumnName("PromotionID");
            entity.Property(e => e.ShippingPrice).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.Total).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.OrderAdd)
                .WithMany(p => p.Orders)
                .HasForeignKey(d => d.OrderAddId)
                .OnDelete(DeleteBehavior.SetNull) // Đặt OrderAddId về NULL khi OrderAddress bị xóa
                .HasConstraintName("FK__Orders__OrderAdd__14270015");

            entity.HasOne(d => d.OrderSta)
                .WithMany(p => p.Orders)
                .HasForeignKey(d => d.OrderStaId)
                .OnDelete(DeleteBehavior.SetNull) // Đặt OrderStaId về NULL khi OrderStatus bị xóa
                .HasConstraintName("FK__Orders__OrderSta__151B244E");

            entity.HasOne(d => d.PaymentMethod)
                .WithMany(p => p.Orders)
                .HasForeignKey(d => d.PaymentMethodId)
                .OnDelete(DeleteBehavior.SetNull) // Đặt PaymentMethodId về NULL khi PaymentMethod bị xóa
                .HasConstraintName("FK__Orders__PaymentM__160F4887");

            entity.HasOne(d => d.Promotion)
                .WithMany(p => p.Orders)
                .HasForeignKey(d => d.PromotionId)
                .OnDelete(DeleteBehavior.SetNull) // Đặt PromotionId về NULL khi Promotion bị xóa
                .HasConstraintName("FK__Orders__Promotio__17036CC0");

            entity.HasOne(d => d.User)
                .WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull) // Đặt UserId về NULL khi User bị xóa
                .HasConstraintName("FK__Orders__UserID__1332DBDC");
        });

        modelBuilder.Entity<OrderAddress>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderAdd__3214EC070899D2DC");
            entity.ToTable("OrderAddress");
            entity.Property(e => e.AddressDetailRecipient).HasMaxLength(500);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.District).HasMaxLength(100);
            entity.Property(e => e.IsDeafaultAddress).HasDefaultValue(true);
            entity.Property(e => e.PhoneNumber).HasMaxLength(15).IsUnicode(false);
            entity.Property(e => e.Recipient).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Ward).HasMaxLength(100);

            entity.HasOne(d => d.User)
                .WithMany(p => p.OrderAddresses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull) // Đặt UserId về NULL khi User bị xóa
                .HasConstraintName("FK__OrderAddr__UserI__00200768");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderDet__3214EC074C4CC390");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.Price).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.ProVarId).HasColumnName("ProVarID");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Order)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade) // Xóa OrderDetail khi Order bị xóa
                .HasConstraintName("FK__OrderDeta__Order__1AD3FDA4");

            entity.HasOne(d => d.ProVar)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProVarId)
                .OnDelete(DeleteBehavior.NoAction) // Không cho phép xóa ProductVariant nếu có OrderDetail
                .HasConstraintName("FK__OrderDeta__ProVa__19DFD96B");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderSta__3214EC07DFC249EC");
            entity.ToTable("OrderStatus");
            entity.HasIndex(e => e.OrderStatusName, "UQ__OrderSta__837D0BC1D2EC05A9").IsUnique();
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.OrderStatusName).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PaymentM__3214EC0785D434BC");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.NamePaymentMethod).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Products__3214EC078EA55457");
            entity.Property(e => e.BrandId).HasColumnName("BrandID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CountriesId).HasColumnName("CountriesID");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ProductName).HasMaxLength(200);
            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Brand)
                .WithMany(p => p.Products)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.SetNull) // Đặt BrandId về NULL khi Brand bị xóa
                .HasConstraintName("FK__Products__BrandI__571DF1D5");

            entity.HasOne(d => d.Category)
                .WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.SetNull) // Đặt CategoryId về NULL khi Category bị xóa
                .HasConstraintName("FK__Products__Catego__5812160E");

            entity.HasOne(d => d.Countries)
                .WithMany(p => p.Products)
                .HasForeignKey(d => d.CountriesId)
                .OnDelete(DeleteBehavior.SetNull) // Đặt CountriesId về NULL khi Country bị xóa
                .HasConstraintName("FK__Products__Countr__59063A47");

            entity.HasOne(d => d.Supplier)
                .WithMany(p => p.Products)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.SetNull) // Đặt SupplierId về NULL khi Supplier bị xóa
                .HasConstraintName("FK__Products__Suppli__59FA5E80");
        });

        modelBuilder.Entity<ProductReview>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductR__3214EC0725B2E5C1");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ProId).HasColumnName("ProID");
            entity.Property(e => e.RelatedVideo).HasMaxLength(255);
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Pro)
                .WithMany(p => p.ProductReviews)
                .HasForeignKey(d => d.ProId)
                .OnDelete(DeleteBehavior.Cascade) // Xóa ProductReview khi Product bị xóa
                .HasConstraintName("FK__ProductRe__ProID__72C60C4A");

            entity.HasOne(d => d.User)
                .WithMany(p => p.ProductReviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull) // Đặt UserId về NULL khi User bị xóa
                .HasConstraintName("FK__ProductRe__UserI__71D1E811");
        });

        modelBuilder.Entity<ProductVariant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductV__3214EC07E1570EB1");
            entity.ToTable("ProductVariant");
            entity.Property(e => e.ColorId).HasColumnName("ColorID");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DiscountedPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ImageUrl).HasMaxLength(255).HasColumnName("ImageURL");
            entity.Property(e => e.MaterialId).HasColumnName("MaterialID");
            entity.Property(e => e.OriginalPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.SizeId).HasColumnName("SizeID");
            entity.Property(e => e.UnitId).HasColumnName("UnitID");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Color)
                .WithMany(p => p.ProductVariants)
                .HasForeignKey(d => d.ColorId)
                .OnDelete(DeleteBehavior.SetNull) // Đặt ColorId về NULL khi Color bị xóa
                .HasConstraintName("FK__ProductVa__Color__5EBF139D");

            entity.HasOne(d => d.Material)
                .WithMany(p => p.ProductVariants)
                .HasForeignKey(d => d.MaterialId)
                .OnDelete(DeleteBehavior.SetNull) // Đặt MaterialId về NULL khi Material bị xóa
                .HasConstraintName("FK__ProductVa__Mater__60A75C0F");

            entity.HasOne(d => d.Product)
                .WithMany(p => p.ProductVariants)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade) // Xóa ProductVariant khi Product bị xóa
                .HasConstraintName("FK__ProductVa__Produ__5DCAEF64");

            entity.HasOne(d => d.Size)
                .WithMany(p => p.ProductVariants)
                .HasForeignKey(d => d.SizeId)
                .OnDelete(DeleteBehavior.SetNull) // Đặt SizeId về NULL khi Size bị xóa
                .HasConstraintName("FK__ProductVa__SizeI__5FB337D6");

            entity.HasOne(d => d.Unit)
                .WithMany(p => p.ProductVariants)
                .HasForeignKey(d => d.UnitId)
                .OnDelete(DeleteBehavior.SetNull) // Đặt UnitId về NULL khi Unit bị xóa
                .HasConstraintName("FK__ProductVa__UnitI__619B8048");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Promotio__3214EC07667C6850");
            entity.HasIndex(e => e.PromotionCode, "UQ__Promotio__A617E4B652A9ACCD").IsUnique();
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DiscountValue).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.MaximumDiscountAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.OrderMinimum).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PromotionCode).HasMaxLength(20);
            entity.Property(e => e.PromotionTypeId).HasColumnName("PromotionTypeID");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.PromotionType)
                .WithMany(p => p.Promotions)
                .HasForeignKey(d => d.PromotionTypeId)
                .OnDelete(DeleteBehavior.SetNull) // Đặt PromotionTypeId về NULL khi PromotionType bị xóa
                .HasConstraintName("FK__Promotion__Promo__0A9D95DB");
        });

        modelBuilder.Entity<PromotionType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Promotio__3214EC0721CB3E8D");
            entity.HasIndex(e => e.PromotionTypeName, "UQ__Promotio__793B63B5D8D6A930").IsUnique();
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.PromotionTypeName).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC0757D63D86");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.RoleName).HasMaxLength(20).IsUnicode(false);
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Shipping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Shipping__3214EC07C5EB6847");
            entity.ToTable("Shipping");
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.District).HasMaxLength(100);
            entity.Property(e => e.ShippingPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Ward).HasMaxLength(100);

            entity.HasOne(d => d.User)
                .WithMany(p => p.Shippings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull) // Đặt UserId về NULL khi User bị xóa
                .HasConstraintName("FK__Shipping__UserID__0D7A0286");
        });

        modelBuilder.Entity<Size>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Sizes__3214EC0734E31FD4");
            entity.HasIndex(e => e.SizeName, "UQ__Sizes__619EFC3EF8D278AD").IsUnique();
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.SizeName).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Slider>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Sliders__3214EC0751E2FF80");
            entity.HasIndex(e => e.Title, "UQ__Sliders__2CB664DC5BD1B0B4").IsUnique();
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.ImageUrl).HasMaxLength(255).HasColumnName("ImageURL");
            entity.Property(e => e.IsPoster).HasDefaultValue(false);
            entity.Property(e => e.LinkUrl).HasMaxLength(255).HasColumnName("LinkURL");
            entity.Property(e => e.Position).HasMaxLength(50);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Product)
                .WithMany(p => p.Sliders)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade) // Xóa Slider khi Product bị xóa
                .HasConstraintName("FK__Sliders__Product__6E01572D");
        });

        modelBuilder.Entity<StoreInformation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__StoreInf__3214EC0749370112");
            entity.ToTable("StoreInformation");
            entity.Property(e => e.BranchCode).HasMaxLength(20).IsUnicode(false);
            entity.Property(e => e.BusinessType).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Latitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.LinkSocialFacebook).HasMaxLength(255);
            entity.Property(e => e.LinkSocialInstagram).HasMaxLength(255);
            entity.Property(e => e.LinkSocialTiktok).HasMaxLength(255);
            entity.Property(e => e.LinkSocialTwitter).HasMaxLength(255);
            entity.Property(e => e.LinkSocialYoutube).HasMaxLength(255);
            entity.Property(e => e.LinkWebsite).HasMaxLength(255);
            entity.Property(e => e.Logo).HasMaxLength(255);
            entity.Property(e => e.Longitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.OperatingHours).HasMaxLength(100);
            entity.Property(e => e.OwnerName).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.StoreAddress).HasMaxLength(255);
            entity.Property(e => e.StoreDescription).HasMaxLength(500);
            entity.Property(e => e.StoreName).HasMaxLength(100);
            entity.Property(e => e.TaxId).HasMaxLength(20).IsUnicode(false);
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Supplier__3214EC07AE152CFF");
            entity.HasIndex(e => e.Email, "UQ__Supplier__A9D10534A1D312AD").IsUnique();
            entity.Property(e => e.ContactName).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.ImageUrl).HasMaxLength(255).HasColumnName("ImageURL");
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.SupplierName).HasMaxLength(100);
            entity.Property(e => e.TaxId).HasMaxLength(20).IsUnicode(false).HasColumnName("TaxID");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Units__3214EC07728D31B0");
            entity.HasIndex(e => e.UnitName, "UQ__Units__B5EE66781C591100").IsUnique();
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.UnitName).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC0741950FFC");
            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534CD3D7CEE").IsUnique();
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Gender).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(15).IsUnicode(false);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.UserAddress).HasMaxLength(255);
            entity.Property(e => e.UserName).HasMaxLength(100);

            entity.HasOne(d => d.Role)
                .WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.SetNull) // Đặt RoleId về NULL khi Role bị xóa
                .HasConstraintName("FK__Users__RoleID__3B75D760");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}