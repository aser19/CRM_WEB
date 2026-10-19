using Microsoft.AspNetCore.Components.WebView.Maui;

namespace BiztvillCRM.Mobile;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	public BlazorWebView BlazorWebView => blazorWebView;
}
