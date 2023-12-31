﻿using Microsoft.Extensions.Configuration;
using WorkLogger.Application;
using WorkLogger.Domain;
using WorkLogger.Persistance;
using WorkLogger.Ui.ConsoleUi;

AppDomain.CurrentDomain.UnhandledException += ExceptionHandler;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var settings = config.GetSection("Settings").Get<Settings>() ?? new Settings();

var workDayLength = new WorkDay(settings.WorkdayInMinutes);
var workLog = new WorkLog(workDayLength);

var persistanceManager = new PersistanceManager(settings);
persistanceManager.LoadState(ref workLog);
persistanceManager.Subscribe(workLog);

var formatter = new WorkLogFormatter(workLog);

var ui = new ConsoleUi(workLog, formatter, settings.Shortcuts);

ui.Run();

static void ExceptionHandler(object sender, UnhandledExceptionEventArgs e)
{
    Console.WriteLine("Exception occurred:");
    var exception = (Exception)e.ExceptionObject;
    Console.WriteLine(exception.Message);
    Console.WriteLine(exception.InnerException?.Message);
    Console.WriteLine("Press enter to exit");
    Console.ReadLine();
}
