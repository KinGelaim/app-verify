using HealthCheck.Models.HTML;
using HealthCheck.Modules.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;


namespace HealthCheck.Modules;

/// <summary>
/// Содержит в себе правила вывода результатов хелсчека
/// </summary>
public sealed class HealthWriter
{
    #region Json

    private static readonly JsonSerializerOptions _serializerOptions = new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

    private static readonly JsonWriterOptions _options = new()
    {
        Indented = true,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
    };

    /// <summary>
    /// Вывод в json
    /// </summary>
    /// <param name="context">контекст запроса</param>
    /// <param name="report">результат хелсчека</param>
    public static Task JsonWriter(HttpContext context, HealthReport report)
    {
        if (report is null)
        {
            return context.Response.WriteAsync("Пустой отчёт по проверке работоспособности приложения");
        }

        context.Response.ContentType = "application/json; charset=utf-8";

        try
        {
            using var memoryStream = new MemoryStream();
            using (var jsonWriter = new Utf8JsonWriter(memoryStream, _options))
            {
                jsonWriter.WriteStartObject();
                jsonWriter.WriteString("now", DateTime.Now.ToDateAndTime());
                jsonWriter.WriteString("status", report.Status.ToString());
                jsonWriter.WriteStartObject("modules");

                if (report.Entries != null)
                {
                    foreach (var healthReportEntry in report.Entries)
                    {
                        jsonWriter.WriteStartObject(healthReportEntry.Key);
                        jsonWriter.WriteString("status", healthReportEntry.Value.Status.ToString());
                        jsonWriter.WriteString("description", healthReportEntry.Value.Description);

                        GoThroughEntry(healthReportEntry.Value, jsonWriter);

                        jsonWriter.WriteEndObject();
                    }
                }

                jsonWriter.WriteEndObject();
                jsonWriter.WriteEndObject();
            }

            return context.Response.WriteAsync(Encoding.UTF8.GetString(memoryStream.ToArray()));
        }
        catch (Exception ex)
        {
            return context.Response.WriteAsync($"Ошибка при формировании HealthCheck: {ex.Message}");
        }
    }

    private static void GoThroughEntry(HealthReportEntry entry, Utf8JsonWriter writer)
    {
        foreach (var item in entry.Data)
        {
            writer.WritePropertyName(item.Key);

            JsonSerializer.Serialize(writer, item.Value, item.Value?.GetType() ?? typeof(object), _serializerOptions);
        }
    }

    #endregion Json


    #region HTML

    /// <summary>
    /// Формирования ответа в виде HTML страницы
    /// </summary>
    /// <param name="context">контекст запроса</param>
    /// <param name="report">результат хелсчека</param>
    public static Task HtmlWriter(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "text/html; charset=utf-8";

        if (report is null)
        {
            return context.Response.WriteAsync("Пустой отчёт по проверке работоспособности приложения");
        }

        try
        {
            var template = new HtmlTemplate(report);

            var fileContent = template.RenderHtml();

            return context.Response.WriteAsync(fileContent);
        }
        catch (Exception ex)
        {
            return context.Response.WriteAsync($"Ошибка при формировании HealthCheck: {ex.Message}");
        }
    }

    #endregion HTML
}