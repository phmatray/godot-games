using System.Diagnostics;

namespace Raga.Server.Features.Info.Services;


public class UptimeService
{
    private readonly Stopwatch _stopwatch;

    public UptimeService()
    {
        _stopwatch = Stopwatch.StartNew();
    }
    
    public string GetUptime()
    {
        return _stopwatch.Elapsed.ToString();
    }
}