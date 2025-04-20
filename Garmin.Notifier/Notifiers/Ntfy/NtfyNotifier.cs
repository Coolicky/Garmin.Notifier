using System.Net.Http.Json;
using Microsoft.Extensions.Options;

namespace Garmin.Notifier.Notifiers.Ntfy;

public class NtfyNotifier : INotifier
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly NtfyOptions _options;
    private readonly ILogger<NtfyNotifier> _logger;

    public NtfyNotifier(
        IHttpClientFactory httpClientFactory,
        IOptions<NtfyOptions> options,
        ILogger<NtfyNotifier> logger)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<bool> Notify(string garminUrl, CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient(NtfyOptions.Prefix);

        var payload = new
        {
            topic = _options.Topic,
            message = _options.Message,
            actions = (List<object>)
            [
                new
                {
                    action = "view",
                    label = _options.Label,
                    url = garminUrl,
                }
            ]
        };

        var response = await client.PostAsJsonAsync("", payload, cancellationToken: cancellationToken);
        if (response.IsSuccessStatusCode)
            _logger.LogInformation("Notification sent successfully.");
        else
            _logger.LogError("Failed to send notification: {statusCode}", response.StatusCode);

        return true;
    }
}