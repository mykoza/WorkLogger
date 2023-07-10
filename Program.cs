using Namespace;

var log = new WorkLog();

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
}