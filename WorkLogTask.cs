namespace Namespace;
public class WorkLogTask
{
    public string Name { get; set; } = string.Empty;
    public DateTime Start { get; set; } = DateTime.Now;
    public DateTime? End { get; set; }
    public TimeSpan Time { get; set; }
    public string Comment { get; set; } = string.Empty;

    public WorkLogTask()
    {
    }

    public WorkLogTask(string name)
    {
        Name = name;
    }

    public WorkLogTask(string name, DateTime start, DateTime end)
    {
        Name = name;
        Start = start;
        End = end;
    }

    public WorkLogTask(string name, string start, string? end, string time)
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
        Time = Time.Add(time);
        End = null;
    }

    public void ModifyTimes(TaskTimesModificationRequest taskTimesModificationRequest)
    {
        if (taskTimesModificationRequest.Start is not null)
        {
            Start = taskTimesModificationRequest.Start.Value;
        }

        if (taskTimesModificationRequest.End is not null)
        {
            End = taskTimesModificationRequest.End.Value;
        }

        if (taskTimesModificationRequest.ChangeDurationRequest is not null)
        {
            ModifyDuration(taskTimesModificationRequest.ChangeDurationRequest.Duration, taskTimesModificationRequest.ChangeDurationRequest.TimeCalculationTarget);
            return;
        }

        CalculateTime();
    }

    public void ModifyStart(DateTime start)
    {
        Start = start;
        
        if (End is not null)
        {
            CalculateTime();
        }
    }

    public void ModifyEnd(DateTime end)
    {
        End = end;
        CalculateTime();
    }

    public void ModifyDuration(TimeSpan duration, TimeCalculationTarget timeCalculationTarget = TimeCalculationTarget.End)
    {
        Time = duration;

        if (timeCalculationTarget == TimeCalculationTarget.End)
        {
            CalculateEndFromStart(Time);
        }
        else if (timeCalculationTarget == TimeCalculationTarget.Start)
        {
            CalculateStartFromEnd(Time);
        }
    }

    private void CalculateEndFromStart(TimeSpan timeSpan)
    {
        End = Start + timeSpan;
    }

    private void CalculateStartFromEnd(TimeSpan timeSpan)
    {
        if (End is null)
        {
            throw new InvalidOperationException("Cannot calculate start if end time is not set");
        }
        
        Start = End.Value - timeSpan;
    }

    public void CalculateTime()
    {
        if (End is null)
        {
            throw new InvalidOperationException("Cannot calculate time if end time is not set");
        }

        Time = End.Value - Start;
    }

    public void Finish()
    {
        End = DateTime.Now;
        Time = End.Value - Start;
    }

    public WorkLogTask Clone()
    {
        return new WorkLogTask(Name);
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
