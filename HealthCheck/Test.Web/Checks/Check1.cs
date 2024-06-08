using Microsoft.Extensions.Diagnostics.HealthChecks;
using HealthCheck.Interfaces;
using HealthCheck.Models;


namespace Test.Web.Checks;

public class Check1 : INamedCheck
{
    public string CheckName => "Check1";

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var check1 = Check.GetBuilder()
            .Passed("Проверка пройдена")
            .SetName("Проверка API 1")
            .StartAt(DateTime.Now)
            .EndAt(DateTime.Now.AddSeconds(3))
            .Build();

        var check2 = Check.GetBuilder()
            .Passed("Проверка пройдена")
            .SetName("Проверка API 2")
            .StartAt(DateTime.Now)
            .EndAt(DateTime.Now.AddSeconds(3))
            .Build();

        var data = new Dictionary<string, object>
        {
            { "api1", check1 },
            { "api2", check2 }
        };

        var result = HealthCheckResult.Healthy("API функционирует хорошо", data);

        return Task.FromResult(result);
    }
}