using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Tutorial.Areas.Admin.Repository;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Repository;
using System.Numerics;

var builder = WebApplication.CreateBuilder(args);


//// C?u hình d?ch v?
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(options =>
//    {
//        options.LoginPath = "/Account/Login";
//        options.LogoutPath = "/Account/Logout";
//        // C?u hình thêm tùy ch?n
//    });

//builder.Services.AddAuthorization(); // Thêm d?ch v? phân quy?n


//Connection db
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:ConnectedDb"]);
});

//Add email sender
builder.Services.AddTransient<IEmailSender, EmailSender>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.IsEssential = true;
});


builder.Services.AddIdentity<AppUserModel, IdentityRole>()
	.AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
	// Password settings.
	options.Password.RequireDigit = true;
	options.Password.RequireLowercase = true;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequireUppercase = false;
	options.Password.RequiredLength = 4;
	//options.Password.RequiredUniqueChars = 1;

	// Lockout settings.
	//options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
	//options.Lockout.MaxFailedAccessAttempts = 5;
	//options.Lockout.AllowedForNewUsers = true;

	// User settings.
	//options.User.AllowedUserNameCharacters =
	//"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
	options.User.RequireUniqueEmail = false;
});

var app = builder.Build();


app.UseStatusCodePagesWithRedirects("/Home/Error?statuscode={0}");

app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();	//??ng nh?p
app.UseAuthorization();		//Ki?m tra quy?n

app.MapControllerRoute(
    name: "Areas",
    pattern: "{area:exists}/{controller=Product}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "category",
    pattern: "/category/{Slug?}",
    defaults: new {controller="Category", action="Index" });

app.MapControllerRoute(
    name: "brand",
    pattern: "/brand/{Slug?}",
    defaults: new { controller = "Brand", action = "Index" });

app.MapControllerRoute(
	name: "account",
	pattern: "/account/",
	defaults: new { controller = "Account", action = "Login" });



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


//Seeding Data
var context =app.Services.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
SeedData.SeedingData(context);

using (var scope = app.Services.CreateScope())
{
	var roleManage = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

	var roles = new[] { "Admin", "Sales", "Author" };

	foreach(var role in roles)
	{
		if (!await roleManage.RoleExistsAsync(role))
			await roleManage.CreateAsync(new IdentityRole(role));
	}
}

app.Run();
