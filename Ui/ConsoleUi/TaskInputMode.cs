using WorkLogger.Domain;

namespace WorkLogger.Ui.ConsoleUi;

public class TaskInputMode : UiMode
{
    private readonly WorkLogFormatter _workLogFormatter;
    private readonly WorkLog _workLog;
    private readonly HashSet<string> _shortcuts = [];

    public TaskInputMode(WorkLog workLog, WorkLogFormatter workLogFormatter, string[] shortcuts)
    {
        _workLogFormatter = workLogFormatter;
        _workLog = workLog;
        _shortcuts = [.. shortcuts, .. workLog.Tasks.Select(t => t.Name)];
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
        Console.WriteLine(NameShortCuts());
    }

    private string NameShortCuts()
    {
        var records = string.Join(
            ", ",
            _shortcuts.Select((d, i) => $"[{i}] {d}")
        );

        return $"Shortcuts: {records}";
    }

    private string AskForNewTask()
    {
        Console.Write("New task: ");
        return Console.ReadLine() ?? string.Empty;
    }

private void HandleNewTaskRequest(string input)
{
    WorkLogTask workLogTask;
    if (int.TryParse(input, out var index))
    {
        if (index < 0 || index >= _shortcuts.Count)
        {
            ConsoleExt.WriteWarning("Index out of range. Press enter to try again.");
            Console.ReadLine();
            return;
        }

        workLogTask = new WorkLogTask(_shortcuts.ElementAt(index));
    }
    else
    {
        workLogTask = new WorkLogTask(input);
    }

    _workLog.LogWork(workLogTask);
    _shortcuts.Add(workLogTask.Name);
}
}
