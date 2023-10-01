using WorkLogger.Ui;

namespace WorkLogger;

public class TaskModificationMode : UiMode
{
    private WorkLog _workLog;
    private WorkLogFormatter _workLogFormatter;

    public TaskModificationMode(WorkLog workLog, WorkLogFormatter workLogFormatter)
    {
        _workLog = workLog;
        _workLogFormatter = workLogFormatter;
    }

    public override void Run()
    {
        Loop();
    }

    protected override bool ExitRequested(string input)
    {
        return string.IsNullOrEmpty(input);
    }

    protected override void Loop()
    {
        while (true)
        {
            string input = AskForTaskIndex();

            if (ExitRequested(input))
            {
                break;
            }

            HandleTaskModificationRequest(input);
        }
    }

    private void HandleTaskModificationRequest(string input)
    {
        if (!int.TryParse(input, out var taskIndex))
        {
            Console.Write("Input provided is not a valid index. Press enter to try again.");
            Console.ReadLine();
            return;
        }

        var task = _workLog.Tasks[taskIndex];
        Console.WriteLine(_workLogFormatter.TaskDetails(taskIndex));

        DateTime? start = default;
        DateTime? end = default;
        TimeSpan duration = TimeSpan.Zero;
        TimeCalculationTarget target = TimeCalculationTarget.Start;

        while (true)
        {
            Console.WriteLine("What to modify? [s] start, [e] end, [ds] duration - change start, [de] duration - change end");
            var modificationTypeInput = Console.ReadLine() ?? string.Empty;

            if (modificationTypeInput.Trim() == "s")
            {
                string v = task.Start.ToString("yyyy-MM-dd HH:mm:ss");
                Console.WriteLine($"Modifying start. Currently set to: {v}");
                var startModificationInput = Console.ReadLine();

                if (!DateTime.TryParse(startModificationInput, out DateTime tryStart))
                {
                    Console.WriteLine("Input provided is not a valid date. Press enter to try again.");
                    continue;
                }

                start = tryStart;
            }
            else if (modificationTypeInput.Trim() == "e")
            {
                string v = task.End?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty;
                Console.WriteLine($"Modifying end. Currently set to: {v}");
                var endModificationInput = Console.ReadLine();

                if (!DateTime.TryParse(endModificationInput, out DateTime tryEnd))
                {
                    Console.WriteLine("Input provided is not a valid date. Press enter to try again.");
                    continue;
                }

                end = tryEnd;
            }
            else if (modificationTypeInput.Trim() == "ds")
            {
                target = TimeCalculationTarget.Start;
                string v = WorkLogFormatter.FormatTimeSpan(task.Time);
                Console.Write($"Modifying duration. Currently set to: {v}. Start will be modified.");
                var durationModificationInput = Console.ReadLine();

                if (!TimeSpan.TryParse(durationModificationInput, out duration))
                {
                    Console.WriteLine("Input provided is not a valid duration. Press enter to try again.");
                    continue;
                }
            }
            else if (modificationTypeInput.Trim() == "de")
            {
                target = TimeCalculationTarget.End;
                string v = WorkLogFormatter.FormatTimeSpan(task.Time);
                Console.Write($"Modifying duration. Currently set to: {v}. End will be modified.");
                var durationModificationInput = Console.ReadLine();

                if (!TimeSpan.TryParse(durationModificationInput, out duration))
                {
                    Console.WriteLine("Input provided is not a valid duration. Press enter to try again.");
                    continue;
                }
            }
            else
            {
                break;
            }
        }

        ChangeDurationRequest? changeDurationRequest = null;
        if (duration > TimeSpan.Zero)
        {
            changeDurationRequest = new(duration, target);
        }

        // if (start is not null || end is not null || changeDurationRequest is not null)
        // {
        var modificationRequest = new TaskTimesModificationRequest(
            start,
            end,
            changeDurationRequest
        );

        _workLog.ModifyTask(taskIndex, modificationRequest);
        // }
    }

    private string AskForTaskIndex()
    {
        Console.WriteLine(_workLogFormatter.ListOfTasksWithDurations());
        Console.Write("Task to modify, empty to return: ");
        var index = Console.ReadLine() ?? string.Empty;
        return index;
    }
}
