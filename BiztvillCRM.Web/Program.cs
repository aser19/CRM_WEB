using BiztvillCRM.Data;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// --- MudBlazor ---
builder.Services.AddMudServices();

// --- Blazor ---
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// --- MySQL + EF Core ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CrmDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

// --- Autentikáció (később bővítjük) ---
// builder.Services.AddAuthentication(...)
// builder.Services.AddAuthorization(...)

// --- Szolgáltatások regisztrálása (később bővítjük) ---
// builder.Services.AddScoped<IUgyfelService, UgyfelService>();

var app = builder.Build();

// --- Middleware pipeline ---
if (app.Environment.IsDevelopment()) {
    app.UseWebAssemblyDebugging();
} else {
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// --- Blazor renderelés ---
app.MapRazorComponents<BiztvillCRM.Web.Components.App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BiztvillCRM.Client._Imports).Assembly);

app.Run();