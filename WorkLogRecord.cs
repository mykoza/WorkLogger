namespace Namespace;
public class WorkLogRecord
{
    public string Name { get; set;}
    public DateTime Start { get; private set; } = DateTime.Now;
    public DateTime? End { get; private set; }
    public TimeSpan Time { get; private set; }
    public string Comment { get; set; } = string.Empty;

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

    public void Finish()
    {
        End = DateTime.Now;
        Time = (TimeSpan)(End - Start);
    }

    public void AddTime(TimeSpan time)
    {
        Time += time;
        End = null;
    }

    public WorkLogRecord Clone()
    {
        return new WorkLogRecord(Name);
    }

    // public WorkLogRecord(string name, TimeSpan time)
    // {
    //     Name = name;
    //     Time = time;
    // }

    // public WorkLogRecord(string name, TimeSpan time, string comment)
    // {
    //     Name = name;
    //     Time = time;
    //     Comment = comment;
    // }

}
