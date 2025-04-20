using System.Net.Http.Json;
using Microsoft.Extensions.Options;

namespace Garmin.Notifier.Notifiers.HomeAssistant;

public class HomeAssistantNotifier : INotifier
{
    private const string Url = "{0}/api/services/notify/{1}";
    
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HomeAssistantOptions _options;
    private readonly ILogger<HomeAssistantNotifier> _logger;

    public HomeAssistantNotifier(
        IHttpClientFactory httpClientFactory,
        IOptions<HomeAssistantOptions> options,
        ILogger<HomeAssistantNotifier> logger)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<bool> Notify(string garminUrl, CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient(HomeAssistantOptions.Prefix);
        
        var payload = new
        {
            message = _options.Message,
            title = _options.Title,
            data = new
            {
                clickAction = garminUrl,
            }
        };

        foreach (var entityId in _options.EntityIdList)
        {
            var url = string.Format(Url, _options.Url, entityId);

            var response = await client.PostAsJsonAsync(url, payload, cancellationToken: cancellationToken);    
            if (response.IsSuccessStatusCode)
                _logger.LogInformation("Notification sent successfully.");
            else
                _logger.LogError("Failed to send notification: {statusCode} | entityId: {EntityId}", response.StatusCode, entityId);
        }

        return true;
    }
}