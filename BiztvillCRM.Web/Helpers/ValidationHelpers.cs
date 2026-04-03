namespace BiztvillCRM.Web.Helpers;

public static class ValidationHelpers
{
    /// <summary>Validálja a mező hosszát.</summary>
    public static string? ValidateMaxLength(string? value, int maxLength, string? fieldName = null)
    {
        if (string.IsNullOrEmpty(value))
            return null;
        
        if (value.Length > maxLength)
        {
            var name = string.IsNullOrEmpty(fieldName) ? "Ez a mező" : $"A(z) {fieldName}";
            return $"{name} maximum {maxLength} karakter lehet! ({value.Length}/{maxLength})";
        }
        
        return null;
    }

    /// <summary>Kötelező mező validáció.</summary>
    public static string? ValidateRequired(string? value, string? fieldName = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            var name = string.IsNullOrEmpty(fieldName) ? "Ez a mező" : $"A(z) {fieldName}";
            return $"{name} kitöltése kötelező!";
        }
        return null;
    }
}