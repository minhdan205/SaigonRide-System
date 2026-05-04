using Microsoft.EntityFrameworkCore;
using SaigonRideSystem.Data;
using SaigonRideSystem.Models;
using SaigonRideSystem.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession();

builder.Services.AddScoped<SaigonRideSystem.Services.PricingService>();

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

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    string adminEmail = "admin@saigonride.com";

    var existingAdmin = context.Users.FirstOrDefault(u => u.Email == adminEmail);

    if (existingAdmin == null)
    {
        var adminUser = new User
        {
            Name = "System Admin",
            Email = adminEmail,
            PasswordHash = PasswordHelper.HashPassword("admin123"),
            UserType = UserType.Admin,
            Passport = null
        };

        context.Users.Add(adminUser);
    }
    else
    {
        existingAdmin.Name = "System Admin";
        existingAdmin.PasswordHash = PasswordHelper.HashPassword("admin123");
        existingAdmin.UserType = UserType.Admin;
        existingAdmin.Passport = null;
    }

    context.SaveChanges();
}

app.Run();
