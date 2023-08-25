using System.Text;
using Newtonsoft.Json;

namespace Namespace;
public class WorkLog
{
    public List<WorkLogRecord> Records { get; set; } = new();
    public TimeSpan TotalTime => Records.Aggregate(TimeSpan.Zero, (x,y) => x + y.Time);
    public TimeSpan FullTime { get; init; } = TimeSpan.Zero;
    public HashSet<string> Shortcuts { get; set; } = new();
    private string StateDirectory { get; init; } = "state";
    private string StateFileName { get; init; } = DateTime.Today.ToString("yyyy-MM-dd") + ".json";
    private string StatePath { get; init; } = String.Empty;
    private int StateFilesToKeep { get; init; } = 1;

    public WorkLog()
    {
        StatePath = Path.Combine(StateDirectory, StateFileName);
    }

    public WorkLog(Settings settings)
    {
        FullTime = new TimeSpan(0, settings.WorkdayInMinutes, 0);
        Shortcuts = settings.Shortcuts.ToHashSet();
        StatePath = Path.Combine(StateDirectory, StateFileName);
        StateFilesToKeep = settings.StateFilesToKeep;
    }

    public void LogWork(string input)
    {
        if (int.TryParse(input, out var index))
        {
            AddRecord(index);
        }
        else
        {
            AddRecord(input);
        }

        PersistState();
    }

    private void AddRecord(WorkLogRecord record)
    {
        if (Records.Count > 0)
        {
            Records.Last().Finish();
        }
        Records.Add(record);
        Shortcuts.Add(record.Name);
    }

    private void AddRecord(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException($"{nameof(name)} cannot be empty.");
        }

        AddRecord(new WorkLogRecord(name));
    }

    private void AddRecord(int index)
    {
        if (index >= Shortcuts.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        AddRecord(new WorkLogRecord(Shortcuts.ElementAt(index)));
    }

    public string Info()
    {
        var builder = new StringBuilder();

        builder.AppendLine(TotalTimes());
        builder.Append("Previous tasks: ");
        builder.AppendLine(RecordTimes());

        if (Records.Count > 0)
        {
            builder.Append("Current task: ");

            var last = Records.Last();
            builder.AppendLine($"{last.Name}, started at {last.Start.ToString("HH:mm")}");
        }
        else
        {
            builder.AppendLine("Work not started");
        }

        return builder.ToString();
    }

    public string NameShortCuts()
    {
        var records = string.Join(
            ", ", 
            Shortcuts.Select((d, i) => $"{d} [{i}]")
        );

        return $"Shortcuts: {records}";
    }

    public void CloseLastTask()
    {
        if (Records.Count > 0)
        {
            Records.Last().Finish();
        }
        PersistState();
    }

    private string RecordTimes()
    {
        return string.Join(
            " | ", 
            Records
                .Where(record => record.Time > TimeSpan.Zero)
                .Select(record => $"{record.Name}: {FormatTimeSpan(record.Time)}")
        );
    }

    private string TotalTimes()
    {
        var total = TotalTime.ToString(@"h\:mm\:ss");
        var remaining = (FullTime - TotalTime).ToString(@"h\:mm\:ss");
        return $"Total time: {total} | Remaining time: {remaining}";
    }

    public string AggregatedTimes()
    {
        return string.Join(
            " | ",
            Records
                .GroupBy(
                    x => x.Name,
                    x => x.Time,
                    (name, times) => new {
                        Name = name,
                        Time = times.Aggregate((agg, time) => agg + time),
                    }
                )
                .Select(record => $"{record.Name}: {FormatTimeSpan(record.Time)}")
        );
    }

    private static string FormatTimeSpan(TimeSpan timeSpan)
    {
        if (timeSpan.Hours == 0)
        {
            return $"{timeSpan.Minutes}m";
        }
        else
        {
            return $"{timeSpan.Hours}h {timeSpan.Minutes}m";
        }
    }

    // Persist state in json file using System.Text.Json
    private async void PersistState()
    {
        var json = JsonConvert.SerializeObject(this, Formatting.Indented);

        // asynchronously write json to state.json file
        Directory.CreateDirectory("state");
        using var file = File.CreateText(StatePath);
        await file.WriteAsync(json);
    }

    public void Boot()
    {
        LoadState();
    }

    // Read state from file
    private void LoadState()
    {
        try
        {
            var json = File.ReadAllText(StatePath);
            var obj = JsonConvert.DeserializeObject<WorkLog>(json);

            if (obj is not null)
            {
                Records = obj.Records;
                Shortcuts = obj.Shortcuts;
            }
        }
        catch (FileNotFoundException)
        {
            return;
        }
        catch (DirectoryNotFoundException)
        {
            return;
        }
    }

    public void CleanUp()
    {
        DeleteOldStates();
    }

    private void DeleteOldStates()
    {
        var stateDir = new DirectoryInfo(StateDirectory);
        var files = stateDir.GetFiles();
        Array.Sort(files, (x, y) => y.CreationTime.CompareTo(x.CreationTime));

        if (files.Length <= StateFilesToKeep)
        {
            return;
        }

        foreach (var item in files[StateFilesToKeep..])
        {
            item.Delete();
        }
    }
}
