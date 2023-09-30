namespace WorkLogger;
public class WorkLog : IObservable<WorkLog>
{
    public List<WorkLogRecord> Records { get; set; } = new();
    public TimeSpan TotalTime => Records.Aggregate(TimeSpan.Zero, (x,y) => x + y.Time);
    public TimeSpan FullTime { get; init; } = TimeSpan.Zero;
    public HashSet<string> Shortcuts { get; set; } = new();
    private HashSet<IObserver<WorkLog>> _observers = new();

    public WorkLog()
    {
    }

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

        StateChanged();
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

    public void CloseLastTask()
    {
        if (Records.Count > 0)
        {
            Records.Last().Finish();
        }
        StateChanged();
    }

    public IDisposable Subscribe(IObserver<WorkLog> observer)
    {
        if (_observers.Add(observer))
        {
            observer.OnNext(this);
        }

        return new Unsubscriber<WorkLog>(_observers, observer);
    }

    public void StateChanged()
    {
        foreach (var observer in _observers)
        {
            observer.OnNext(this);
        }
    }

    public void Finish()
    {
        foreach (var observer in _observers)
        {
            observer.OnCompleted();
        }
    }
}
