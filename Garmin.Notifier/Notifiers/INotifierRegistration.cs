namespace Garmin.Notifier.Notifiers;

public interface INotifierRegistration
{
    void Register(IServiceCollection services, IConfiguration configuration);
}