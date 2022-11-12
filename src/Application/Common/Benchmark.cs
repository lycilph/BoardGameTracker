using System.Diagnostics;

namespace BoardGameTracker.Application.Common;

public class Benchmark : IDisposable
{
    private readonly Stopwatch timer;

    public Benchmark(Stopwatch timer)
    {
        this.timer = timer;
        timer.Start();
    }

    public void Dispose() => timer.Stop();
}