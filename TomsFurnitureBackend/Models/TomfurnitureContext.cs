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

    public virtual DbSet<ConfirmOtp> ConfirmOtps { get; set; }

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

    public virtual DbSet<UserGuest> UserGuests { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=ASUS;Initial Catalog=TOMFURNITURE;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Banner>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Banners__3214EC0784B382EB");

            entity.HasIndex(e => e.Title, "UQ__Banners__2CB664DCC72D8B40").IsUnique();

            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.DisplayOrder).HasDefaultValue(0);
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("ImageURL");
            entity.Property(e => e.LinkUrl)
                .HasMaxLength(255)
                .HasColumnName("LinkURL");
            entity.Property(e => e.Position).HasMaxLength(50);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Banners)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Banners__UserId__68487DD7");
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Brands__3214EC0783D4608A");

            entity.Property(e => e.BrandName).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("ImageURL");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cart__3214EC0763F8252E");

            entity.ToTable("Cart");

            entity.HasIndex(e => new { e.ProId, e.UserId }, "UQ__Cart__B37A193B716286C5").IsUnique();

            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ProId).HasColumnName("ProID");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Pro).WithMany(p => p.Carts)
                .HasForeignKey(d => d.ProId)
                .HasConstraintName("FK__Cart__ProID__51300E55");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Cart__UserID__1F98B2C1");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC0765CBC780");

            entity.Property(e => e.CategoryName).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Descriptions).HasMaxLength(255);
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("ImageURL");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Color>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Colors__3214EC07087F387B");

            entity.HasIndex(e => e.ColorName, "UQ__Colors__C71A5A7BCC4AFE6A").IsUnique();

            entity.Property(e => e.ColorCode).HasMaxLength(100);
            entity.Property(e => e.ColorName).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Comment__3214EC0747C1B8B6");

            entity.ToTable("Comment");

            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ProReviewId).HasColumnName("ProReviewID");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.ProReview).WithMany(p => p.Comments)
                .HasForeignKey(d => d.ProReviewId)
                .HasConstraintName("FK__Comment__ProRevi__7A672E12");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Comment__UserId__797309D9");
        });

        modelBuilder.Entity<ConfirmOtp>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ConfirmO__3214EC07DB273D9C");

            entity.ToTable("ConfirmOTP");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ExpiredDate).HasColumnType("datetime");
            entity.Property(e => e.Otpcode)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("OTPCode");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.ConfirmOtps)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ConfirmOT__UserI__3D2915A8");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Countrie__3214EC0744269532");

            entity.Property(e => e.CountryName).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("ImageURL");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Feedback__3214EC07E6D54238");

            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Message).HasMaxLength(255);
            entity.Property(e => e.ParentFeedbackId).HasColumnName("ParentFeedbackID");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.ParentFeedback).WithMany(p => p.InverseParentFeedback)
                .HasForeignKey(d => d.ParentFeedbackId)
                .HasConstraintName("FK__Feedbacks__Paren__236943A5");

            entity.HasOne(d => d.User).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Feedbacks__UserI__22751F6C");
        });

        modelBuilder.Entity<ImageProductReview>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ImagePro__3214EC07668E1880");

            entity.Property(e => e.Attribute).HasMaxLength(255);
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("ImageURL");
            entity.Property(e => e.ProductReviewId).HasColumnName("ProductReviewID");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.ProductReview).WithMany(p => p.ImageProductReviews)
                .HasForeignKey(d => d.ProductReviewId)
                .HasConstraintName("FK__ImageProd__Produ__75A278F5");
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Material__3214EC0764A4B1BF");

            entity.HasIndex(e => e.MaterialName, "UQ__Material__9C87053C2DBE2E36").IsUnique();

            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.MaterialName).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<News>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__News__3214EC073F3791EC");

            entity.HasIndex(e => e.Title, "UQ__News__2CB664DC80BB2873").IsUnique();

            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.NewsAvatar).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.News)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__News__UserId__3F466844");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Orders__3214EC0736702D95");

            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DeliveryDate).HasColumnType("datetime");
            entity.Property(e => e.OrderAddId).HasColumnName("OrderAddID");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");
            entity.Property(e => e.OrderStaId).HasColumnName("OrderStaID");
            entity.Property(e => e.PaymentMethodId).HasColumnName("PaymentMethodID");
            entity.Property(e => e.PriceDiscount).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.PromotionId).HasColumnName("PromotionID");
            entity.Property(e => e.ShippingPrice).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.Total).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.UserGuestId).HasColumnName("UserGuestID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.OrderAdd).WithMany(p => p.Orders)
                .HasForeignKey(d => d.OrderAddId)
                .HasConstraintName("FK__Orders__OrderAdd__14270015");

            entity.HasOne(d => d.OrderSta).WithMany(p => p.Orders)
                .HasForeignKey(d => d.OrderStaId)
                .HasConstraintName("FK__Orders__OrderSta__151B244E");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PaymentMethodId)
                .HasConstraintName("FK__Orders__PaymentM__160F4887");

            entity.HasOne(d => d.Promotion).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PromotionId)
                .HasConstraintName("FK__Orders__Promotio__17036CC0");

            entity.HasOne(d => d.UserGuest).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserGuestId)
                .HasConstraintName("FK_Orders_UserGuest");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Orders__UserID__1332DBDC");
        });

        modelBuilder.Entity<OrderAddress>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderAdd__3214EC07E289BEEA");

            entity.ToTable("OrderAddress");

            entity.Property(e => e.AddressDetailRecipient).HasMaxLength(500);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.District).HasMaxLength(100);
            entity.Property(e => e.IsDeafaultAddress).HasDefaultValue(true);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Recipient).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Ward).HasMaxLength(100);

            entity.HasOne(d => d.User).WithMany(p => p.OrderAddresses)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__OrderAddr__UserI__00200768");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderDet__3214EC07613AC8E8");

            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.Price).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.ProVarId).HasColumnName("ProVarID");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__OrderDeta__Order__1AD3FDA4");

            entity.HasOne(d => d.ProVar).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProVarId)
                .HasConstraintName("FK__OrderDeta__ProVa__19DFD96B");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderSta__3214EC078618A90D");

            entity.ToTable("OrderStatus");

            entity.HasIndex(e => e.OrderStatusName, "UQ__OrderSta__837D0BC16B58C580").IsUnique();

            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.OrderStatusName).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PaymentM__3214EC077E3D3F96");

            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.NamePaymentMethod).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Products__3214EC07A6E158CB");

            entity.Property(e => e.BrandId).HasColumnName("BrandID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CountriesId).HasColumnName("CountriesID");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ProductName).HasMaxLength(200);
            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Brand).WithMany(p => p.Products)
                .HasForeignKey(d => d.BrandId)
                .HasConstraintName("FK__Products__BrandI__571DF1D5");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Products__Catego__5812160E");

            entity.HasOne(d => d.Countries).WithMany(p => p.Products)
                .HasForeignKey(d => d.CountriesId)
                .HasConstraintName("FK__Products__Countr__59063A47");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Products)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("FK__Products__Suppli__59FA5E80");
        });

        modelBuilder.Entity<ProductReview>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductR__3214EC07536DDDA3");

            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ProId).HasColumnName("ProID");
            entity.Property(e => e.RelatedVideo).HasMaxLength(255);
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Pro).WithMany(p => p.ProductReviews)
                .HasForeignKey(d => d.ProId)
                .HasConstraintName("FK__ProductRe__ProID__72C60C4A");

            entity.HasOne(d => d.User).WithMany(p => p.ProductReviews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ProductRe__UserI__71D1E811");
        });

        modelBuilder.Entity<ProductVariant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductV__3214EC07BA51F015");

            entity.ToTable("ProductVariant");

            entity.Property(e => e.ColorId).HasColumnName("ColorID");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DiscountedPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("ImageURL");
            entity.Property(e => e.MaterialId).HasColumnName("MaterialID");
            entity.Property(e => e.OriginalPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.SizeId).HasColumnName("SizeID");
            entity.Property(e => e.UnitId).HasColumnName("UnitID");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Color).WithMany(p => p.ProductVariants)
                .HasForeignKey(d => d.ColorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductVa__Color__5EBF139D");

            entity.HasOne(d => d.Material).WithMany(p => p.ProductVariants)
                .HasForeignKey(d => d.MaterialId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductVa__Mater__60A75C0F");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductVariants)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductVa__Produ__5DCAEF64");

            entity.HasOne(d => d.Size).WithMany(p => p.ProductVariants)
                .HasForeignKey(d => d.SizeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductVa__SizeI__5FB337D6");

            entity.HasOne(d => d.Unit).WithMany(p => p.ProductVariants)
                .HasForeignKey(d => d.UnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductVa__UnitI__619B8048");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Promotio__3214EC07B15FCF7A");

            entity.HasIndex(e => e.PromotionCode, "UQ__Promotio__A617E4B624C68463").IsUnique();

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

            entity.HasOne(d => d.PromotionType).WithMany(p => p.Promotions)
                .HasForeignKey(d => d.PromotionTypeId)
                .HasConstraintName("FK__Promotion__Promo__0A9D95DB");
        });

        modelBuilder.Entity<PromotionType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Promotio__3214EC0700C64591");

            entity.HasIndex(e => e.PromotionTypeName, "UQ__Promotio__793B63B5DC8BB176").IsUnique();

            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.PromotionTypeName).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07B988247D");

            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.RoleName)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Shipping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Shipping__3214EC07E194EF48");

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

            entity.HasOne(d => d.User).WithMany(p => p.Shippings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Shipping__UserID__0D7A0286");
        });

        modelBuilder.Entity<Size>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Sizes__3214EC07B24CF567");

            entity.HasIndex(e => e.SizeName, "UQ__Sizes__619EFC3E40EF8F9B").IsUnique();

            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.SizeName).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Slider>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Sliders__3214EC07AC036113");

            entity.HasIndex(e => e.Title, "UQ__Sliders__2CB664DCD229C1C5").IsUnique();

            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("ImageURL");
            entity.Property(e => e.IsPoster).HasDefaultValue(false);
            entity.Property(e => e.LinkUrl)
                .HasMaxLength(255)
                .HasColumnName("LinkURL");
            entity.Property(e => e.Position).HasMaxLength(50);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Product).WithMany(p => p.Sliders)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Sliders__Product__6E01572D");
        });

        modelBuilder.Entity<StoreInformation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__StoreInf__3214EC07E8ABF2A3");

            entity.ToTable("StoreInformation");

            entity.Property(e => e.BranchCode)
                .HasMaxLength(20)
                .IsUnicode(false);
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
            entity.Property(e => e.TaxId)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Supplier__3214EC07F60801CE");

            entity.HasIndex(e => e.Email, "UQ__Supplier__A9D105344265C7EB").IsUnique();

            entity.Property(e => e.ContactName).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("ImageURL");
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.SupplierName).HasMaxLength(100);
            entity.Property(e => e.TaxId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("TaxID");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Units__3214EC071C48C166");

            entity.HasIndex(e => e.UnitName, "UQ__Units__B5EE6678A8849DBC").IsUnique();

            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.UnitName).HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC0701DD3DA9");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534CF7550B8").IsUnique();

            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Gender).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.UserAddress).HasMaxLength(255);
            entity.Property(e => e.UserName).HasMaxLength(100);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__RoleID__3B75D760");
        });

        modelBuilder.Entity<UserGuest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserGues__3214EC071FCDC0C3");

            entity.ToTable("UserGuest");

            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DetailAddress).HasMaxLength(255);
            entity.Property(e => e.District).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Ward).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
