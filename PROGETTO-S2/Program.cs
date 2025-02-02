using Microsoft.AspNetCore.Authentication.Cookies;
using PROGETTO_S2.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//AUTHENTICATION
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
    });

builder.Services
    .AddScoped<IAuthService, AuthService>()
    .AddScoped<IPrenotazioniService, PrenotazioneService>()
    .AddScoped<ICreationService, CreateService>()
    .AddScoped<IAggServizioService, AggServizioService>()
    .AddScoped<ICheckoutService, CheckoutService>();

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

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

    endpoints.MapControllerRoute(
        name: "managment",
        pattern: "Managment/{action=Index}/{id?}",
        defaults: new { controller = "Managment" });
});


app.Run();
