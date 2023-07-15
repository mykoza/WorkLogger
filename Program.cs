using Microsoft.Extensions.Configuration;
using Namespace;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var settings = config.GetSection("Settings").Get<Settings>() ?? new Settings();

var log = new WorkLog(settings);
log.Boot();

while (true)
{
    Console.Clear();
    Console.WriteLine(log.Info());
    Console.WriteLine(log.NameShortCuts());
    Console.Write("New task: ");

    var input = Console.ReadLine();

    if (string.IsNullOrEmpty(input))
    {
        ConsoleExt.WriteColor("Are you sure? (y/n): ", ConsoleColor.Yellow);

        var confirmation = Console.ReadLine();

        if (string.IsNullOrEmpty(confirmation) || confirmation == "y")
        {
            log.CloseLastTask();
            Console.WriteLine("");
            Console.WriteLine("Aggregated times:");
            Console.WriteLine(log.AggregatedTimes());
            Console.Write("Waiting for any key to exit...");
            Console.ReadLine();
            log.CleanUp();
            break;
        }

        continue;
    }

    try
    {
        log.LogWork(input);
    }
    catch (ArgumentOutOfRangeException)
    {
        Console.Write("Index out of range. Press enter to try again.");
        Console.ReadLine();
    }
}