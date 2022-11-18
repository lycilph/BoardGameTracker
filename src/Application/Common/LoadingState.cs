namespace BoardGameTracker.Application.Common;

public class LoadingState : IDisposable
{
    public bool IsLoading { get; set; }
    public Action FinishAction { get; set; }

    public LoadingState(Action finishAction)
    {
        FinishAction = finishAction;
    }

    public LoadingState Load()
    {
        IsLoading = true;
        return this;
    }

    public void Dispose()
    {
        IsLoading = false;
        FinishAction();
    }
}
