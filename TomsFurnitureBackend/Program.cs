using Microsoft.EntityFrameworkCore;
using System;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services;
using TomsFurnitureBackend.Services.IServices;

var builder = WebApplication.CreateBuilder(args);

// --[Add services to the container.]-----------------------------------------
// 1. CORS để phân quyền truy cập api:
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// 2. Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TomfurnitureContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddScoped<IUnitService, UnitService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
