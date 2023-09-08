using Microsoft.Extensions.Configuration;
using Namespace;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var settings = config.GetSection("Settings").Get<Settings>() ?? new Settings();

var log = new WorkLog(settings);

var ui = new ConsoleUi(log);

ui.Run();
