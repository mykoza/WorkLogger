namespace WorkLogger;

public class ChangeDurationRequest
{
    public TimeSpan Duration { get; init; }
    public TimeCalculationTarget TimeCalculationTarget { get; init; } = TimeCalculationTarget.End;

    public ChangeDurationRequest(TimeSpan duration)
    {
        if (!ValidateDuration(duration))
        {
            throw new ArgumentOutOfRangeException(nameof(duration));
        }

        Duration = duration;
    }
    
    public ChangeDurationRequest(TimeSpan duration, TimeCalculationTarget timeCalculationTarget)
    {
        if (!ValidateDuration(duration))
        {
            throw new ArgumentOutOfRangeException(nameof(duration));
        }

        Duration = duration;
        TimeCalculationTarget = timeCalculationTarget;
    }

    private bool ValidateDuration(TimeSpan duration) => duration > TimeSpan.Zero;
}
