namespace WorkLogger.Application;

public class TaskTimesModificationRequest
{
    public DateTime? Start { get; init; }
    public DateTime? End { get; init; }
    public ChangeDurationRequest? ChangeDurationRequest { get; init; }
    public bool ChangesStart => 
        Start is not null
        || ChangeDurationRequest?.TimeCalculationTarget == TimeCalculationTarget.Start;
    
    public bool ChangesEnd =>
        End is not null
        || ChangeDurationRequest?.TimeCalculationTarget == TimeCalculationTarget.End;

    public TaskTimesModificationRequest(DateTime? start, DateTime? end, ChangeDurationRequest? changeDurationRequest)
    {
        if (end != default && end < start)
        {
            throw new ArgumentException("End cannot be before start.");
        }

        if (start != default && changeDurationRequest?.TimeCalculationTarget == TimeCalculationTarget.Start)
        {
            throw new InvalidOperationException("Cannot modify start by parameter and duration at the same time.");
        }

        if (end != default && changeDurationRequest?.TimeCalculationTarget == TimeCalculationTarget.End)
        {
            throw new InvalidOperationException("Cannot modify end by parameter and duration at the same time.");
        }

        Start = start;
        End = end;
        ChangeDurationRequest = changeDurationRequest;
    }
}
