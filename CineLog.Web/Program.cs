using CineLog.Core.Interfaces;
using CineLog.Data.Context;
using CineLog.Data.Repositories;
using CineLog.Service.Services;
using Microsoft.EntityFrameworkCore;
using CineLog.Core.Entities;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon"),
        b => b.MigrationsAssembly("CineLog.Data"));

});

//IDENTITY AYARLARI
builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    // Þifre kurallarý (Öðrenirken iþi zorlaþtýrmasýn diye basitleþtiriyoruz)
    options.Password.RequireDigit = false;           // Rakam zorunlu deðil
    options.Password.RequireLowercase = false;       // Küçük harf zorunlu deðil
    options.Password.RequireUppercase = false;       // Büyük harf zorunlu deðil
    options.Password.RequireNonAlphanumeric = false; // Sembol (!, *, ?) zorunlu deðil
    options.Password.RequiredLength = 3;             // En az 3 karakter olsun yeter
})
.AddEntityFrameworkStores<AppDbContext>() // Veriler AppDbContext üzerinden SQL'e gidecek
.AddDefaultTokenProviders(); // Þifre sýfýrlama tokenlarý için gerekli

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IService<>), typeof(Service<>));

builder.Services.AddHttpClient();

builder.Services.AddScoped<IImdbService, ImdbService>();

builder.Services.AddScoped<IService<UserMovie>, Service<UserMovie>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
