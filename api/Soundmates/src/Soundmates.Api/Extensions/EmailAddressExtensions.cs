namespace Soundmates.Api.Extensions;

public static class EmailAddressExtensions
{
    public static string NormalizeEmail(this string email)
    {
        email = email.Trim().ToLowerInvariant();

        var parts = email.Split('@');
        if (parts.Length != 2)
            throw new ArgumentException("Invalid email format.", nameof(email));

        var local = parts[0];
        var domain = parts[1];

        if (domain == "gmail.com" || domain == "googlemail.com")
        {
            local = local.Split('+')[0];    // remove gmail alias
            local = local.Replace(".", ""); // remove dots from local part
        }

        return $"{local}@{domain}";
    }
}
