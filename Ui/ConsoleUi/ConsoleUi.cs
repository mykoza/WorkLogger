using WorkLogger.Application;
using WorkLogger.Domain;

namespace WorkLogger.Ui.ConsoleUi;

public class ConsoleUi
{
    private readonly WorkLog _workLog;
    private readonly WorkLogFormatter _workLogFormatter;
    private readonly UiMode _inputMode;
    private readonly UiMode _modificationMode;
    private string _taskInputModeShortcut = "i";
    private string _taskModificationModeShortcut = "m";

    public ConsoleUi(WorkLog workLog, WorkLogFormatter workLogFormatter, string[] inputShortcuts)
    {
        _workLog = workLog;
        _workLogFormatter = workLogFormatter;
        _inputMode = new TaskInputMode(workLog, workLogFormatter, inputShortcuts);
        _modificationMode = new TaskModificationMode(workLog, workLogFormatter);
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
            if (_workLog.Tasks.Count == 0)
            {
                _inputMode.Run();
                continue;
            }

            Console.WriteLine("Which mode do you want to use? [i] insert, [m] modification, empty to quit");
            var input = Console.ReadLine() ?? string.Empty;
    
            if (ExitRequested(input))
            {
                return;
            }
            else if (TaskInputModeRequested(input))
            {
                _inputMode.Run();
            }
            else if (TaskModificationModeRequested(input))
            {
                _modificationMode.Run();
            }
        }
    }

    private void WriteInfo()
    {
        Console.WriteLine(_workLogFormatter.Info());
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

    private bool TaskInputModeRequested(string input)
    {
        return input == _taskInputModeShortcut;
    }

    private bool TaskModificationModeRequested(string input)
    {
        return input == _taskModificationModeShortcut;
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

    private void WriteAggregates()
    {
        Console.WriteLine("Aggregated times:");
        Console.WriteLine(_workLogFormatter.AggregatedTimes());
    }

    private string? AskForExitConfirmation()
    {
        ConsoleExt.WriteWarning("Are you sure you want to quit the application? (y/n): ");
        return Console.ReadLine();
    }

    private void WriteWaitForExit()
    {
        Console.Write("Waiting for any key to exit...");
        Console.ReadLine();
    }
}
