namespace WorkLogger.Domain;

public class WorkDayLength
{
    public WorkDayLength(TimeSpan length)
    {
        Length = length;
    }

    public WorkDayLength(int lengthInMinutes)
    {
        Length = TimeSpan.FromMinutes(lengthInMinutes);
    }

    public TimeSpan Length { get; set; }   
}
