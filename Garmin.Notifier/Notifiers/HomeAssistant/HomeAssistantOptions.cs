namespace Garmin.Notifier.Notifiers.HomeAssistant;

public class HomeAssistantOptions : INotifierOptions
{
    public static string Prefix => "homeassistant";

    public bool IsValid()
    {
        if (EntityIds.Length < 1)
            return false;
        if (string.IsNullOrEmpty(Title))
            return false;
        if (!Uri.TryCreate(Url, UriKind.Absolute, out _))
            return false;
        if (string.IsNullOrEmpty(Token))
            return false;
        return true;
    }

    public string[] EntityIds { get; set; } = [];
    public string? Message { get; set; }
    public string Title { get; set; } = "";
    public string Url { get; set; } = "";
    public string Token { get; set; } = "";

}