using WorkLogger.Application;
using WorkLogger.Common;

namespace WorkLogger.Domain;

public class WorkLog : IObservable<WorkLog>
{
    public List<WorkLogTask> Tasks { get; set; } = new();
    public TimeSpan TotalTime => Tasks.Aggregate(TimeSpan.Zero, (x,y) => x + y.Time);
    public TimeSpan FullTime { get; init; } = TimeSpan.Zero;
    private HashSet<IObserver<WorkLog>> _observers = new();

    public WorkLog()
    {
    }

    public WorkLog(WorkDay workDay)
    {
        FullTime = workDay.Length;
    }
    
    public void LogWork(WorkLogTask task)
    {
        if (!task.IsNew())
        {
            throw new ArgumentException("Task is not new.");
        }

        if (task.Start < Tasks.Max(x => x.End))
        {
            throw new ArgumentException("Task start time is before previous task end time.");
        }

        AddTask(task);

        StateChanged();
    }

    private void AddTask(WorkLogTask record)
    {
        if (Tasks.Count > 0)
        {
            Tasks.Last().Finish();
        }
        Tasks.Add(record);
    }

    public void CloseLastTask()
    {
        if (Tasks.Count > 0)
        {
            Tasks.Last().Finish();
        }

        StateChanged();
    }

    public void ModifyTask(
        int taskIndex,
        TaskTimesModificationRequest taskTimesModificationRequest
    ) {
        if (taskIndex >= Tasks.Count || taskIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(taskIndex));
        }

        var record = Tasks[taskIndex];
        record.ModifyTimes(taskTimesModificationRequest);

        if (taskIndex > 0 && taskTimesModificationRequest.ChangesStart)
        {
            var previousRecord = Tasks[taskIndex - 1];
            previousRecord.ModifyEnd(record.Start.AddSeconds(-1));
        }
        
        if (taskIndex < Tasks.Count - 1 && taskTimesModificationRequest.ChangesEnd)
        {
            var nextRecord = Tasks[taskIndex + 1];

            if (record.End is not null)
            {
                nextRecord.ModifyStart(record.End.Value.AddSeconds(1));
            }
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
