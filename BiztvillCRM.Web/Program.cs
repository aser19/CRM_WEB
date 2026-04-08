using BiztvillCRM.Data;
using BiztvillCRM.Services;
using BiztvillCRM.Services.Interfaces;
using BiztvillCRM.Shared.Enums;
using BiztvillCRM.Shared.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using BiztvillCRM.Web.Services;

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
.AddDefaultTokenProviders()
.AddClaimsPrincipalFactory<BiztvillCRM.Web.Services.CustomUserClaimsPrincipalFactory>();

// --- Cookie beállítások ---
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/bejelentkezes";
    options.LogoutPath = "/kijelentkezes";
    options.AccessDeniedPath = "/hozzaferes-megtagadva";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
    // NE legyen OnRedirectToLogin event!
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
builder.Services.AddScoped<ICegService, CegService>();
builder.Services.AddAuthenticationCore();
builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
builder.Services.AddScoped<IEszkozTipusService, EszkozTipusService>();
builder.Services.AddScoped<IKarbantartasTipusService, KarbantartasTipusService>();
// builder.Services.AddScoped<IJedzokonyvPdfService, JedzokonyvPdfService>();
builder.Services.AddScoped<ISablonService>(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    var sablonMappa = Path.Combine(env.WebRootPath, "sablonok");
    return new SablonService(sablonMappa);
});
builder.Services.AddScoped<IJegyzokonyvWordService>(sp =>
{
    var meresService = sp.GetRequiredService<IMeresService>();
    var tenantService = sp.GetRequiredService<ITenantService>();
    var cegService = sp.GetRequiredService<ICegService>();
    var sablonService = sp.GetRequiredService<ISablonService>();
    return new JegyzokonyvWordService(meresService, tenantService, cegService, sablonService);
}); // <-- ÚJ SZOLGÁLTATÁS
builder.Services.AddScoped<IKepzesTipusService, KepzesTipusService>(); // <-- ÚJ SZOLGÁLTATÁS
builder.Services.AddScoped<IKepzesSzabalyService, KepzesSzabalyService>(); // <-- ÚJ SZOLGÁLTATÁS
builder.Services.AddScoped<IFelulvizsgaloService, FelulvizsgaloService>();
builder.Services.AddScoped<IJegyzokonyvPdfService, JegyzokonyvPdfService>(); // <-- ÚJ SOR
builder.Services.AddScoped<IJegyzokonyvJogosultsagService, JegyzokonyvJogosultsagService>(); // <-- ÚJ SZOLGÁLTATÁS

// === Email szolgáltatások ===
builder.Services.AddScoped<ISmtpBeallitasService, SmtpBeallitasService>();
builder.Services.AddScoped<IEmailSablonService, EmailSablonService>();
builder.Services.AddScoped<IEmailBeallitasService, EmailBeallitasService>();
builder.Services.AddScoped<IEmailKuldoService, EmailKuldoService>();
builder.Services.AddScoped<ILejaratErtesitoService, LejaratErtesitoService>();
builder.Services.AddHostedService<EmailErtesitesBackgroundService>();

// Egyszerű authorization, FallbackPolicy NÉLKÜL
builder.Services.AddAuthorizationCore();

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
        // DbInitializer.Initialize(db); <-- TÖRÖLVE
        await SzerepkorokLetrehozasa(roleManager);
        await AdminFelhasznaloLetrehozasa(db, userManager);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Hiba az adatbázis inicializálása közben.");
        throw;
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

// ⚠️ FONTOS: UseStaticFiles ELŐBB kell, mint UseAuthentication!
app.UseStaticFiles();

// --- Authentication & Authorization ---
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

// --- Blazor renderelés ---
app.MapRazorComponents<BiztvillCRM.Web.Components.App>()
    .AddInteractiveServerRenderMode();

// --- Bejelentkezési endpoint (Interactive Server módban a HttpContext válasza már le van zárva,
//     ezért egy valódi HTTP POST végponton keresztül állítjuk be a sütit.) ---
app.MapPost("/account/login", async (HttpContext ctx, SignInManager<Felhasznalo> signInManager, UserManager<Felhasznalo> userManager, ILogger<Program> logger) =>
{
    var form = ctx.Request.Form;
    var usernameOrEmail = form["email"].FirstOrDefault() ?? string.Empty;
    var password = form["password"].FirstOrDefault() ?? string.Empty;
    bool.TryParse(form["rememberMe"].FirstOrDefault(), out var rememberMe);

    logger.LogInformation("Bejelentkezési kísérlet: {UsernameOrEmail}, RememberMe={RememberMe}", usernameOrEmail, rememberMe);

    // Először próbáljuk felhasználónévként
    var user = await userManager.FindByNameAsync(usernameOrEmail);
    
    // Ha nem találtuk, próbáljuk email-ként
    if (user == null)
    {
        user = await userManager.FindByEmailAsync(usernameOrEmail);
    }

    if (user == null)
    {
        logger.LogWarning("Felhasználó nem található: {UsernameOrEmail}", usernameOrEmail);
        return Results.Redirect("/bejelentkezes?hiba=hibas");
    }

    // Ellenőrizzük, hogy aktív-e
    if (!user.Aktiv)
    {
        logger.LogWarning("Inaktív felhasználó próbál belépni: {UsernameOrEmail}", usernameOrEmail);
        return Results.Redirect("/bejelentkezes?hiba=inaktiv");
    }

    var result = await signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: false);

    if (result.Succeeded)
    {
        // Utolsó belépés frissítése
        user.UtolsoBelepes = DateTime.Now;
        await userManager.UpdateAsync(user);
        
        logger.LogInformation("Bejelentkezés sikeres: {UserName}", user.UserName);
        return Results.LocalRedirect("/");
    }

    if (result.IsLockedOut)
    {
        logger.LogWarning("A fiók zárolva: {UserName}", user.UserName);
        return Results.Redirect("/bejelentkezes?hiba=zarolt");
    }

    logger.LogWarning("Hibás jelszó: {UsernameOrEmail}", usernameOrEmail);
    return Results.Redirect("/bejelentkezes?hiba=hibas");
}).AllowAnonymous();  // <-- HOZZÁADVA

// --- Kijelentkezési endpoint ---
app.MapGet("/account/logout", async (HttpContext ctx, SignInManager<Felhasznalo> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/bejelentkezes");
}).AllowAnonymous();  // <-- HOZZÁADVA

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