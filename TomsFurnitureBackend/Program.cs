using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using StoreLinhKien.Services;
using System;
using TomsFurnitureBackend.Helpers;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services;
using TomsFurnitureBackend.Services.Interfaces;
using TomsFurnitureBackend.Services.IServices;

var builder = WebApplication.CreateBuilder(args);

// --[Add services to the container.]-----------------------------------------
// 1. CORS để phân quyền truy cập api:
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy =>
        policy.WithOrigins("http://localhost:5173", "https://tomsfurniture.vercel.app")
        .AllowAnyMethod().AllowAnyHeader().AllowCredentials());
});

// 2. Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TomfurnitureContext>(options =>
    options.UseSqlServer(connectionString));

//3. Services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderStatusService, OrderStatusService>();
builder.Services.AddScoped<IOrderAddressService, OrderAddressService>();
builder.Services.AddScoped<IUserGuestService, UserGuestService>();
builder.Services.AddScoped<IBannerService, BannerService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IPromotionTypeService, PromotionTypeService>();
builder.Services.AddScoped<IPromotionService, PromotionService>();
builder.Services.AddScoped<INewsService, NewsService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IPaymentMethodService, PaymentMethodService>();
builder.Services.AddScoped<IStoreInformationService, StoreInformationService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<ISizeService, SizeService>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IMaterialService, MaterialService>();
builder.Services.AddScoped<IColorService, ColorService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUnitService, UnitService>();
builder.Services.AddScoped<ISliderService, SliderService>();
builder.Services.AddScoped<IProductVariantImageService, ProductVariantImageService>();
builder.Services.AddScoped<IVnPayService, VnPayService>();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 4. Cloudinary
var cloudinarySettings = builder.Configuration.GetSection("CloudinarySettings").Get<CloudinarySettings>();
if (cloudinarySettings == null)
{
    throw new InvalidOperationException("Cloudinary settings are not configured properly.");
}
builder.Services.AddSingleton(new Cloudinary(new Account(
    cloudinarySettings.CloudName ?? throw new InvalidOperationException("CloudName is not configured."),
    cloudinarySettings.ApiKey ?? throw new InvalidOperationException("ApiKey is not configured."),
    cloudinarySettings.ApiSecret ?? throw new InvalidOperationException("ApiSecret is not configured.")
)));

// 5. Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/api/auth/login";
        options.LogoutPath = "/api/auth/logout";
        options.AccessDeniedPath = "/api/auth/accessdenied";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.None;
        // options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Chỉ dùng khi đưa lên hosting
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseCors("AllowAll"); // Đây là nơi áp dung chính sách CORS đã định nghĩa ở trên
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
