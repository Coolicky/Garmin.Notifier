using Garmin.Notifier.Email;
using Garmin.Notifier.Notifiers;
using Microsoft.Extensions.Options;

namespace Garmin.Notifier;

public static class HostExtensions
{
    public static async Task ValidateOptionsAndNotifiers(this IHost host)
    {
        var email = host.Services.GetService<IOptions<EmailOptions>>();
        if (email?.Value == null)
        {
            Console.WriteLine("Email options are not set");
            await host.StopAsync();
            return;
        }

        if (!email.Value.IsValid(out var messages))
        {
            Console.WriteLine("Email options are not valid");
            foreach (var message in messages)
            {
                Console.WriteLine(message);
            }
            await host.StopAsync();
            return;
        }
        using var scope = host.Services.CreateScope();
        var emailReader = scope.ServiceProvider.GetRequiredService<IEmailReader>();
        var isConnected = await emailReader.Connect();
        if (!isConnected)
        {
            Console.WriteLine("Could not connect to email server");
            await host.StopAsync();
            return;
        }

        var notifiers = scope.ServiceProvider.GetServices<INotifier>();
        if (!notifiers.Any())
        {
            Console.WriteLine("No notifiers found");
            await host.StopAsync();
            return;
        }
    }
    
}