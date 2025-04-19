namespace Garmin.Notifier.Notifiers;

public interface INotifierOptions
{
    static abstract string Prefix { get; }
    bool IsValid();
}