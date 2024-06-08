using HealthCheck.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics;


namespace HealthCheck.Checks;

/// <summary>
/// Вывод системных параметров
/// </summary>
public sealed class SystemCheck : INamedCheck
{
    public string CheckName => "SystemCheck";

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            using var currentProcess = Process.GetCurrentProcess();

            var data = new Dictionary<string, object>()
            {
                { "Process name", currentProcess.ProcessName },
                { "Process id", currentProcess.Id },
                { "Physical memory usage", currentProcess.WorkingSet64.ToString() },
                { "Paged system memory size", currentProcess.PagedSystemMemorySize64.ToString() },
                { "Paged memory size", currentProcess.PagedMemorySize64.ToString() },
                { "User processor time", currentProcess.UserProcessorTime.ToString() },
                { "Privileged processor time", currentProcess.PrivilegedProcessorTime.ToString() },
                { "Total processor time", currentProcess.TotalProcessorTime.ToString() }
            };

            return Task.FromResult(HealthCheckResult.Healthy("Собрана информация о процессе", data));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Ошибка при сборе информации о процессе", ex));
        }
    }
}