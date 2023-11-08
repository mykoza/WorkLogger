namespace WorkLogger.Common;

internal sealed class Unsubscriber<WorkLog> : IDisposable
{
    private readonly ISet<IObserver<WorkLog>> _observers;
    private readonly IObserver<WorkLog> _observer;

    internal Unsubscriber(
        ISet<IObserver<WorkLog>> observers,
        IObserver<WorkLog> observer
    ) => (_observers, _observer) = (observers, observer);

    public void Dispose() => _observers.Remove(_observer);
}
