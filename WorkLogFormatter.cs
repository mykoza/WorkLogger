using System.Text;

namespace WorkLogger;
public class WorkLogFormatter
{
    private readonly WorkLog _workLog;
    
    public WorkLogFormatter(WorkLog workLog)
    {
        _workLog = workLog;
    }

    public string Info()
    {
        var builder = new StringBuilder();

        builder.AppendLine(TotalTimes());
        builder.Append("Previous tasks: ");
        builder.AppendLine(RecordTimes());

        if (_workLog.Records.Count > 0)
        {
            builder.Append("Current task: ");

            var last = _workLog.Records.Last();
            builder.AppendLine($"{last.Name}, started at {last.Start.ToString("HH:mm")}");
        }
        else
        {
            builder.AppendLine("Work not started");
        }

        return builder.ToString();
    }
    
    private string TotalTimes()
    {
        var total = _workLog.TotalTime.ToString(@"h\:mm\:ss");
        var remaining = (_workLog.FullTime - _workLog.TotalTime).ToString(@"h\:mm\:ss");
        return $"Total time: {total} | Remaining time: {remaining}";
    }

    public string NameShortCuts()
    {
        var records = string.Join(
            ", ", 
            _workLog.Shortcuts.Select((d, i) => $"{d} [{i}]")
        );

        return $"Shortcuts: {records}";
    }
    
    private string RecordTimes()
    {
        return string.Join(
            " | ", 
            _workLog.Records
                .Where(record => record.Time > TimeSpan.Zero)
                .Select(record => $"{record.Name}: {FormatTimeSpan(record.Time)}")
        );
    }

    public string AggregatedTimes()
    {
        return string.Join(
            " | ",
            _workLog.Records
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
}