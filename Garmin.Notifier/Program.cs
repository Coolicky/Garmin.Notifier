using Garmin.Notifier;
using Garmin.Notifier.Email;
using Garmin.Notifier.Notifiers;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<IEmailReader, EmailReader>();
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("Imap"));
builder.Services.AddHostedService<Worker>();
builder.Services.RegisterNotifiers(builder.Configuration);

var host = builder.Build();
host.Run();