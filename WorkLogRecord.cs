namespace Namespace;
public class WorkLogRecord
{
    public string Name { get; set; } = string.Empty;
    public DateTime Start { get; set; } = DateTime.Now;
    public DateTime? End { get; set; }
    public TimeSpan Time { get; set; }
    public string Comment { get; set; } = string.Empty;

    public WorkLogRecord()
    {
    }

    public WorkLogRecord(string name)
    {
        Name = name;
    }

    public WorkLogRecord(string name, DateTime start, DateTime end)
    {
        Name = name;
        Start = start;
        End = end;
    }

    public WorkLogRecord(string name, string start, string? end, string time)
    {
        Name = name;
        Start = DateTime.Parse(start);

        if (end is not null)
        {
            End = DateTime.Parse(end);
        }

        Time = TimeSpan.Parse(time);
    }

    public void AddTime(TimeSpan time)
    {
        Time += time;
        End = null;
    }

    public void Finish()
    {
        End = DateTime.Now;
        Time = (TimeSpan)(End - Start);
    }

    public WorkLogRecord Clone()
    {
        return new WorkLogRecord(Name);
    }

    public string FormatAsDuration()
    {
        if (Time.Hours == 0)
        {
            return $"{Time.Minutes}m";
        }
        else
        {
            return $"{Time.Hours}h {Time.Minutes}m";
        }
    }
}
