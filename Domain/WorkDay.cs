namespace WorkLogger.Domain;

public class WorkDay
{
    public WorkDay(TimeSpan length)
    {
        ValidateLengthTimeSpan(length);

        Length = length;
    }

    public WorkDay(int lengthInMinutes)
    {
        ValidateLengthTimeSpan(TimeSpan.FromMinutes(lengthInMinutes));

        Length = TimeSpan.FromMinutes(lengthInMinutes);
    }

    private static void ValidateLengthTimeSpan(TimeSpan length)
    {
        if (length <= TimeSpan.Zero || length >= TimeSpan.FromDays(1))
        {
            throw new ArgumentOutOfRangeException(nameof(length), "Length must be greater than 0 minutes and less than 24 hours.");
        }
    }

    public TimeSpan Length { get; init; }
}
