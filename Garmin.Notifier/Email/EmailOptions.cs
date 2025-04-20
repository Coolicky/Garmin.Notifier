namespace Garmin.Notifier.Email;

public class EmailOptions
{
    public string Host { get; init; } = "";
    public int Port { get; init; } = 993;
    public bool UseSsl { get; init; } = true;
    public string Username { get; init; } = "";
    public string Password { get; init; } = "";

    public bool IsValid(out List<string> messages)
    {
        messages = [];
        if (string.IsNullOrWhiteSpace(Host))
            messages.Add($"{nameof(Host)} is required");
        if (string.IsNullOrWhiteSpace(Username))
            messages.Add($"{nameof(Username)} is required");
        if (string.IsNullOrWhiteSpace(Password))
            messages.Add($"{nameof(Password)} is required");
        return !messages.Any();
    }
}