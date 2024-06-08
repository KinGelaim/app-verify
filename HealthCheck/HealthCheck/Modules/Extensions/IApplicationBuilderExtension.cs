using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;


namespace HealthCheck.Modules.Extensions;

/// <summary>
/// Класс расширений для IApplicationBuilder
/// </summary>
public static class IApplicationBuilderExtension
{
    /// <summary>
    /// Устанавливает конечную точку для вывода в json
    /// </summary>
    /// <param name="app">Сам appbuilder</param>
    /// <param name="path">Путь до вывода в json (либо значение по умолчанию)</param>
    public static void UseHealthChecksJson(this IApplicationBuilder app, string path = "/health/json")
    {
        if (!path.StartsWith("/"))
        {
            path = "/" + path;
        }

        app.UseHealthChecks(path, new HealthCheckOptions
        {
            ResponseWriter = HealthWriter.JsonWriter
        });
    }

    /// <summary>
    /// Устанавливает конечную точку для вывода в виде веб страницы
    /// </summary>
    /// <param name="app">Сам appbuilder</param>
    /// <param name="path">Путь до вывода в html (либо значение по умолчанию)</param>
    public static void UseHealthChecksHtml(this IApplicationBuilder app, string path = "/health/html")
    {
        if (!path.StartsWith("/"))
        {
            path = "/" + path;
        }

        app.UseHealthChecks(path, new HealthCheckOptions
        {
            ResponseWriter = HealthWriter.HtmlWriter
        });
    }
}