using System.Net.Http.Headers;
using System.Net.Http.Json;
using BiztvillCRM.Shared.Dtos.Mobile;

namespace BiztvillCRM.Mobile.Services;

/// <summary>
/// A backend API-val (BiztvillCRM.Web /api/... végpontok) kommunikáló kliens.
/// Csak olvasási (GET) hívásokat használ, mert a mobil app read-only.
/// </summary>
public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly AuthState _authState;

    public ApiClient(HttpClient httpClient, AuthState authState)
    {
        _httpClient = httpClient;
        _authState = authState;
    }

    private void AlkalmazTokent()
    {
        _httpClient.DefaultRequestHeaders.Authorization = _authState.Token is null
            ? null
            : new AuthenticationHeaderValue("Bearer", _authState.Token);
    }

    public async Task<LoginResponseDto?> BejelentkezesAsync(string usernameOrEmail, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", new LoginRequestDto
        {
            UsernameOrEmail = usernameOrEmail,
            Password = password
        });

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<LoginResponseDto>();
    }

    public async Task<List<JegyzokonyvListItemDto>> GetJegyzokonyvekAsync()
    {
        AlkalmazTokent();
        var eredmeny = await _httpClient.GetFromJsonAsync<List<JegyzokonyvListItemDto>>("api/jegyzokonyvek");
        return eredmeny ?? new List<JegyzokonyvListItemDto>();
    }

    public async Task<JegyzokonyvDetailDto?> GetJegyzokonyvAsync(int id)
    {
        AlkalmazTokent();
        return await _httpClient.GetFromJsonAsync<JegyzokonyvDetailDto>($"api/jegyzokonyvek/{id}");
    }

    public async Task<List<HitelesitesListItemDto>> GetHitelesitesekAsync()
    {
        AlkalmazTokent();
        var eredmeny = await _httpClient.GetFromJsonAsync<List<HitelesitesListItemDto>>("api/hitelesitesek");
        return eredmeny ?? new List<HitelesitesListItemDto>();
    }

    public async Task<HitelesitesDetailDto?> GetHitelesitesAsync(int id)
    {
        AlkalmazTokent();
        return await _httpClient.GetFromJsonAsync<HitelesitesDetailDto>($"api/hitelesitesek/{id}");
    }

    public async Task<List<MunkaszamListItemDto>> GetMunkaszamokAsync()
    {
        AlkalmazTokent();
        var eredmeny = await _httpClient.GetFromJsonAsync<List<MunkaszamListItemDto>>("api/munkaszamok");
        return eredmeny ?? new List<MunkaszamListItemDto>();
    }

    public async Task<MunkaszamListItemDto?> GetMunkaszamAsync(int id)
    {
        AlkalmazTokent();
        return await _httpClient.GetFromJsonAsync<MunkaszamListItemDto>($"api/munkaszamok/{id}");
    }

    public async Task<List<UgyfelListItemDto>> GetUgyfelekAsync()
    {
        AlkalmazTokent();
        var eredmeny = await _httpClient.GetFromJsonAsync<List<UgyfelListItemDto>>("api/ugyfelek");
        return eredmeny ?? new List<UgyfelListItemDto>();
    }

    public async Task<UgyfelListItemDto?> GetUgyfelAsync(int id)
    {
        AlkalmazTokent();
        return await _httpClient.GetFromJsonAsync<UgyfelListItemDto>($"api/ugyfelek/{id}");
    }

    public async Task<List<TelephelyListItemDto>> GetTelephelyekAsync()
    {
        AlkalmazTokent();
        var eredmeny = await _httpClient.GetFromJsonAsync<List<TelephelyListItemDto>>("api/telephelyek");
        return eredmeny ?? new List<TelephelyListItemDto>();
    }

    public async Task<TelephelyListItemDto?> GetTelephelyAsync(int id)
    {
        AlkalmazTokent();
        return await _httpClient.GetFromJsonAsync<TelephelyListItemDto>($"api/telephelyek/{id}");
    }

    public async Task<List<EszkozListItemDto>> GetEszkozokAsync()
    {
        AlkalmazTokent();
        var eredmeny = await _httpClient.GetFromJsonAsync<List<EszkozListItemDto>>("api/eszkozok");
        return eredmeny ?? new List<EszkozListItemDto>();
    }

    public async Task<EszkozListItemDto?> GetEszkozAsync(int id)
    {
        AlkalmazTokent();
        return await _httpClient.GetFromJsonAsync<EszkozListItemDto>($"api/eszkozok/{id}");
    }

    public async Task<List<KarbantartasListItemDto>> GetKarbantartasokAsync()
    {
        AlkalmazTokent();
        var eredmeny = await _httpClient.GetFromJsonAsync<List<KarbantartasListItemDto>>("api/karbantartasok");
        return eredmeny ?? new List<KarbantartasListItemDto>();
    }

    public async Task<KarbantartasListItemDto?> GetKarbantartasAsync(int id)
    {
        AlkalmazTokent();
        return await _httpClient.GetFromJsonAsync<KarbantartasListItemDto>($"api/karbantartasok/{id}");
    }

    public async Task<List<KalibracioListItemDto>> GetKalibraciokAsync()
    {
        AlkalmazTokent();
        var eredmeny = await _httpClient.GetFromJsonAsync<List<KalibracioListItemDto>>("api/kalibraciok");
        return eredmeny ?? new List<KalibracioListItemDto>();
    }

    public async Task<KalibracioListItemDto?> GetKalibracioAsync(int id)
    {
        AlkalmazTokent();
        return await _httpClient.GetFromJsonAsync<KalibracioListItemDto>($"api/kalibraciok/{id}");
    }

    public async Task<List<KepzesListItemDto>> GetKepzesekAsync()
    {
        AlkalmazTokent();
        var eredmeny = await _httpClient.GetFromJsonAsync<List<KepzesListItemDto>>("api/kepzesek");
        return eredmeny ?? new List<KepzesListItemDto>();
    }

    public async Task<KepzesListItemDto?> GetKepzesAsync(int id)
    {
        AlkalmazTokent();
        return await _httpClient.GetFromJsonAsync<KepzesListItemDto>($"api/kepzesek/{id}");
    }

    public async Task<List<MunkavedelmiOktatasListItemDto>> GetMunkavedelmiOktatasokAsync()
    {
        AlkalmazTokent();
        var eredmeny = await _httpClient.GetFromJsonAsync<List<MunkavedelmiOktatasListItemDto>>("api/munkavedelmioktatasok");
        return eredmeny ?? new List<MunkavedelmiOktatasListItemDto>();
    }

    public async Task<MunkavedelmiOktatasListItemDto?> GetMunkavedelmiOktatasAsync(int id)
    {
        AlkalmazTokent();
        return await _httpClient.GetFromJsonAsync<MunkavedelmiOktatasListItemDto>($"api/munkavedelmioktatasok/{id}");
    }

    public async Task<List<KockazatertekelesListItemDto>> GetKockazatertekelesekAsync()
    {
        AlkalmazTokent();
        var eredmeny = await _httpClient.GetFromJsonAsync<List<KockazatertekelesListItemDto>>("api/kockazatertekelesek");
        return eredmeny ?? new List<KockazatertekelesListItemDto>();
    }

    public async Task<KockazatertekelesListItemDto?> GetKockazatertekelesAsync(int id)
    {
        AlkalmazTokent();
        return await _httpClient.GetFromJsonAsync<KockazatertekelesListItemDto>($"api/kockazatertekelesek/{id}");
    }

    public async Task<List<FelulvizsgaloListItemDto>> GetFelulvizsgalokAsync()
    {
        AlkalmazTokent();
        var eredmeny = await _httpClient.GetFromJsonAsync<List<FelulvizsgaloListItemDto>>("api/felulvizsgalok");
        return eredmeny ?? new List<FelulvizsgaloListItemDto>();
    }

    public async Task<FelulvizsgaloDetailDto?> GetFelulvizsgaloAsync(int id)
    {
        AlkalmazTokent();
        return await _httpClient.GetFromJsonAsync<FelulvizsgaloDetailDto>($"api/felulvizsgalok/{id}");
    }

    public async Task<List<TanusitvanyListItemDto>> GetTanusitvanyokAsync()
    {
        AlkalmazTokent();
        var eredmeny = await _httpClient.GetFromJsonAsync<List<TanusitvanyListItemDto>>("api/tanusitvanyok");
        return eredmeny ?? new List<TanusitvanyListItemDto>();
    }

    public async Task<TanusitvanyListItemDto?> GetTanusitvanyAsync(int id)
    {
        AlkalmazTokent();
        return await _httpClient.GetFromJsonAsync<TanusitvanyListItemDto>($"api/tanusitvanyok/{id}");
    }

    public async Task<List<TerminalListItemDto>> GetTerminalokAsync()
    {
        AlkalmazTokent();
        var eredmeny = await _httpClient.GetFromJsonAsync<List<TerminalListItemDto>>("api/terminalok");
        return eredmeny ?? new List<TerminalListItemDto>();
    }

    public async Task<TerminalListItemDto?> GetTerminalAsync(int id)
    {
        AlkalmazTokent();
        return await _httpClient.GetFromJsonAsync<TerminalListItemDto>($"api/terminalok/{id}");
    }

    public async Task<List<JogszabalyListItemDto>> GetJogszabalyokAsync()
    {
        AlkalmazTokent();
        var eredmeny = await _httpClient.GetFromJsonAsync<List<JogszabalyListItemDto>>("api/jogszabalyok");
        return eredmeny ?? new List<JogszabalyListItemDto>();
    }

    public async Task<JogszabalyListItemDto?> GetJogszabalyAsync(int id)
    {
        AlkalmazTokent();
        return await _httpClient.GetFromJsonAsync<JogszabalyListItemDto>($"api/jogszabalyok/{id}");
    }

    public async Task<List<HatosagListItemDto>> GetHatosagokAsync()
    {
        AlkalmazTokent();
        var eredmeny = await _httpClient.GetFromJsonAsync<List<HatosagListItemDto>>("api/hatosagok");
        return eredmeny ?? new List<HatosagListItemDto>();
    }

    public async Task<HatosagListItemDto?> GetHatosagAsync(int id)
    {
        AlkalmazTokent();
        return await _httpClient.GetFromJsonAsync<HatosagListItemDto>($"api/hatosagok/{id}");
    }

    public async Task<List<GyartoListItemDto>> GetGyartokAsync()
    {
        AlkalmazTokent();
        var eredmeny = await _httpClient.GetFromJsonAsync<List<GyartoListItemDto>>("api/gyartok");
        return eredmeny ?? new List<GyartoListItemDto>();
    }

    public async Task<GyartoListItemDto?> GetGyartoAsync(int id)
    {
        AlkalmazTokent();
        return await _httpClient.GetFromJsonAsync<GyartoListItemDto>($"api/gyartok/{id}");
    }

    public async Task<List<KotelezoHitelesitesListItemDto>> GetKotelezoHitelesitesekAsync()
    {
        AlkalmazTokent();
        var eredmeny = await _httpClient.GetFromJsonAsync<List<KotelezoHitelesitesListItemDto>>("api/kotelezohitelesitesek");
        return eredmeny ?? new List<KotelezoHitelesitesListItemDto>();
    }

    public async Task<KotelezoHitelesitesListItemDto?> GetKotelezoHitelesitesAsync(int id)
    {
        AlkalmazTokent();
        return await _httpClient.GetFromJsonAsync<KotelezoHitelesitesListItemDto>($"api/kotelezohitelesitesek/{id}");
    }
}
