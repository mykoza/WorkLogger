using WorkLogger.Domain;

namespace WorkLogger.Ui.ConsoleUi;

public class TaskInputMode : UiMode
{
    private readonly WorkLogFormatter _workLogFormatter;
    private readonly WorkLog _workLog;

    public TaskInputMode(WorkLog workLog, WorkLogFormatter workLogFormatter)
    {
        _workLogFormatter = workLogFormatter;
        _workLog = workLog;
    }

    public override void Run()
    {
        Loop();
    }

    protected override void Loop()
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

    protected override bool ExitRequested(string input)
    {
        return string.IsNullOrEmpty(input);
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
}
