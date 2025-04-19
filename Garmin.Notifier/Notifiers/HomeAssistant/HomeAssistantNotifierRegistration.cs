namespace Garmin.Notifier.Notifiers.HomeAssistant;

public class HomeAssistantNotifierRegistration : INotifierRegistration
{
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        var homeAssistantOptions = configuration
            .GetSection(HomeAssistantOptions.Prefix)
            .Get<HomeAssistantOptions>();
        if (homeAssistantOptions?.IsValid() == true)
        {
            services.AddHttpClient(HomeAssistantOptions.Prefix, client =>
            {
                client.BaseAddress = new Uri(homeAssistantOptions.Url);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {homeAssistantOptions.Token}");
            });
            services.Configure<HomeAssistantOptions>(configuration.GetSection(HomeAssistantOptions.Prefix));
            services.AddSingleton<INotifier, HomeAssistantNotifier>();
        }
    }
}