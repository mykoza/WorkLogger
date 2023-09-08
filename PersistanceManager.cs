using Newtonsoft.Json;

namespace Namespace;
public class PersistanceManager : IObserver<WorkLog>
{
    private WorkLog? _workLog;
    private IDisposable? _cancellation;
    private readonly string _stateDirectory = "state";
    private readonly string _stateFileName = DateTime.Today.ToString("yyyy-MM-dd") + ".json";
    private readonly string _statePath = string.Empty;
    private readonly int _stateFilesToKeep = 1;

    public PersistanceManager(Settings settings)
    {
        _statePath = Path.Combine(_stateDirectory, _stateFileName);
        _stateFilesToKeep = settings.StateFilesToKeep;
    }

    private async void PersistState()
    {
        var json = JsonConvert.SerializeObject(_workLog, Formatting.Indented);

        Directory.CreateDirectory("state");

        using var file = File.CreateText(_statePath);
        await file.WriteAsync(json);
    }

    public void LoadState(ref WorkLog workLog)
    {
        try
        {
            var json = File.ReadAllText(_statePath);
            var obj = JsonConvert.DeserializeObject<WorkLog>(json);

            if (obj is not null)
            {
                workLog.Records = obj.Records;
                workLog.Shortcuts = obj.Shortcuts;
            }
        }
        catch (FileNotFoundException)
        {
            //
        }
        catch (DirectoryNotFoundException)
        {
            //
        }
    }

    public void DeleteOldStates()
    {
        var stateDir = new DirectoryInfo(_stateDirectory);
        var files = stateDir.GetFiles();
        Array.Sort(files, (x, y) => y.CreationTime.CompareTo(x.CreationTime));

        if (files.Length <= _stateFilesToKeep)
        {
            return;
        }

        foreach (var item in files[_stateFilesToKeep..])
        {
            item.Delete();
        }
    }

    public virtual void Subscribe(WorkLog workLog)
    {
        _cancellation = workLog.Subscribe(this);
    }

    public virtual void Unsubscribe()
    {
        _cancellation?.Dispose();
        _workLog = null;
    }

    public void OnCompleted()
    {
        _workLog = null;
        DeleteOldStates();
    }

    public void OnError(Exception error)
    {
        throw new NotImplementedException();
    }

    public void OnNext(WorkLog value)
    {
        _workLog ??= value;

        PersistState();
    }

}
