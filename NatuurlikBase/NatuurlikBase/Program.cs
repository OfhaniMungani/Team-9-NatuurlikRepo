using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using Microsoft.AspNetCore.Identity;
using NatuurlikBase.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using NatuurlikBase.Services;
using NatuurlikBase.Repository.IRepository;
using NatuurlikBase.Repository;
using NatuurlikBase.Models;
using Stripe;
using NatuurlikBase.Factory;
using NatuurlikBase.Data.DbInitilizer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<ReminderService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true).AddDefaultTokenProviders().AddDefaultUI()
    .AddEntityFrameworkStores<DatabaseContext>()
     .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();

builder.Services.AddServerSideBlazor().AddCircuitOptions(option => { option.DetailedErrors = true; });
builder.Services.AddTransient<IProductInventoryRepository, ProductInventoryRepository>();
builder.Services.AddTransient<IInventoryItemRepository, InventoryItemRepository>();
builder.Services.AddTransient<IProductionTransactionRepository, ProductionTransactionRepository>();
builder.Services.AddTransient<IInventoryItemTransactionRepository, InventoryItemTransactionRepository>();
builder.Services.AddTransient<IViewProductsByName, ViewProductsByName>();
builder.Services.AddTransient<IViewConfiguredProductsByName, ViewConfiguredProductsByName>();
builder.Services.AddTransient<IViewInventoriesByName, ViewInventoriesByName>();
builder.Services.AddTransient<IViewInventoryById, ViewInventoryById>();
builder.Services.AddTransient<IViewProductById, ViewProductById>();
builder.Services.AddTransient<IEditProduct, EditProduct>();
builder.Services.AddTransient<IProduceFinishedProduct, ProduceFinishedProduct>();
builder.Services.AddTransient<IValidateEnoughInventories, ValidateEnoughInventories>();
builder.Services.AddTransient<ISupplierOrderRepository, SupplierOrderRepository>();
builder.Services.AddTransient<IViewSuppliersByName, ViewSuppliersByName>();
builder.Services.AddTransient<ISupplierOrderRepository, SupplierOrderRepository>();
builder.Services.AddTransient<ISendSupplierOrderRepository, SendSupplierOrderRepository>();
builder.Services.AddTransient<IViewSupplierById, ViewSupplierById>();
builder.Services.AddTransient<ISearchProductionTransactionsRepository, SearchProductionTransactionsRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDbInitilizer, DbInitilizer>();
builder.Services.AddTransient<IEmailSender, SendGridEmailSender>();
builder.Services.AddRazorPages();
builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, AppUserClaimsPrincipalFactory>();
builder.Services.Configure<DataProtectionTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromHours(3));

//configure mobile application cors 
builder.Services.AddCors(options => options.AddDefaultPolicy(
               builder =>
               {
                   builder.AllowAnyOrigin();
                   builder.AllowAnyHeader();
                   builder.AllowAnyMethod();
               }));
//Configure Application Cookies
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
    options.LogoutPath = $"/Identity/Account/Logout";
}
);


// Add services to the container.

//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

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
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
SeedDatabase();
app.UseAuthentication();;

app.UseAuthorization();
app.MapRazorPages();
app.UseCors();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseEndpoints(endpoints =>
{
    endpoints.MapBlazorHub();
    app.MapFallbackToPage("/_Host");
});



app.Run();

void SeedDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbInitiliazer = scope.ServiceProvider.GetRequiredService<IDbInitilizer>();
        dbInitiliazer.Initialize();
    }
}
