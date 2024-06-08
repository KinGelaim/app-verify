using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;


namespace HealthCheck.Modules.Kestrel;

public partial class KestrelHealthCheck
{
    private readonly WebApplication _app;

    private KestrelHealthCheck(WebApplication app)
    {
        _app = app;
    }

    /// <summary>
    /// Создание построителя для модуля HealthCheck
    /// </summary>
    public static KestrelHealthCheckBuilder GetBuilder() => new();

    /// <summary>
    /// Запуск веб-сервера для ожидания обращений к healthCheck
    /// </summary>
    /// <param name="logger">Логгер</param>
    public async Task RunAsync(ILogger? logger = null)
    {
        _ = _app.RunAsync()
            .ContinueWith(t =>
            {
                logger?.LogError(t.Exception, $"{nameof(KestrelHealthCheck)}.{nameof(RunAsync)}: error during starting healthCheck.");
            }, TaskContinuationOptions.OnlyOnFaulted);
    }
}