using System.Text.RegularExpressions;
using MailKit;
using MailKit.Net.Imap;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Garmin.Notifier.Email;

public class EmailReader : IDisposable, IEmailReader
{
    private readonly EmailOptions _options;
    private ImapClient _client;
    private string? _url;

    public EmailReader(IOptions<EmailOptions> options)
    {
        _options = options.Value;
        _client = new ImapClient();
    }

    public async Task<bool> Connect()
    {
        try
        {
            await _client.ConnectAsync(_options.Host, _options.Port, _options.UseSsl);
            await _client.AuthenticateAsync(_options.Username, _options.Password);
            await _client.DisconnectAsync(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
        return true;
    }

    public async Task<bool> IsEmailAvailable(CancellationToken stoppingToken, DateTime lastRun)
    {
        try
        {
            await _client.ConnectAsync(_options.Host, _options.Port, _options.UseSsl, stoppingToken);
            await _client.AuthenticateAsync(_options.Username, _options.Password, stoppingToken);

            var inbox = _client.Inbox;
            await inbox.OpenAsync(FolderAccess.ReadOnly, stoppingToken);

            if (inbox.Count < 1) return false;

            var current = inbox.Count;
            while (inbox.Count > 0 && current > 0)
            {
                var message = await inbox.GetMessageAsync(inbox.Count - 1, stoppingToken);
                if (message.Date < lastRun)
                {
                    break;
                }

                if (!IsGarminMessage(message)) continue;
                _url = GetGarminUrl(message);
                if (_url != null)
                {
                    await _client.DisconnectAsync(true, stoppingToken);
                    return true;
                }

                current--;
            }

            await _client.DisconnectAsync(true, stoppingToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        return false;
    }

    public string? GetUrl()
    {
        return _url;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_client.IsConnected)
                _client.Disconnect(true);
            _client.Dispose();
        }
    }

    private static bool IsGarminMessage(MimeMessage message)
    {
        if (!message.From.Any(r => r.ToString().Contains("Garmin")))
            return false;
        if (!Regex.IsMatch(message.Subject, "Watch\\s+.+?\\s+live activity now!"))
            return false;
        if (!message.HtmlBody.Contains("livetrack.garmin.com"))
            return false;
        return true;
    }

    private static string? GetGarminUrl(MimeMessage message)
    {
        var hrefRegex = new Regex(@"href=""https:\/\/livetrack\.garmin\.com\/session\/[^""]+""");
        var hrefMatch = hrefRegex.Match(message.HtmlBody);
        if (!hrefMatch.Success) return null;

        return hrefMatch.Value.Replace("href=", "").Replace("\"", "").Trim();
    }
}