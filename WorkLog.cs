using System.Text;
using Newtonsoft.Json;

namespace Namespace;
public class WorkLog
{
    public List<WorkLogRecord> Records { get; set; } = new();
    // public List<WorkLogRecord> AggredatedRecords { get; private set; } = new();
    public TimeSpan TotalTime => Records.Aggregate(TimeSpan.Zero, (x,y) => x + y.Time);
    public TimeSpan FullTime { get; init; }
    public HashSet<string> Shortcuts { get; set; }

    public WorkLog(Settings settings)
    {
        FullTime = new TimeSpan(0, settings.WorkdayInMinutes, 0);
        Shortcuts = settings.Shortcuts.ToHashSet();
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

    public void AddRecord(WorkLogRecord record)
    {
        if (Records.Count > 0)
        {
            Records.Last().Finish();
        }
        Records.Add(record);
        Shortcuts.Add(record.Name);
    }

    public void AddRecord(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException($"{nameof(name)} cannot be empty.");
        }

        AddRecord(new WorkLogRecord(name));
    }

    public void AddRecord(int index)
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
                        Time = times.Aggregate((agg, time) => agg + time)
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
        using var file = File.CreateText("state.json");
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
            var json = File.ReadAllText("state.json");
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
    }

    public void CleanUp()
    {
        DeleteState();
    }

    // Delete persisted state
    private void DeleteState()
    {
        File.Delete("state.json");
    }
}
