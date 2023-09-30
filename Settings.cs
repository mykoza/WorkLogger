namespace WorkLogger;
public class Settings
{
    public int WorkdayInMinutes { get; set; } = 480;
    public string[] Shortcuts { get; set; } = {
        "HD",
        "Meeting",
        "Przerwa"
    };
    public int StateFilesToKeep { get; set; } = 5;
}
