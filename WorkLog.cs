using System.Linq;
using System.Text;

namespace Namespace;
public class WorkLog
{
    public List<WorkLogRecord> Records { get; set; } = new();
    // public List<WorkLogRecord> AggredatedRecords { get; private set; } = new();
    public TimeSpan TotalTime => Records.Aggregate(TimeSpan.Zero, (x,y) => x + y.Time);
    public TimeSpan FullTime { get; } = new TimeSpan(7, 30, 0);
    public HashSet<string> Shortcuts { get; private set; } = new() { "HD", "Meetings", "Przerwa" };

    public WorkLog()
    {
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

    private string RecordTimes()
    {
        return string.Join(
            " | ", 
            Records
                .Where(record => record.Time > TimeSpan.Zero)
                .Select(record => $"{record.Name}: {record.Time.ToString(@"%m")}m")
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
                .Select(record => $"{record.Name}: {record.Time.ToString(@"%m")}m")
        );
    }

}
