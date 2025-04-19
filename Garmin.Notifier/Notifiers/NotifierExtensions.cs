namespace Garmin.Notifier.Notifiers;

public static class NotifierExtensions
{
    public static IServiceCollection RegisterNotifiers(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var x = typeof(INotifier)
            .Assembly
            .GetTypes()
            .Where(r => r.IsClass && !r.IsAbstract)
            .Where(r => r.GetInterfaces()
                .Any(i => i == typeof(INotifierRegistration)));
        foreach (var r in x)
        {
            var a = Activator.CreateInstance(r);
            if (a is INotifierRegistration notifierRegistration)
            {
                notifierRegistration.Register(services, configuration);
            }
            else
            {
                throw new InvalidOperationException($"Type {r.Name} does not implement INotifierRegistration<INotifier>");
            }
            
        }

        typeof(INotifier)
            .Assembly
            .GetTypes()
            .Where(r => r.IsClass && !r.IsAbstract)
            .Where(r => r.GetInterfaces()
                .Any(i =>
                    i.IsGenericType && 
                    i.GetGenericTypeDefinition() == typeof(INotifierRegistration)))
            .Select(r => Activator.CreateInstance(r) as INotifierRegistration)
            .ToList()
            .ForEach(r => r?.Register(services, configuration));
        return services;
    }
}