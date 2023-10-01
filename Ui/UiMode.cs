namespace WorkLogger.Ui;

public abstract class UiMode
{
    public abstract void Run();
    protected abstract void Loop();
    protected abstract bool ExitRequested(string input);
}
