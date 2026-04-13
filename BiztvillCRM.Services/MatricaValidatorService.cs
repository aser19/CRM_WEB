using System.Text.RegularExpressions;

namespace BiztvillCRM.Services;

public class MatricaValidationResult
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
    public int? MatrikakSzama { get; set; }
}

public interface IMatricaValidatorService
{
    MatricaValidationResult Validate(string? matricaTol, string? matricaIg, int eszkozokSzama, string? cegElotag);
    string GenerateMatricaElotag(string? matricaTol, string cegElotag);
    (string elotag, int szam)? ParseMatrica(string? matricaErtek);
}

public class MatricaValidatorService : IMatricaValidatorService
{
    // Regex: 3 betűs előtag + max 7 számjegy
    private static readonly Regex MatricaRegex = new(@"^([A-Z]{3})(\d{1,7})$", RegexOptions.Compiled);

    public MatricaValidationResult Validate(string? matricaTol, string? matricaIg, int eszkozokSzama, string? cegElotag)
    {
        // Ha nincs kitöltve, nincs validáció
        if (string.IsNullOrWhiteSpace(matricaTol) && string.IsNullOrWhiteSpace(matricaIg))
        {
            return new MatricaValidationResult { IsValid = true };
        }

        // Ha csak az egyik van kitöltve
        if (string.IsNullOrWhiteSpace(matricaTol) || string.IsNullOrWhiteSpace(matricaIg))
        {
            return new MatricaValidationResult
            {
                IsValid = false,
                ErrorMessage = "Mindkét matrica mezőt ki kell tölteni (tól és ig)!"
            };
        }

        // Előtag ellenőrzés
        var parsedTol = ParseMatrica(matricaTol);
        var parsedIg = ParseMatrica(matricaIg);

        if (parsedTol == null)
        {
            return new MatricaValidationResult
            {
                IsValid = false,
                ErrorMessage = $"Hibás matrica formátum (tól): '{matricaTol}'. Formátum: 3 betű + max 7 számjegy (pl. {cegElotag ?? "ABC"}0001234)"
            };
        }

        if (parsedIg == null)
        {
            return new MatricaValidationResult
            {
                IsValid = false,
                ErrorMessage = $"Hibás matrica formátum (ig): '{matricaIg}'. Formátum: 3 betű + max 7 számjegy (pl. {cegElotag ?? "ABC"}0001234)"
            };
        }

        // Előtag egyezés ellenőrzés
        if (parsedTol.Value.elotag != parsedIg.Value.elotag)
        {
            return new MatricaValidationResult
            {
                IsValid = false,
                ErrorMessage = $"A matrica előtagok nem egyeznek: '{parsedTol.Value.elotag}' vs '{parsedIg.Value.elotag}'"
            };
        }

        // Cég előtag ellenőrzés (ha van beállítva)
        if (!string.IsNullOrWhiteSpace(cegElotag) && parsedTol.Value.elotag != cegElotag.ToUpper())
        {
            return new MatricaValidationResult
            {
                IsValid = false,
                ErrorMessage = $"A matrica előtag ({parsedTol.Value.elotag}) nem egyezik a cég előtagjával ({cegElotag})!"
            };
        }

        // Sorrend ellenőrzés
        if (parsedTol.Value.szam > parsedIg.Value.szam)
        {
            return new MatricaValidationResult
            {
                IsValid = false,
                ErrorMessage = $"A 'tól' érték ({parsedTol.Value.szam}) nem lehet nagyobb mint az 'ig' érték ({parsedIg.Value.szam})!"
            };
        }

        // Matricák száma
        var matricakSzama = parsedIg.Value.szam - parsedTol.Value.szam + 1;

        // Eszközök száma ellenőrzés
        if (matricakSzama > eszkozokSzama)
        {
            return new MatricaValidationResult
            {
                IsValid = false,
                ErrorMessage = $"Több matrica ({matricakSzama} db) van megadva mint eszköz ({eszkozokSzama} db)! A matricák száma nem lehet több mint az eszközök száma.",
                MatrikakSzama = matricakSzama
            };
        }

        return new MatricaValidationResult
        {
            IsValid = true,
            MatrikakSzama = matricakSzama
        };
    }

    public string GenerateMatricaElotag(string? szam, string cegElotag)
    {
        if (string.IsNullOrWhiteSpace(szam))
            return "";

        // Ha már van előtag, ne adjunk hozzá
        if (MatricaRegex.IsMatch(szam.ToUpper()))
            return szam.ToUpper();

        // Ha csak szám, adjuk hozzá az előtagot
        if (int.TryParse(szam, out var num))
        {
            return $"{cegElotag.ToUpper()}{num:D7}";
        }

        return szam;
    }

    public (string elotag, int szam)? ParseMatrica(string? matricaErtek)
    {
        if (string.IsNullOrWhiteSpace(matricaErtek))
            return null;

        var match = MatricaRegex.Match(matricaErtek.ToUpper().Trim());
        if (!match.Success)
            return null;

        var elotag = match.Groups[1].Value;
        var szam = int.Parse(match.Groups[2].Value);

        return (elotag, szam);
    }
}