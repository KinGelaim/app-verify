using HealthCheck.Interop;
using HealthCheck.Models.Enum;
using HealthCheck.Modules.Extensions;


namespace HealthCheck.Models;

/// <summary>
/// Результат проверки здоровья какого-то шага
/// </summary>
public partial class Check
{
    private Check() { }

    private DateTime? _checkStartTime;

    private DateTime? _checkEndTime;

    private CheckStatus _status;

    public static CheckBuilder GetBuilder() => new();

    /// <summary>
    /// Наименование проверки
    /// </summary>
    public string? Name { get; private set; }

    /// <summary>
    /// Описание результата проверки
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Ошибка проверки, если есть
    /// </summary>
    public string? Error { get; private set; }

    /// <summary>
    /// Предупреждение проверки, если есть
    /// </summary>
    public string? Warning { get; private set; }

    /// <summary>
    /// Описание статуса проверки
    /// </summary>
    public string StatusDescription => _status.ToDescription();

    /// <summary>
    /// Статус проверки
    /// </summary>
    public CheckStatus Status => _status;

    /// <summary>
    /// Проверка начата в
    /// </summary>
    public string? CheckStartTime => _checkStartTime.HasValue ? _checkStartTime.Value.ToDateAndTime() : null;

    /// <summary>
    /// Проверка закончена в
    /// </summary>
    public string? CheckEndTime => _checkEndTime.HasValue ? _checkEndTime.Value.ToDateAndTime() : null;
}