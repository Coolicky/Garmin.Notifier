namespace Garmin.Notifier.Notifiers.Ntfy;

public class NtfyNotifierRegistration : INotifierRegistration
{
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration
            .GetSection(NtfyOptions.Prefix)
            .Get<NtfyOptions>();
        if (options?.IsValid() == true)
        {
            services.AddHttpClient(NtfyOptions.Prefix, client =>
            {
                client.BaseAddress = new Uri(options.Url);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", options.Auth());
            });
            services.Configure<NtfyOptions>(configuration.GetSection(NtfyOptions.Prefix));
            services.AddSingleton<INotifier, NtfyNotifier>();
        }
    }
}