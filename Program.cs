using Microsoft.Extensions.Configuration;
using WorkLogger;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var settings = config.GetSection("Settings").Get<Settings>() ?? new Settings();

var workLog = new WorkLog(settings);

var persistanceManager = new PersistanceManager(settings);
persistanceManager.LoadState(ref workLog);
persistanceManager.Subscribe(workLog);

var formatter = new WorkLogFormatter(workLog);

var ui = new ConsoleUi(workLog, formatter);

ui.Run();
