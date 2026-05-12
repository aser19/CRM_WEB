namespace BiztvillCRM.Services;

public class AktualisCegService
{
    public int? AktualisCegId { get; private set; }
    public string? AktualisCegNev { get; private set; }

    public event Action? OnChange;

    public void CegValtas(int cegId, string cegNev)
    {
        AktualisCegId = cegId;
        AktualisCegNev = cegNev;
        OnChange?.Invoke();
    }
}