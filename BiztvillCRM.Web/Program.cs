using BiztvillCRM.Data;
using BiztvillCRM.Services;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// --- MudBlazor ---
builder.Services.AddMudServices();

// --- Blazor ---
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        options.DetailedErrors = builder.Environment.IsDevelopment();
    });

// --- Azure SQL + EF Core ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CrmDbContext>(options =>
    options.UseSqlServer(connectionString));

// --- Identity ---
builder.Services.AddIdentity<Felhasznalo, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<CrmDbContext>()
.AddDefaultTokenProviders();

// --- Cookie beállítások ---
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/bejelentkezes";
    options.LogoutPath = "/kijelentkezes";
    options.AccessDeniedPath = "/hozzaferes-megtagadva";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

// --- Auth cascade ---
builder.Services.AddCascadingAuthenticationState();

// --- HTTP Context ---
builder.Services.AddHttpContextAccessor();

// --- Tenant szolgáltatás ---
builder.Services.AddScoped<ITenantService, BiztvillCRM.Web.Services.TenantService>();

// --- Szolgáltatások regisztrálása ---
builder.Services.AddScoped<IUgyfelService, UgyfelService>();
builder.Services.AddScoped<ITelephelyService, TelephelyService>();
builder.Services.AddScoped<IGyartoService, GyartoService>();
builder.Services.AddScoped<IEszkozService, EszkozService>();
builder.Services.AddScoped<IMeresService, MeresService>();
builder.Services.AddScoped<IMeresTipusService, MeresTipusService>();
builder.Services.AddScoped<IKarbantartasService, KarbantartasService>();
builder.Services.AddScoped<ITanusitvanyService, TanusitvanyService>();
builder.Services.AddScoped<IKepzesService, KepzesService>();
builder.Services.AddScoped<IHitelesitesService, HitelesitesService>();
builder.Services.AddScoped<IHatosagService, HatosagService>();
builder.Services.AddScoped<IJogszabalyService, JogszabalyService>();
builder.Services.AddScoped<IKalibracioService, KalibracioService>();
builder.Services.AddScoped<ITerminalService, TerminalService>();
builder.Services.AddScoped<IUgyszamService, UgyszamService>();

var app = builder.Build();

// --- Adatbázis inicializálás + szerepkörök létrehozása ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Felhasznalo>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        DbInitializer.Initialize(db);
        await SzerepkorokLetrehozasa(roleManager);
        await AdminFelhasznaloLetrehozasa(db, userManager);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Hiba az adatbázis inicializálása közben.");
        throw; // <-- Add hozzá ideiglenesen a hiba megtekintéséhez
    }
}

// --- Middleware pipeline ---
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// --- Authentication & Authorization ---
app.UseAuthentication();
app.UseAuthorization();

// --- Blazor renderelés ---
app.MapRazorComponents<BiztvillCRM.Web.Components.App>()
    .AddInteractiveServerRenderMode();

// --- Bejelentkezési endpoint (Interactive Server módban a HttpContext válasza már le van zárva,
//     ezért egy valódi HTTP POST végponton keresztül állítjuk be a sütit.) ---
app.MapPost("/account/login", async (HttpContext ctx, SignInManager<Felhasznalo> signInManager) =>
{
    var form = ctx.Request.Form;
    var email = form["email"].FirstOrDefault() ?? string.Empty;
    var password = form["password"].FirstOrDefault() ?? string.Empty;
    bool.TryParse(form["rememberMe"].FirstOrDefault(), out var rememberMe);

    var result = await signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);
    if (result.Succeeded)
        return Results.LocalRedirect("/");
    SignInResult signInResult = await signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);
    

    if (result.Succeeded)
        return Results.LocalRedirect("/");


    var hiba = result.IsLockedOut ? "zarolt" : "hibas";
    return Results.Redirect($"/bejelentkezes?hiba={hiba}");
});


app.Run();

// --- Szerepkörök létrehozása ---
static async Task SzerepkorokLetrehozasa(RoleManager<IdentityRole> roleManager)
{
    string[] roles = {
        FelhasznaloSzerepkor.Admin.ToString(),
        FelhasznaloSzerepkor.CegAdmin.ToString(),
        FelhasznaloSzerepkor.Felhasznalo.ToString(),
        FelhasznaloSzerepkor.Megtekinto.ToString()
    };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

// --- Alapértelmezett admin felhasználó létrehozása ---
static async Task AdminFelhasznaloLetrehozasa(CrmDbContext db, UserManager<Felhasznalo> userManager)
{
    if (!db.Cegek.Any())
    {
        var ceg = new Ceg
        {
            Nev = "Biztovill CRM Demo",
            Email = "info@biztovill.hu",
            Aktiv = true,
            Letrehozva = DateTime.Now
        };
        db.Cegek.Add(ceg);
        await db.SaveChangesAsync();
    }

    if (await userManager.FindByEmailAsync("admin@biztovill.hu") == null)
    {
        var ceg = db.Cegek.First();
        var admin = new Felhasznalo
        {
            UserName = "admin@biztovill.hu",
            Email = "admin@biztovill.hu",
            EmailConfirmed = true,
            Nev = "Rendszergazda",
            CegId = ceg.Id,
            Aktiv = true,
            Letrehozva = DateTime.Now
        };

        var result = await userManager.CreateAsync(admin, "Admin123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, FelhasznaloSzerepkor.Admin.ToString());
        }
    }
}