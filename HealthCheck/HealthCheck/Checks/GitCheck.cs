using HealthCheck.Interfaces;
using HealthCheck.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;


namespace HealthCheck.Checks;

/// <summary>
/// Создание результатов проверки репозитория GIT
/// </summary>
public sealed class GitCheck : INamedCheck
{
    public string CheckName => "GitCheck";

    private readonly GitInfo? _gitInfo;

    public GitCheck() { }

    public GitCheck(GitInfo? gitInfo = null)
    {
        _gitInfo = gitInfo;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (_gitInfo is null)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Отсутствует информация о гите"));
        }

        var data = new Dictionary<string, object>
        {
            { "tag", _gitInfo.Tag },
            { "branch", _gitInfo.Branch },
            { "version", _gitInfo.Version },
            { "commit", _gitInfo.Commit },
            { "commit date", _gitInfo.CommitDate }
        };

        var result = HealthCheckResult.Healthy("Собрана информация по GIT", data);

        return Task.FromResult(result);
    }
}