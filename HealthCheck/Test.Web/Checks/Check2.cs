using HealthCheck.Interfaces;
using HealthCheck.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;


namespace Test.Web.Checks;

public class Check2 : INamedCheck
{
    public string CheckName => "Check2";

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var check1 = Check.GetBuilder()
            .Partially("Тормозит дико", "Проверка пройдена частично")
            .SetName("Проверка DB 1")
            .StartAt(DateTime.Now)
            .EndAt(DateTime.Now.AddSeconds(3))
            .Build();

        var check2 = Check.GetBuilder()
            .Error("Отвалилась БД", "Проверка не пройдена")
            .SetName("Проверка DB 2")
            .StartAt(DateTime.Now)
            .EndAt(DateTime.Now.AddSeconds(3))
            .Build();

        var check3 = Check.GetBuilder()
            .Passed("passed")
            .Partially("warning", "partially")
            .Error("error", "error")
            .Build();

        var data = new Dictionary<string, object>
        {
            { "db1", check1 },
            { "db2", check2 },
            { "db3", check3 }
        };

        var result = HealthCheckResult.Unhealthy("DB функционирует плохо", data: data);

        return Task.FromResult(result);
    }
}