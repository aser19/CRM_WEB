using BiztvillCRM.Data;
using BiztvillCRM.Services;
using BiztvillCRM.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// --- MudBlazor ---
builder.Services.AddMudServices();

// --- Blazor ---
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// --- Azure SQL + EF Core ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CrmDbContext>(options =>
    options.UseSqlServer(connectionString));

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

// --- Adatbázis inicializálás (táblák + teszt adatok) ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        DbInitializer.Initialize(db);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Hiba az adatbázis inicializálása közben.");
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

// --- Blazor renderelés ---
app.MapRazorComponents<BiztvillCRM.Web.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();