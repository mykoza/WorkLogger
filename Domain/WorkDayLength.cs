namespace WorkLogger.Domain;

public class WorkDayLength
{
    public WorkDayLength(TimeSpan length)
    {
        ValidateLengthTimeSpan(length);

        Length = length;
    }

    public WorkDayLength(int lengthInMinutes)
    {
        ValidateLengthTimeSpan(TimeSpan.FromMinutes(lengthInMinutes));

        Length = TimeSpan.FromMinutes(lengthInMinutes);
    }

    private void ValidateLengthTimeSpan(TimeSpan length)
    {
        if (length <= TimeSpan.Zero || length >= TimeSpan.FromDays(1))
        {
            throw new ArgumentOutOfRangeException(nameof(length), "Length must be greater than 0 minutes and less than 24 hours.");
        }
    }

    public TimeSpan Length { get; init; }   
}
