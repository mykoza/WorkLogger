// See https://aka.ms/new-console-template for more information
using Namespace;

Console.WriteLine("Hello, World!");

var log = new WorkLog();
// log.AddRecord(new WorkLogRecord("HD", new DateTime(2022, 7,6,8,0,0), new DateTime(2022, 7,6,8,30,0)));
// log.AddRecord(new WorkLogRecord("Meetings", new DateTime(2022, 7,6,8,30,0), new DateTime(2022, 7,6,9,0,0)));

while (true)
{
    Console.Clear();
    Console.WriteLine(log.Info());
    Console.WriteLine(log.NameShortCuts());
    Console.Write("New task: ");

    var input = Console.ReadLine();

    if (string.IsNullOrEmpty(input))
    {
        Console.Write("Are you sure? (y/n): ");

        var confirmation = Console.ReadLine();

        if (string.IsNullOrEmpty(confirmation) || confirmation == "y")
        {
            Console.WriteLine("Aggregated times:");
            Console.WriteLine(log.AggregatedTimes());
            Console.Write("Waiting for any key to exit.");
            Console.ReadLine();
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

    // Console.WriteLine(input);
}