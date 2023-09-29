namespace Namespace;
public class ConsoleUi
{
    private readonly WorkLog _workLog;
    private readonly WorkLogFormatter _workLogFormatter;

    public ConsoleUi(WorkLog workLog, WorkLogFormatter workLogFormatter)
    {
        _workLog = workLog;
        _workLogFormatter = workLogFormatter;
    }

    public void Run()
    {
        Loop();
        Shutdown();
        WriteWaitForExit();
    }

    private void Loop()
    {
        while (true)
        {
            Console.Clear();
            WriteInfo();
            WriteNameShortCuts();

            var input = AskForNewTask();

            if (ExitRequested(input))
            {
                break;
            }

            HandleNewTaskRequest(input);
        }
    }

    private void WriteInfo()
    {
        Console.WriteLine(_workLogFormatter.Info());
    }

    private void WriteNameShortCuts()
    {
        Console.WriteLine(_workLogFormatter.NameShortCuts());
    }

    private string AskForNewTask()
    {
        Console.Write("New task: ");
        return Console.ReadLine() ?? string.Empty;
    }

    private bool ExitRequested(string input)
    {
        if (! string.IsNullOrEmpty(input))
        {
            return false;
        }

        var confirmation = AskForExitConfirmation();

        if (string.IsNullOrEmpty(confirmation) || confirmation == "y")
        {
            return true;
        }

        return false;
    }

    private void Shutdown()
    {
        _workLog.CloseLastTask();
        _workLog.Finish();

        Console.Clear();
        WriteInfo();
        Console.WriteLine("");
        WriteAggregates();
    }

    private void HandleNewTaskRequest(string input)
    {
        try
        {
            _workLog.LogWork(input);
        }
        catch (ArgumentOutOfRangeException)
        {
            Console.Write("Index out of range. Press enter to try again.");
            Console.ReadLine();
        }
    }

    private void WriteAggregates()
    {
        Console.WriteLine("Aggregated times:");
        Console.WriteLine(_workLogFormatter.AggregatedTimes());
    }

    private string? AskForExitConfirmation()
    {
        ConsoleExt.WriteColor("Are you sure? (y/n): ", ConsoleColor.Yellow);
        return Console.ReadLine();
    }

    private void WriteWaitForExit()
    {
        Console.Write("Waiting for any key to exit...");
        Console.ReadLine();
    }
}
