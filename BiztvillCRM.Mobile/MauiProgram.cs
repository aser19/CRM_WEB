using Microsoft.Extensions.Logging;
using BiztvillCRM.Mobile.Services;
using MudBlazor.Services;

namespace BiztvillCRM.Mobile;

public static class MauiProgram
{
	/// <summary>
	/// A backend API alapcíme. Mivel éles (publikált Azure) szervert használunk teszteléshez is,
	/// ez közvetlenül az éles webalkalmazás címére mutat.
	/// </summary>
	private const string ApiBaseUrl = "https://biztvillcrmweb20260327201621-fqduhxh2e8b0fbdf.westeurope-01.azurewebsites.net/";

	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();
		builder.Services.AddMudServices();

		builder.Services.AddSingleton<AuthState>();
		builder.Services.AddSingleton<OfflineCacheService>();
		builder.Services.AddHttpClient<ApiClient>(client =>
		{
			client.BaseAddress = new Uri(ApiBaseUrl);
		});

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
