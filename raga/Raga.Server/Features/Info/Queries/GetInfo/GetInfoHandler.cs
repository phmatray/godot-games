using System.Diagnostics;
using System.Runtime.InteropServices;
using MediatR;

namespace Raga.Server.Features.Info.Queries.GetInfo;

public class GetInfoHandler(
    IConfiguration configuration,
    IHostEnvironment environment)
    : IRequestHandler<GetInfoQuery, GetInfoResponse>
{
    public Task<GetInfoResponse> Handle(
        GetInfoQuery request,
        CancellationToken cancellationToken)
    {
        var process = Process.GetCurrentProcess();
        
        var response = new GetInfoResponse
        {
            Name = configuration["App:Name"],
            Version = configuration["App:Version"],
            Environment = environment.EnvironmentName,
            Hostname = System.Net.Dns.GetHostName(),
            OsVersion = RuntimeInformation.OSDescription,
            ProcessorCount = Environment.ProcessorCount,
            FrameworkDescription = RuntimeInformation.FrameworkDescription,
            MemoryUsage = process.WorkingSet64,
            StartTime = process.StartTime.ToString("O"),
            CurrentTime = DateTime.UtcNow.ToString("O"),
            MachineArchitecture = RuntimeInformation.ProcessArchitecture.ToString(),
            ActiveThreads = process.Threads.Count
        };
        
        return Task.FromResult(response);
    }
}