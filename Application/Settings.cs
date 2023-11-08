namespace WorkLogger.Application;
public class Settings
{
    private string _appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

    public int WorkdayInMinutes { get; set; } = 480;
    public string[] Shortcuts { get; set; } = {
        "HD",
        "Meeting",
        "Przerwa"
    };
    public int StateFilesToKeep { get; set; } = 5;
    public string AppDataPath
    {
        get => _appDataPath;
        set
        {
            if (!Path.Exists(value))
            {
                throw new DirectoryNotFoundException($"Directory '{value}' does not exist.");
            }

            _appDataPath = value;
        }
    }
}
