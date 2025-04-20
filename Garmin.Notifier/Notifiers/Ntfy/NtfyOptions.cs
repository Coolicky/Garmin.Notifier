using System.Text;

namespace Garmin.Notifier.Notifiers.Ntfy;

public class NtfyOptions : INotifierOptions
{
    public static string Prefix => "Ntfy";

    public bool IsValid()
    {
        if (string.IsNullOrEmpty(Topic))
            return false;
        if (string.IsNullOrEmpty(Message))
            return false;
        if (!Uri.TryCreate(Url, UriKind.Absolute, out _))
            return false;
        var isBasicMissing = string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password);
        var isTokenMissing = string.IsNullOrEmpty(Token);
        if (isBasicMissing && isTokenMissing)
            return false;
        return true;
    }

    public string Auth()
    {
        if (!string.IsNullOrEmpty(Token))
        {
            return $"Bearer {Token}";
        }

        return $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Username}:{Password}"))}";
    }

    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Token { get; set; }

    public string Topic { get; set; } = "";
    public string Message { get; set; } = "";
    public string? Label { get; set; }
    public string Url { get; set; } = "";
}