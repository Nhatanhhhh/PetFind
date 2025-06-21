using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PetStore.DAO;
using PetStore.DAO.Interfaces;
using PetStore.Data.Context;
using PetStore.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add HttpClient for FileService
builder.Services.AddHttpClient();

// Add FileService
builder.Services.AddScoped<IFileService, FileService>();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Add application DAOs
builder.Services.AddScoped<IUserDAO, UserDAO>();
builder.Services.AddScoped<IPetReportDAO, PetReportDAO>();
builder.Services.AddScoped<IMessageDAO, MessageDAO>();
builder.Services.AddScoped<IContentModerationDAO, ContentModerationDAO>();

// Add authentication with cookie scheme
builder
    .Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Đường dẫn đến trang đăng nhập
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied"; // Tùy chọn
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Đảm bảo UseAuthentication được gọi trước UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

// Configure default route
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
